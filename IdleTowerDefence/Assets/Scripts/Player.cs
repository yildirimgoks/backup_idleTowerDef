using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public Minion MinionPrefab;
        public Mage MagePrefab;
        public Minion BossPrefab;
        public Waypoint StartWaypoint;
        public GameObject PlayerSpellPrefab;
        public GameObject TowerSpell;

        public Text CurrText;
        public Text WaveText;
        public Text WaveLifeText;
        public Text MageText;
        public Text IncomeText;
        public Text DamageUpgrade;
        public Text RangeUpgrade;
        public Text RateUpgrade;
        public Text PlayerUpgrade;

        public LayerMask FloorMask;
        public LayerMask IgnorePlayerSpell;

        //Upgrade System Variables
        private BigIntWithUnit _priceDamageUpgrade = 100;
        private BigIntWithUnit _priceRangeUpgrade = 100;
        private BigIntWithUnit _priceFirerateUpgrade = 100;
        private BigIntWithUnit _pricePlayerSpellUpgrade = 100;
        private float _upgradeLevelDamage = 1;
        private float _upgradeLevelRange = 1;
        private float _upgradeLevelFirerate = 1;
        private float _upgradeLevelPlayerSpell = 1;

        private BigIntWithUnit _currency;

        // Stores minions
        private readonly List<Minion> _wave = new List<Minion>();

        // Minion amount in a wave
        private int _waveLength = 30;

        private int _currentWave = 0;

        // If a minion survives from towers, the bool is set to true
        // It is used for reseting the wave.
        private bool _minionSurvived;

        // Use this for initialization
        private void Start()
        {
            _currency = new BigIntWithUnit();

            SendWave(true);

            //BugFix for Upgrades Not Resetting on New Game
            TowerSpell.GetComponent<TowerSpell>().Damage = 20;
            TowerSpell.GetComponent<TowerSpell>().Range = 10;
            TowerSpell.GetComponent<TowerSpell>().Speed = 70;
            PlayerSpellPrefab.GetComponent<PlayerSpell>().Damage = 20;
        }

        // Update is called once per frame
        private void Update()
        {
            //PlayerSpell Targeting
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit floorHit;
            if (Physics.Raycast(camRay, out floorHit, Mathf.Infinity, FloorMask) && Input.GetMouseButtonDown(0))
            {
                var floor2Cam = Camera.main.transform.position - floorHit.point;
                var instantPos = floorHit.point + floor2Cam.normalized * 12;
                PlayerSpell.Clone(PlayerSpellPrefab, instantPos, FindClosestMinion(instantPos));
            }

            //1M Currency Cheat
            if (Input.GetKeyDown(KeyCode.M))
            {
                _currency += 1000000;
            }

            if (_wave.Count == 0)
            {
                Debug.Log("Minions No More");
                SendWave(_minionSurvived);
                _minionSurvived = false;
            }
            UpdateLabels();
        }

        // Minion calls this function, when it is destroyed
        public void MinionDied(Minion minion, BigIntWithUnit currencyGivenOnDeath)
        {
            if (_wave.Contains(minion))
            {
                IncreaseCurrency(currencyGivenOnDeath);
                _wave.Remove(minion);
                if (minion.tag == "Boss")
                {
                    Instantiate(MagePrefab, new Vector3(minion.transform.position.x, 12.2f, minion.transform.position.z), Quaternion.Euler(0, 0, 90));
                }
            }
        }

        // Minion calls this function, when it survives from Tower or Player
        public void MinionSurvived(Minion survivor)
        {
            _minionSurvived = true;
            _wave.Remove(survivor);
            Destroy(survivor.gameObject);
        }

        //Total life of all alive minions
        private BigIntWithUnit CalculateWaveLife()
        {
            return _wave.Aggregate(new BigIntWithUnit(), (life, minion) => life + minion.GetComponent<Minion>().Life);
        }

        // Creates a new wave from the beginning point
        // If reset is true, the amount of minions in a wave doesn't change.
        private void SendWave(bool reset)
        {
            if (!reset)
            {
                _currentWave++;
            }
            if ((_currentWave + 1) % 5 == 0)
            {
                var bossPos = StartWaypoint.transform.position;
                var bossRot = StartWaypoint.transform.rotation;
                var boss = Instantiate(BossPrefab, bossPos, bossRot) as Minion;
                boss.Life = (_currentWave + 1) * 200;
                boss.CurrencyGivenOnDeath = boss.Life;
                boss.tag = "Boss";
                _wave.Add(boss);
            }
            else
            {
                double multiplierLife = System.Math.Pow(1.1, _currentWave);
                double multiplierMoney = System.Math.Pow(1.03, _currentWave);
                for (var i = 0; i < _waveLength; i++)
                {
                    //var instantPos = new Vector3(MinionPrefab.transform.position.x, MinionPrefab.transform.position.y,
                    //MinionPrefab.transform.position.z - 2*i);

                    var instantPos = StartWaypoint.transform.position - StartWaypoint.transform.forward * 5 * i;
                    var instantRot = StartWaypoint.transform.rotation;

                    var clone = Instantiate(MinionPrefab, instantPos, instantRot) as Minion;
                    if (clone == null) continue;
                    clone.Life = BigIntWithUnit.MultiplyPercent(Minion.BaseLife, multiplierLife * 100);
                    clone.CurrencyGivenOnDeath = BigIntWithUnit.MultiplyPercent(Minion.BaseCurrencyGivenOnDeath, multiplierMoney * 100);
                    clone.tag = "Minion";
                    _wave.Add(clone);
                }
            }
        }

        //returns if there are any minion on map
        public bool AnyMinionOnMap()
        {
            return _wave.Any(minion => minion.GetComponent<Minion>().OnMap);
        }

        // Find closest minion's name
        public Minion FindClosestMinion(Vector3 position)
        {
            Minion closestMinion = null;
            var distance = Mathf.Infinity;
            foreach (var minion in _wave)
            {
                var curDistance = Vector3.Distance(minion.transform.position, position);
                if (curDistance < distance)
                {
                    closestMinion = minion;
                    distance = curDistance;
                }
            }
            return closestMinion;
        }

        public void IncreaseCurrency(BigIntWithUnit amount)
        {
            _currency += amount;
        }

        public void DecreaseCurrency(BigIntWithUnit amount)
        {
            _currency -= amount;
        }

        private void UpdateLabels()
        {
            CurrText.text = "Currency:\n" + _currency.ToString();
            WaveText.text = "Wave:\n" + (_currentWave + 1).ToString();
            WaveLifeText.text = "Wave Life:\n" + CalculateWaveLife().ToString();
            MageText.text = "Mage:\n";
            IncomeText.text = "Income:\n";
            DamageUpgrade.text = "Upgrade Mage Damage (" + _priceDamageUpgrade + ")";
            RangeUpgrade.text = "Upgrade Mage Range (" + _priceRangeUpgrade + ")";
            RateUpgrade.text = "Upgrade Mage Fire Rate (" + _priceFirerateUpgrade + ")";
            PlayerUpgrade.text = "Upgrade Player Spell (" + _pricePlayerSpellUpgrade + ")";
        }

        public void UpgradeDamage()
        {
            if (_currency >= _priceDamageUpgrade)
            {
                //Upgrade
                TowerSpell.GetComponent<TowerSpell>().Damage += 20;

                //Scaling
                _currency = _currency - _priceDamageUpgrade;
                _upgradeLevelDamage = _upgradeLevelDamage * 1.1f;
                _priceDamageUpgrade.IncreasePercent((int)((_upgradeLevelDamage - 1) * 100));
            }
        }

        public void UpgradeRange()
        {
            if (_currency >= _priceRangeUpgrade)
            {
                //Upgrade
                TowerSpell.GetComponent<TowerSpell>().Range += 2;

                //Scaling
                _currency = _currency - _priceRangeUpgrade;
                _upgradeLevelRange = _upgradeLevelRange * 1.1f;
                _priceRangeUpgrade.IncreasePercent((int)((_upgradeLevelRange - 1) * 100));
            }
        }

        public void UpgradeRate()
        {
            if (_currency >= _priceFirerateUpgrade)
            {
                //Upgrade
                TowerSpell.GetComponent<TowerSpell>().Speed += 20;

                //Scaling
                _currency = _currency - _priceFirerateUpgrade;
                _upgradeLevelFirerate = _upgradeLevelFirerate * 1.1f;
                _priceFirerateUpgrade.IncreasePercent((int)((_upgradeLevelFirerate - 1) * 100));
            }
        }

        public void UpgradePlayer()
        {
            if (_currency >= _pricePlayerSpellUpgrade)
            {
                //Upgrade
                PlayerSpellPrefab.GetComponent<PlayerSpell>().Damage += 5;

                //Scaling
                _currency = _currency - _pricePlayerSpellUpgrade;
                _upgradeLevelPlayerSpell = _upgradeLevelPlayerSpell * 1.1f;
                _pricePlayerSpellUpgrade.IncreasePercent((int)((_upgradeLevelPlayerSpell - 1) * 100));
            }
        }

        public List<Minion> GetMinionList()
        {
            return _wave;
        }
    }
}