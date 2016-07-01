using System.Collections.Generic;
using Assets.Scripts.Model;
using Assets.Scripts.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public Mage MagePrefab;
        public PlayerSpell PlayerSpellPrefab;

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

		public Texture[] TowerTextures;
        private MageFactory _mageFactory;
        public PlayerData Data;

        private List<Mage> _mage;

        // Use this for initialization
        private void Start()
        {
            Data = new PlayerData(20, 100, 0, new List<MageData>(), 100, 1, Element.Air);
            
			WaveManager.SendWave();
			MageButtons.Ins.AddPlayerButton();
            
            _mageFactory = new MageFactory(MagePrefab);
			ElementController.Instance.textures = TowerTextures;

            _mage = new List<Mage>();
            for (int i = 0; i < 3; i++)
            {
                _mage.Add(_mageFactory.GetMage(6.1f, 13 + 4 * i));
            }
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
                    Spell.Clone(PlayerSpellPrefab, Data.GetSpellData(), instantPos,
                            WaveManager.FindClosestMinion(instantPos));
                }
            }

            //1M Currency Cheat
            if (Input.GetKeyDown(KeyCode.M))
            {
                Data.IncreaseCurrency(1000000);
            }

            // Kill wave cheat
            if (Input.GetKeyDown(KeyCode.K))
            {
                foreach (var minion in WaveManager.GetMinionList())
                {
                    minion.Life = 0;
                }
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
            if (!WaveManager.SafeRemove(minion)) return;
            Data.IncreaseCurrency(currencyGivenOnDeath);

            if (minion.tag != "Boss") return;
            //Boss drops a new mage
            var newMage = _mageFactory.GetMage(minion.transform.position.x, minion.transform.position.z);
            if (newMage == null) return;
            Data.AddMage(newMage.Data);
            MageButtons.Ins.AddMageButton(newMage);
            Time.timeScale = 0;
        }

        private void UpdateLabels()
        {
            CurrText.text = "Currency:\n" + Data.GetCurrency();
            WaveText.text = "Wave:\n" + (WaveManager.CurrentWave + 1);
			WaveLifeText.text = "Wave Life:\n" + WaveManager.WaveLife;
			WaveLifeBar.value = 1 / WaveManager.TotalWaveLife.Divide(WaveManager.WaveLife);
			MageText.text = "Damage:\n" + Data.CumulativeDps();
            IncomeText.text = "Income:\n";
            var currentWaveBlock = WaveManager.CurrentWave / 5*5;
            Wave1.GetComponentInChildren<Text>().text ="" + (currentWaveBlock + 1);
			Wave2.GetComponentInChildren<Text>().text ="" + (currentWaveBlock + 2);
			Wave3.GetComponentInChildren<Text>().text ="" + (currentWaveBlock + 3);
			Wave4.GetComponentInChildren<Text>().text ="" + (currentWaveBlock + 4);
			Wave5.GetComponentInChildren<Text>().text ="" + (currentWaveBlock + 5);
			ColorBlock cb;
			switch (WaveManager.CurrentWave % 5) {
			case 1: cb = Wave2.colors; cb.disabledColor=Color.yellow; Wave2.colors = cb; break;
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

        //Can be used for any menu
		public void OpenCloseMenu(GameObject menu, bool open)
        {
            var anim = menu.GetComponent<Animator>();
			anim.SetBool("isDisplayed", !anim.GetBool("isDisplayed") && open);
        }
    }
}