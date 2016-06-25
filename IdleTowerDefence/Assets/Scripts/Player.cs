using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public Mage MagePrefab;
        public PlayerSpell PlayerSpellPrefab;
		public BigIntWithUnit SpellDamage = 20;
		public int SpellSpeed = 100;
		public Element Element;
        public TowerSpell TowerSpell;
        public WaveManager WaveManager;

        public Text CurrText;
        public Text WaveText;
        public Text WaveLifeText;
        public Text MageText;
        public Text IncomeText;
        public Text PlayerUpgrade;
		public Button Wave1;
		public Button Wave2;
		public Button Wave3;
		public Button Wave4;
		public Button Wave5;
		public Slider WaveLifeBar;

        public LayerMask FloorMask;
        public LayerMask IgnorePlayerSpell;
        public EventSystem MainEventSystem;

        //Upgrade System Variables
        public BigIntWithUnit _priceDamageUpgrade = 100;
        public BigIntWithUnit _priceRangeUpgrade = 100;
        public BigIntWithUnit _priceFirerateUpgrade = 100;
        private BigIntWithUnit _pricePlayerSpellUpgrade = 100;
        private float _upgradeLevelDamage = 1;
        private float _upgradeLevelRange = 1;
        private float _upgradeLevelFirerate = 1;
        private float _upgradeLevelPlayerSpell = 1;



        private BigIntWithUnit _currency;

        private List<Mage> _mageList = new List<Mage>();

        // Use this for initialization
        private void Start()
        {
            _currency = new BigIntWithUnit();
			WaveManager.SendWave();

            GameObject[] tmp = GameObject.FindGameObjectsWithTag("Mage");
            foreach (var obj in tmp)
            {
                Mage mage = obj.GetComponent<Mage>();
                _mageList.Add(mage);
				MageButtons.Ins.AddMageButton(mage);
            }

            //BugFix for Upgrades Not Resetting on New Game
//            TowerSpell.GetComponent<TowerSpell>().Damage = 20;
//            TowerSpell.GetComponent<TowerSpell>().Range = 10;
//            TowerSpell.GetComponent<TowerSpell>().Speed = 70;
        }

        // Update is called once per frame
        private void Update()
        {
            //PlayerSpell Targeting

            if (Time.timeScale != 0 && Input.GetMouseButtonDown(0) && !MainEventSystem.IsPointerOverGameObject())
            {
                Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit floorHit;
                RaycastHit uiHit;

                if (!Physics.Raycast(camRay, out uiHit, Mathf.Infinity, IgnorePlayerSpell) &&
                Physics.Raycast(camRay, out floorHit, Mathf.Infinity, FloorMask))
                {
                    var floor2Cam = Camera.main.transform.position - floorHit.point;
                    var instantPos = floorHit.point + floor2Cam.normalized*12;
                    Spell.Clone(PlayerSpellPrefab, SpellDamage, SpellSpeed, Element, instantPos,
                            WaveManager.FindClosestMinion(instantPos));
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
                    Mage newMage = Instantiate(MagePrefab, new Vector3(minion.transform.position.x, 12.2f, minion.transform.position.z), Quaternion.Euler(0, 90, 0)) as Mage;
                    if (newMage != null)
                    {
                        _mageList.Add(newMage);
						newMage.Name = Mage.NameList[Random.Range(0,Mage.NameList.Length)];
						Debug.Log ("A wild " + newMage.Name + " appears!");
						MageButtons.Ins.AddMageButton(newMage);
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
			WaveLifeBar.value = 1 / WaveManager.TotalWaveLife.Divide(WaveManager.WaveLife);
			MageText.text = "Damage:\n" + cumulativeDPS().ToString();
            IncomeText.text = "Income:\n";
			PlayerUpgrade.text = "Upgrade Player Spell (" + _pricePlayerSpellUpgrade + ")";
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
				//Upgrades all mages, individual upgrades are in mages.
				MagePrefab.GetComponent<Mage>().IncreaseSpellDamage(20);

                //Scaling
                _currency = _currency - _priceDamageUpgrade;
                _upgradeLevelDamage = _upgradeLevelDamage * 1.1f;
                _priceDamageUpgrade.IncreasePercent((int)((_upgradeLevelDamage - 1) * 100));
				Debug.Log ("Mage Spell Damage Upgraded.");
            }
        }

        public void UpgradeRange()
        {
            if (_currency >= _priceRangeUpgrade)
            {
                //Upgrade
				//Upgrades all mages, individual upgrades are in mages.
				MagePrefab.GetComponent<Mage>().IncreaseSpellRange(2);

                //Scaling
                _currency = _currency - _priceRangeUpgrade;
                _upgradeLevelRange = _upgradeLevelRange * 1.1f;
                _priceRangeUpgrade.IncreasePercent((int)((_upgradeLevelRange - 1) * 100));
				Debug.Log ("Mage Range Upgraded.");
            }
        }

        public void UpgradeRate()
        {
            if (_currency >= _priceFirerateUpgrade)
            {
                //Upgrade
				//Upgrades all mages, individual upgrades are in mages.
				MagePrefab.GetComponent<Mage>().IncreaseSpellRate(1.2f);

                //Scaling
                _currency = _currency - _priceFirerateUpgrade;
                _upgradeLevelFirerate = _upgradeLevelFirerate * 1.1f;
                _priceFirerateUpgrade.IncreasePercent((int)((_upgradeLevelFirerate - 1) * 100));
				Debug.Log ("Mage Spell Rate Upgraded.");
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
				Debug.Log ("Player Upgraded.");
            }
        }

        public BigIntWithUnit cumulativeDPS()
        {
            BigIntWithUnit result = 0;
            foreach (Mage mage in _mageList)
            {
                if (mage.Active)
                {
                    result += mage.individualDPS();
                }
            }
            return result;
        }

        public BigIntWithUnit getCurrency()
        {
            return _currency;
        }


		//Can be used for any menu
		public void OpenCloseMenu(Animator anim){
			anim.SetBool ("isDisplayed", !anim.GetBool ("isDisplayed"));
		}

		//Idle Functionality Preparations
		/*
		public void OnApplicationQuit(){
			PlayerPrefs.SetString ("GameCloseTime", System.DateTime.Now.ToString());
		}

		public void CalculateIdleIncome(){
			PlayerPrefs.GetString ("GameCloseTime");
			_totalIdleDamage = MageAttackDuration * MageDPS;

			if (_totalIdleDamage > WaveManager.WaveLife && WaveManager.CurrentWave%5 != 0) {
				WaveManager.CurrentWave++;
				IncreaseCurrenc(Minion.
			}
		}
		*/
    }
}