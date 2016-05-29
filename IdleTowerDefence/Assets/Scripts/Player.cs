using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        private BigIntWithUnit _currency;
        public Minion MinionPrefab;
        public Mage MagePrefab;
        public Minion BossPrefab;
        public Waypoint StartWaypoint;
        public LayerMask IgnorePlayerSpell;
		public Text CurrText;
		public Text WaveText;
		public Text WaveLifeText;
		public Text MageText;
		public Text IncomeText;
		public Text DamageUpgrade;
		public Text RangeUpgrade;
		public Text RateUpgrade;
		public Text PlayerUpgrade;
		float camRayLength = 100f;
		int floorMask;
		Animator MinionAnim;

		//Upgrade System Variables
		public GameObject TowerSpell;
		BigIntWithUnit PriceDamageUpgrade = 100;
        BigIntWithUnit PriceRangeUpgrade = 100;
        BigIntWithUnit PriceFirerateUpgrade = 100;
        BigIntWithUnit PricePlayerSpellUpgrade = 100;
		float UpgradeLevelDamage = 1;
		float UpgradeLevelRange = 1;
		float UpgradeLevelFirerate = 1;
		float UpgradeLevelPlayerSpell = 1;


        // If a minion survives from towers, the bool is set to true
        // It is used for reseting the wave.
        private bool _minionSurvived;

        public GameObject PlayerSpellPrefab;

        // Stores minions
        private readonly List<Minion> _wave = new List<Minion>();

        // Minion amount in a wave
        private int _waveLength = 30;

        // Rounds cleared
        private int _rounds = 0;

        private int _newLife;

        // Use this for initialization
        private void Start()
        {
            _currency = new BigIntWithUnit();

            SendWave(true);

			//BugFix for Upgrades Not Resetting on New Game
			TowerSpell.GetComponent<TowerSpell>().Damage = 20;
			TowerSpell.GetComponent<TowerSpell> ().Range = 10;
			TowerSpell.GetComponent<TowerSpell> ().Speed = 70;
			PlayerSpellPrefab.GetComponent<PlayerSpell> ().Damage = 20;

			//PlayerSpell: for mouse position raycast
			floorMask = LayerMask.GetMask ("Floor");
        }

        // Update is called once per frame
		private void Update()
        {

			Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit floorHit;
			if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask) && Input.GetMouseButtonDown(0)) {
				Vector3 instantPos = floorHit.point;
				instantPos.y = 2f;
				PlayerSpell.Clone(PlayerSpellPrefab, instantPos);
			}

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
				//MinionAnim.SetBool ("Die", true);
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
            //Maybe better to just use existing _wave array instead of findinggameobjects
            BigIntWithUnit waveLife = 0;
            var minions = GameObject.FindGameObjectsWithTag("Minion");
            foreach (var minion in minions)
            {
                waveLife += minion.GetComponent<Minion>().Life;
            }
            var boss = GameObject.FindGameObjectWithTag("Boss");
            if (boss != null)
            {
                waveLife += boss.GetComponent<Minion>().Life;
            }
            return waveLife;
        }

        // Creates a new wave from the beginning point
        // If reset is true, the amount of minions in a wave doesn't change.
        private void SendWave(bool reset)
        {
            if (!reset) { 
                _waveLength++;
                _rounds++;
            }
            double multiplier = System.Math.Pow(1.05, _rounds);
            _newLife = (int) (100 * multiplier);
            for (var i = 0; i < _waveLength; i++)
            {
                //var instantPos = new Vector3(MinionPrefab.transform.position.x, MinionPrefab.transform.position.y,
                //MinionPrefab.transform.position.z - 2*i);

				var instantPos = StartWaypoint.transform.position - StartWaypoint.transform.forward*5*i;
				var instantRot = StartWaypoint.transform.rotation;


                var clone = Instantiate(MinionPrefab, instantPos, instantRot) as Minion;
                if (clone == null) continue;
                clone.Life = _newLife;
                clone.tag = "Minion";
                _wave.Add(clone);
            }
            if (_rounds % 5 == 0 && _rounds != 0)
            {
                var bossPos = StartWaypoint.transform.position - StartWaypoint.transform.forward * 2 * _waveLength;
                var bossRot = StartWaypoint.transform.rotation;
                var boss = Instantiate(BossPrefab, bossPos, bossRot) as Minion;
                boss.Life = _rounds * 200;
                boss.tag = "Boss";
                _wave.Add(boss);
            }
        }

        //returns if there are any minion on map
        public static bool AnyMiniononMap()
        {
            var minions = GameObject.FindGameObjectsWithTag("Minion");
            foreach(var minion in minions){
                if (minion.GetComponent<Minion>().OnMap)
                {
                    return true;
                }
            }
            var boss = GameObject.FindGameObjectWithTag("Boss");
            if (boss != null && boss.GetComponent<Minion>().OnMap)
            {
                return true;
            }
            return false;  
        }

        public void IncreaseCurrency(BigIntWithUnit amount)
        {
            _currency += amount;
        }

        public void DecreaseCurrency(BigIntWithUnit amount)
        {
            _currency -= amount;
        }

        private void OnGUI() {
		}

		void UpdateLabels() {
			CurrText.text = "Currency:\n" + _currency.ToString();
			WaveText.text = "Wave:\n" + (_waveLength-29).ToString();
			WaveLifeText.text = "Wave Life:\n" + CalculateWaveLife().ToString();
			MageText.text = "Mage:\n";
			IncomeText.text = "Income:\n";
			DamageUpgrade.text = "Upgrade Mage Damage\n("  + PriceDamageUpgrade + ")";
			RangeUpgrade.text = "Upgrade Mage Range\n("  + PriceRangeUpgrade + ")";
			RateUpgrade.text = "Upgrade Mage Fire Rate\n("  + PriceFirerateUpgrade + ")";
			PlayerUpgrade.text = "Upgrade Player Spell\n("  + PricePlayerSpellUpgrade + ")";
		}

		public void UpgradeDamage(){
			if (_currency >= PriceDamageUpgrade) {

				//Upgrade
				TowerSpell.GetComponent<TowerSpell>().Damage = Mathf.RoundToInt(TowerSpell.GetComponent<TowerSpell>().Damage + 20);

				//Scaling
				_currency = _currency - PriceDamageUpgrade;
				UpgradeLevelDamage = UpgradeLevelDamage*1.1f;
				PriceDamageUpgrade.IncreasePercent((int)((UpgradeLevelDamage-1)*100));
			}

		}

		public void UpgradeRange(){
			if (_currency >= PriceRangeUpgrade) {

				//Upgrade
				TowerSpell.GetComponent<TowerSpell>().Range = Mathf.RoundToInt(TowerSpell.GetComponent<TowerSpell>().Range + 2);

				//Scaling
				_currency = _currency - PriceRangeUpgrade;
				UpgradeLevelRange = UpgradeLevelRange*1.1f;
				PriceRangeUpgrade.IncreasePercent((int)((UpgradeLevelRange - 1) * 100));
			}
		}

		public void UpgradeRate(){
			if (_currency >= PriceFirerateUpgrade) {
				//Upgrade
				TowerSpell.GetComponent<TowerSpell>().Speed = Mathf.RoundToInt(TowerSpell.GetComponent<TowerSpell>().Speed + 20);

				//Scaling
				_currency = _currency - PriceFirerateUpgrade;
				UpgradeLevelFirerate = UpgradeLevelFirerate*1.1f;
				PriceFirerateUpgrade.IncreasePercent((int)((UpgradeLevelFirerate - 1) * 100));
			}
		}

		public void UpgradePlayer(){
			if (_currency >= PricePlayerSpellUpgrade) {
				//Upgrade
				PlayerSpellPrefab.GetComponent<PlayerSpell> ().Damage = PlayerSpellPrefab.GetComponent<PlayerSpell> ().Damage + 5;

				//Scaling
				_currency = _currency - PricePlayerSpellUpgrade;
				UpgradeLevelPlayerSpell = UpgradeLevelPlayerSpell * 1.1f;
				PricePlayerSpellUpgrade.IncreasePercent ((int)((UpgradeLevelPlayerSpell - 1) * 100));
			}
		}


        public List<Minion> getList()
        {
            return _wave;
        }
    }
}