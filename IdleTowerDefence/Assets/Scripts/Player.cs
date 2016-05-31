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
		BigIntWithUnit PriceDamageUpgrade = 100;
        BigIntWithUnit PriceRangeUpgrade = 100;
        BigIntWithUnit PriceFirerateUpgrade = 100;
        BigIntWithUnit PricePlayerSpellUpgrade = 100;
		float UpgradeLevelDamage = 1;
		float UpgradeLevelRange = 1;
		float UpgradeLevelFirerate = 1;
		float UpgradeLevelPlayerSpell = 1;

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
			Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit floorHit;
			if (Physics.Raycast (camRay, out floorHit, Mathf.Infinity, FloorMask) && Input.GetMouseButtonDown(0)) {
				var floor2Cam = Camera.main.transform.position - floorHit.point;
				var instantPos = floorHit.point + floor2Cam.normalized*12;
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
            if (!reset) {
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
            } else {
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

		void UpdateLabels() {
			CurrText.text = "Currency:\n" + _currency.ToString();
			WaveText.text = "Wave:\n" + (_currentWave+1).ToString();
			WaveLifeText.text = "Wave Life:\n" + CalculateWaveLife().ToString();
			MageText.text = "Mage:\n";
			IncomeText.text = "Income:\n";
			DamageUpgrade.text = "Upgrade Mage Damage ("  + PriceDamageUpgrade + ")";
			RangeUpgrade.text = "Upgrade Mage Range ("  + PriceRangeUpgrade + ")";
			RateUpgrade.text = "Upgrade Mage Fire Rate ("  + PriceFirerateUpgrade + ")";
			PlayerUpgrade.text = "Upgrade Player Spell ("  + PricePlayerSpellUpgrade + ")";
		}

		public void UpgradeDamage(){
			if (_currency >= PriceDamageUpgrade) {

				//Upgrade
				TowerSpell.GetComponent<TowerSpell>().Damage += 20;

				//Scaling
				_currency = _currency - PriceDamageUpgrade;
				UpgradeLevelDamage = UpgradeLevelDamage*1.1f;
				PriceDamageUpgrade.IncreasePercent((int)((UpgradeLevelDamage-1)*100));
			}

		}

		public void UpgradeRange(){
			if (_currency >= PriceRangeUpgrade) {

				//Upgrade
				TowerSpell.GetComponent<TowerSpell>().Range += 2;

				//Scaling
				_currency = _currency - PriceRangeUpgrade;
				UpgradeLevelRange = UpgradeLevelRange*1.1f;
				PriceRangeUpgrade.IncreasePercent((int)((UpgradeLevelRange - 1) * 100));
			}
		}

		public void UpgradeRate(){
			if (_currency >= PriceFirerateUpgrade) {
				//Upgrade
				TowerSpell.GetComponent<TowerSpell>().Speed += 20;

				//Scaling
				_currency = _currency - PriceFirerateUpgrade;
				UpgradeLevelFirerate = UpgradeLevelFirerate*1.1f;
				PriceFirerateUpgrade.IncreasePercent((int)((UpgradeLevelFirerate - 1) * 100));
			}
		}

		public void UpgradePlayer(){
			if (_currency >= PricePlayerSpellUpgrade) {
				//Upgrade
				PlayerSpellPrefab.GetComponent<PlayerSpell>().Damage += 5;

				//Scaling
				_currency = _currency - PricePlayerSpellUpgrade;
				UpgradeLevelPlayerSpell = UpgradeLevelPlayerSpell * 1.1f;
				PricePlayerSpellUpgrade.IncreasePercent ((int)((UpgradeLevelPlayerSpell - 1) * 100));
			}
		}

        public List<Minion> GetMinionList()
        {
            return _wave;
        }
    }
}