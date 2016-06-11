using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public Mage MagePrefab;
        public PlayerSpell PlayerSpellPrefab;
		public BigIntWithUnit SpellDamage = 20;
		public int SpellSpeed = 100; //TODO: BigIntWithUnit??
		public Element Element;
        public TowerSpell TowerSpell;
        public WaveManager WaveManager;

        public Text CurrText;
        public Text WaveText;
        public Text WaveLifeText;
        public Text MageText;
        public Text IncomeText;
        public Text DamageUpgrade;
        public Text RangeUpgrade;
        public Text RateUpgrade;
        public Text PlayerUpgrade;
		public Button Wave1;
		public Button Wave2;
		public Button Wave3;
		public Button Wave4;
		public Button Wave5;
		public Slider WaveLifeBar;

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

        // Use this for initialization
        private void Start()
        {
            _currency = new BigIntWithUnit();
			WaveManager.SendWave();

            //BugFix for Upgrades Not Resetting on New Game
//            TowerSpell.GetComponent<TowerSpell>().Damage = 20;
//            TowerSpell.GetComponent<TowerSpell>().Range = 10;
//            TowerSpell.GetComponent<TowerSpell>().Speed = 70;
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
				if (Time.timeScale != 0) {
					PlayerSpell.Clone (PlayerSpellPrefab, SpellDamage, SpellSpeed, Element, instantPos, WaveManager.FindClosestMinion (instantPos));
				}
            }

            //1M Currency Cheat
            if (Input.GetKeyDown(KeyCode.M))
            {
                _currency += 1000000;
            }

            if (WaveManager.AliveMinionCount == 0)
            {
                Debug.Log("Minions No More");
				if (WaveManager._minionSurvived) {
					WaveManager.SendWave();
				} else {
					WaveManager.SendNextLevelIncreaseMax();
				}
            }
            UpdateLabels();
        }

        // Minion calls this function, when it is destroyed
        public void MinionDied(Minion minion, BigIntWithUnit currencyGivenOnDeath)
        {
            if (WaveManager.SafeRemove(minion))
            {
                IncreaseCurrency(currencyGivenOnDeath);
                if (minion.tag == "Boss")
                {
                    Mage newMage = Instantiate(MagePrefab, new Vector3(minion.transform.position.x, 12.2f, minion.transform.position.z), Quaternion.Euler(0, 0, 90)) as Mage;
                    if (newMage != null)
                    {
                        newMage.Dropped = true;
                        Time.timeScale = 0;
                    }             
                }
            }
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
            WaveText.text = "Wave:\n" + (WaveManager.CurrentWave + 1).ToString();
			WaveLifeText.text = "Wave Life:\n" + WaveManager.WaveLife.ToString();
			WaveLifeBar.value = (float) BigIntWithUnit.DivideAsDouble(WaveManager.WaveLife,WaveManager.TotalWaveLife);
            MageText.text = "Mage:\n";
            IncomeText.text = "Income:\n";
			DamageUpgrade.text = "Upgrade Mage Damage\n(" + _priceDamageUpgrade + ")";
            RangeUpgrade.text = "Upgrade Mage Range\n(" + _priceRangeUpgrade + ")";
			RateUpgrade.text = "Upgrade Mage Fire Rate\n(" + _priceFirerateUpgrade + ")";
			PlayerUpgrade.text = "Upgrade Player Spell\n(" + _pricePlayerSpellUpgrade + ")";
			Wave1.GetComponentInChildren<Text> ().text ="" + (((WaveManager.CurrentWave) / 5)*5 + 1).ToString ();
			Wave2.GetComponentInChildren<Text> ().text ="" + (((WaveManager.CurrentWave) / 5)*5 + 2).ToString ();
			Wave3.GetComponentInChildren<Text> ().text ="" + (((WaveManager.CurrentWave) / 5)*5 + 3).ToString ();
			Wave4.GetComponentInChildren<Text> ().text ="" + (((WaveManager.CurrentWave) / 5)*5 + 4).ToString ();
			Wave5.GetComponentInChildren<Text> ().text ="" + (((WaveManager.CurrentWave) / 5)*5 + 5).ToString ();
			ColorBlock cb = new ColorBlock();
			switch ((WaveManager.CurrentWave) % 5) {
			case 1: cb = Wave2.colors; cb.disabledColor = Color.yellow; Wave2.colors = cb; break;
			case 2: cb = Wave3.colors; cb.disabledColor=Color.yellow; Wave3.colors = cb; break;
			case 3: cb = Wave4.colors; cb.disabledColor=Color.yellow; Wave4.colors = cb; break;
			case 4: cb = Wave5.colors; cb.disabledColor=Color.yellow; Wave5.colors = cb; break;
			}
			if (WaveManager.CurrentWave%5==0) {
					cb = Wave1.colors;
					cb.disabledColor=Color.yellow;
					Wave1.colors = cb;
					cb = Wave2.colors;
					cb.disabledColor = Color.white;
					Wave2.colors = cb;
					cb = Wave3.colors;
					cb.disabledColor = Color.white;
					Wave3.colors = cb;
					cb = Wave4.colors;
					cb.disabledColor = Color.white;
					Wave4.colors = cb;
					cb = Wave5.colors;
					cb.disabledColor = Color.white;
					Wave5.colors = cb;
			}
        }

        public void UpgradeDamage()
        {
            if (_currency >= _priceDamageUpgrade)
            {
                //Upgrade

				// TODO: Individual Mage Upgrade
				// It doesn't work right now.
				MagePrefab.GetComponent<Mage>().IncreaseSpellDamage(20);

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

				// TODO: Individual Mage Upgrade
				// It doesn't work right now.
				MagePrefab.GetComponent<Mage>().IncreaseSpellRange(2);

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

				// TODO: Individual Mage Upgrade
				// It doesn't work right now.
				MagePrefab.GetComponent<Mage>().IncreaseSpellSpeed(20);

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
				SpellDamage += 5;

                //Scaling
                _currency = _currency - _pricePlayerSpellUpgrade;
                _upgradeLevelPlayerSpell = _upgradeLevelPlayerSpell * 1.1f;
                _pricePlayerSpellUpgrade.IncreasePercent((int)((_upgradeLevelPlayerSpell - 1) * 100));
            }
        }

		//Idle Functionality Preparations
		/*
		public void OnApplicationQuit(){
		PlayerPrefs.SetString ("GameCloseTime", System.DateTime.Now.ToString());
		}

		public void CalculateIdleIncome(){
		PlayerPrefs.GetString ("GameCloseTime");
		
		}
		*/
    }
}