using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {

        private BigIntWithUnit _currency;
        public GameObject Floor;
        public Minion MinionPrefab;
        public Waypoint startWaypoint;
        public LayerMask IgnorePlayerSpell;

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

		//Total life of all alive minions
		private BigIntWithUnit CalculateWaveLife(){
		BigIntWithUnit WaveLife=0;
		var minions = GameObject.FindGameObjectsWithTag("Minion");
			foreach (var minion in minions) {
				WaveLife = WaveLife + minion.GetComponent<Minion> ().Life;
			} 
		
			return WaveLife;
		}

        // Rounds cleared
        private int _rounds = 0;

        private int _newLife;

        // Use this for initialization
        private void Start()
        {
            _currency = new BigIntWithUnit();

            SendWave(true);

			//BugFix for Upgrades Not Resetting on New Game
			TowerSpell.GetComponent<TowerSpell>().damage = 20;
			TowerSpell.GetComponent<TowerSpell> ().range = 10;
			TowerSpell.GetComponent<TowerSpell> ().speed = 70;
			PlayerSpellPrefab.GetComponent<PlayerSpell> ().Damage = 20;
        }

        // Update is called once per frame
        private void Update()
        {
			if (Input.GetMouseButtonDown(0))
            {
                var mousePos = Input.mousePosition;
                mousePos.z = Camera.main.transform.position.y - 5;
                if (!Physics.Raycast(Camera.main.transform.position, Camera.main.ScreenToWorldPoint(mousePos) - Camera.main.transform.position, Mathf.Infinity, IgnorePlayerSpell) && PlayerSpellPrefab.GetComponent<PlayerSpell>().FindClosestMinion())
                {
                    var instantPos = Camera.main.ScreenToWorldPoint(mousePos);
                    PlayerSpell.Clone(PlayerSpellPrefab, instantPos);
                }
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
        }


        // Minion calls this function, when it is destroyed
        public void MinionDied(Minion minion, BigIntWithUnit currencyGivenOnDeath)
        {
            if (_wave.Contains(minion))
            {
                IncreaseCurrency(currencyGivenOnDeath);
                _wave.Remove(minion);
                
            }
        }

        // Minion calls this function, when it survives from Tower or Player
        public void MinionSurvived(Minion survivor)
        {
            _minionSurvived = true;
            _wave.Remove(survivor);
            Destroy(survivor.gameObject);
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
                // MinionPrefab.transform.position.z - 2*i);
                var instantPos = startWaypoint.transform.position - startWaypoint.transform.forward * 2*i;
                var instantRot = startWaypoint.transform.rotation;
                var clone = Instantiate(MinionPrefab, instantPos, instantRot) as Minion;
                if (clone == null) continue;
                clone.Life = _newLife;
                clone.tag = "Minion";
                _wave.Add(clone);
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
            GUI.Label(new Rect(10, 20, 200, 20), "Currency: " + _currency);
			GUI.Label(new Rect (110,0, 80, 20), "Wave: " + (_waveLength-29));
			GUI.Label(new Rect(190, 0, 100, 20), "Wave Life: " + CalculateWaveLife());
			GUI.Label(new Rect(290, 0, 80, 20), "Mage: ");
			GUI.Label(new Rect(370, 0, 50, 20), "Income: ");
			Upgrade ();
		}


		 void Upgrade() {

			string DamageUpgrade = "Upgrade Mage Damage ("  + PriceDamageUpgrade + ")";
			string RangeUpgrade = "Upgrade Mage Range ("  + PriceRangeUpgrade + ")";
			string FirerateUpgrade = "Upgrade Mage Fire Rate ("  + PriceFirerateUpgrade + ")";
			string PlayerSpellUpgrade = "Upgrade Player Spell ("  + PricePlayerSpellUpgrade + ")";


			if (GUI.Button(new Rect(370,40,200,30), DamageUpgrade) && _currency >= PriceDamageUpgrade) {
				
				//Upgrade
				TowerSpell.GetComponent<TowerSpell>().damage = Mathf.RoundToInt(TowerSpell.GetComponent<TowerSpell>().damage + 20);

				//Scaling
				_currency = _currency - PriceDamageUpgrade;
				UpgradeLevelDamage = UpgradeLevelDamage*1.1f;
				PriceDamageUpgrade.IncreasePercent((int)((UpgradeLevelDamage-1)*100));
			}

			if (GUI.Button(new Rect(370,80,200,30), RangeUpgrade) && _currency >= PriceRangeUpgrade) {

				//Upgrade
				TowerSpell.GetComponent<TowerSpell>().range = Mathf.RoundToInt(TowerSpell.GetComponent<TowerSpell>().range + 2);

				//Scaling
				_currency = _currency - PriceRangeUpgrade;
				UpgradeLevelRange = UpgradeLevelRange*1.1f;
                PriceRangeUpgrade.IncreasePercent((int)((UpgradeLevelRange - 1) * 100));
			}

			if (GUI.Button(new Rect(370,120,200,30), FirerateUpgrade) && _currency >= PriceFirerateUpgrade) {
				//Upgrade
				TowerSpell.GetComponent<TowerSpell>().speed = Mathf.RoundToInt(TowerSpell.GetComponent<TowerSpell>().speed + 20);

				//Scaling
				_currency = _currency - PriceFirerateUpgrade;
				UpgradeLevelFirerate = UpgradeLevelFirerate*1.1f;
                PriceFirerateUpgrade.IncreasePercent((int)((UpgradeLevelFirerate - 1) * 100));
			}
				
			if (GUI.Button(new Rect(370,160,200,30), PlayerSpellUpgrade) && _currency >= PricePlayerSpellUpgrade) {
				//Upgrade
				PlayerSpellPrefab.GetComponent<PlayerSpell>().Damage = PlayerSpellPrefab.GetComponent<PlayerSpell>().Damage + 5;

				//Scaling
				_currency = _currency - PricePlayerSpellUpgrade;
				UpgradeLevelPlayerSpell = UpgradeLevelPlayerSpell*1.1f;
                PricePlayerSpellUpgrade.IncreasePercent((int)((UpgradeLevelPlayerSpell - 1) * 100));
			}
		} 


        public List<Minion> getList()
        {
            return _wave;
        }
    }
}