using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        private static float _maxZ;
        private static float _minZ;
        private static float _maxX;
        private static float _minX;

        private BigIntWithUnit _currency;
        public GameObject Floor;
        public Minion MinionPrefab;

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

		//Total Mage damage on the map
		private int calculatedps(){
			return 200;
		}

        // Use this for initialization
        private void Start()
        {
            _currency = new BigIntWithUnit();
            // Calculating area of the Plane
            var plane = GameObject.FindGameObjectsWithTag("Floor")[0];
            var difz = plane.GetComponent<Collider>().bounds.size.z/2;
            var difx = plane.GetComponent<Collider>().bounds.size.x/2;
            _maxZ = plane.transform.position.z + difz;
            _minZ = plane.transform.position.z - difz;
            _maxX = plane.transform.position.x + difx;
            _minX = plane.transform.position.x - difx;

            SendWave(true);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && PlayerSpellPrefab.GetComponent<PlayerSpell>().FindClosestMinion() != null)
            {
                var mousePos = Input.mousePosition;
                mousePos.z = Camera.main.transform.position.y - 5;
                var instantPos = Camera.main.ScreenToWorldPoint(mousePos);
                Instantiate(PlayerSpellPrefab, instantPos, Quaternion.identity);
            }
        }


        // Minion calls this function, when it is destroyed
        public void MinionDied(Minion minion, BigIntWithUnit currencyGivenOnDeath)
        {
            if (_wave.Contains(minion))
            {
                IncreaseCurrency(currencyGivenOnDeath);
                _wave.Remove(minion);
                if (_wave.Count == 0)
                {
                    Debug.Log("All Minions are Killed");
                    SendWave(_minionSurvived);
                }
            }
        }

        // Minion calls this function, when it survives from Tower or Player
        public void MinionSurvived(Minion survivor)
        {
            _minionSurvived = true;
            MinionDied(survivor, 0);
            Destroy(survivor.gameObject);
        }

        // Creates a new wave from the beginning point
        // If reset is true, the amount of minions in a wave doesn't change.
        private void SendWave(bool reset)
        {
            if (!reset)
                _waveLength++;
            for (var i = 0; i < _waveLength; i++)
            {
                var instantPos = new Vector3(MinionPrefab.transform.position.x, MinionPrefab.transform.position.y,
                    MinionPrefab.transform.position.z - 2*i);
                var clone = Instantiate(MinionPrefab, instantPos, Quaternion.identity) as Minion;
                if (clone == null) continue;
                clone.tag = "Minion";
                _wave.Add(clone);
            }
        }

        // Returns if the given gameObject is on Map
        public static bool OnMap(GameObject gameObject)
        {
            var pos = gameObject.transform.position;
            return pos.x < _maxX && pos.x > _minX && pos.z > _minZ && pos.z < _maxZ;
        }

        // Returns if there are any Minion on Map
        public static bool AnyMinionOnMap()
        {
            return GameObject.FindGameObjectsWithTag("Minion").Any(OnMap);
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
            GUI.Label(new Rect(10, 0, 100, 20), "Currency: " + _currency);
			GUI.Label(new Rect (110,0, 80, 20), "Wave: " + (_waveLength-29));
			GUI.Label(new Rect(190, 0, 100, 20), "Wave Life: " + calculatedps());
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
				DamageUpgrade = "Upgrade Mage Damage (" + PriceDamageUpgrade + ")";
			}

			if (GUI.Button(new Rect(370,80,200,30), RangeUpgrade) && _currency >= PriceRangeUpgrade) {

				//Upgrade
				TowerSpell.GetComponent<TowerSpell>().range = Mathf.RoundToInt(TowerSpell.GetComponent<TowerSpell>().range + 2);

				//Scaling
				_currency = _currency - PriceDamageUpgrade;
				UpgradeLevelRange = UpgradeLevelRange*1.1f;
                PriceRangeUpgrade.IncreasePercent((int)((UpgradeLevelRange - 1) * 100));
				RangeUpgrade = "Upgrade Mage Range (" + PriceRangeUpgrade + ")";
			}

			if (GUI.Button(new Rect(370,120,200,30), FirerateUpgrade) && _currency >= PriceFirerateUpgrade) {
				//Upgrade
				TowerSpell.GetComponent<TowerSpell>().speed = Mathf.RoundToInt(TowerSpell.GetComponent<TowerSpell>().speed + 20);

				//Scaling
				_currency = _currency - PriceFirerateUpgrade;
				UpgradeLevelFirerate = UpgradeLevelFirerate*1.1f;
                PriceFirerateUpgrade.IncreasePercent((int)((UpgradeLevelFirerate - 1) * 100));
                FirerateUpgrade = "Upgrade Mage Fire Rate(" + PriceFirerateUpgrade + ")";
			}
				
			if (GUI.Button(new Rect(370,160,200,30), PlayerSpellUpgrade) && _currency >= PricePlayerSpellUpgrade) {
				//Upgrade
				PlayerSpellPrefab.GetComponent<PlayerSpell>().Damage = PlayerSpellPrefab.GetComponent<PlayerSpell>().Damage + 5;

				//Scaling
				_currency = _currency - PricePlayerSpellUpgrade;
				UpgradeLevelPlayerSpell = UpgradeLevelPlayerSpell*1.1f;
                PricePlayerSpellUpgrade.IncreasePercent((int)((UpgradeLevelPlayerSpell - 1) * 100));
                PlayerSpellUpgrade = "Upgrade Player Spell (" + PricePlayerSpellUpgrade + ")";
			}
		} 


        public List<Minion> getList()
        {
            return _wave;
        }
    }
}