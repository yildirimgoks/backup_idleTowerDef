using System;
using System.Collections.Generic;
using Assets.Scripts.Manager;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class MageButtons : MonoBehaviour
    {
        public static MageButtons Instance;

        private UIManager _uiManager;
        private Player _player;
        private AudioManager _audioManager;

        public List<GameObject> MageButtonsList = new List<GameObject>();
        public GameObject MageButtonPrefab;
        public GameObject PlayerButtonPrefab;
        private GameObject _openProfilePage;

        public Button OpenCloseButton;
		public GameObject MageMenu;
        public ScrollRect MageListScroll;
        private Text[] _info;

        public RectTransform Viewport;

        public TelevoleManager TvMageManager;

        public Sprite[] PlayerPics;
        
        public bool MageMenuOpen;

		public GameObject SettingsMenu;
		public Button AchievementsButton;
		public GameObject ResetAsker;
		public GameObject AchievementsMenu;
		public GameObject AchievementsBackButton;
		public Button[] ResetButtons;
		public GameObject AdAsker;
		public Button AdMenuButton;

        private Func<BigIntWithUnit> _upgradeMagePriceGetter;
        private Func<string[]> _infoGetter;
		private Func<BigIntWithUnit> _upgradeIdleIncomeGetter;

        private void Awake()
        {
            Instance = this;

            _uiManager = Camera.main.GetComponent<UIManager>();
            _player = Camera.main.GetComponent<Player>();
            _audioManager = _player.AudioManager;
        }

        private void Start()
        {
            MageMenuOpen = false;
            _openProfilePage = null;
            OpenCloseButton.onClick.AddListener(delegate
            {
                MageMenuOpen = !MageMenuOpen;
                _uiManager.OpenCloseMenu(MageMenu, MageMenuOpen);
                if (!MageMenuOpen)
                {
                    OnMageButtonsMenuClosed();
                }
                else
                {
                    _uiManager.CreateMainMenuCloser(delegate
                    {
                        RaycastHit towerHit;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out towerHit))
                        {
                            if (towerHit.collider.tag != "Tower" && towerHit.collider.tag != "Shrine" && towerHit.collider.tag != "Mage"
                            || ((towerHit.collider.tag == "Tower" || towerHit.collider.tag == "Shrine") && towerHit.collider.gameObject.GetComponent<MageAssignableBuilding>().InsideMage == null))
                            {
                                _uiManager.OpenCloseMenu(MageMenu, false);
                                OnMageButtonsMenuClosed();
                            }
                        }
                    });
                }
            });
            
			var SettingsCloser = SettingsMenu.GetComponentInChildren<Button>();
			SettingsCloser.onClick.AddListener(delegate {
				_uiManager.OpenCloseMenu(SettingsMenu,false);
			});

			var AchievementsCloser = AchievementsMenu.GetComponentInChildren<Button> ();
			AchievementsCloser.onClick.AddListener(delegate {
				_uiManager.OpenCloseMenu(AchievementsMenu,false);
			});

			AchievementsBackButton.GetComponent<Button>().onClick.AddListener (delegate {
				_uiManager.OpenCloseMenu(SettingsMenu, true);
				_uiManager.OpenCloseMenu(AchievementsMenu, false);
			});

			AchievementsButton.onClick.AddListener (delegate {
				_uiManager.OpenCloseMenu(SettingsMenu,false);
				_uiManager.OpenCloseMenu(AchievementsMenu,true);
			});
				
			foreach (var button in ResetAsker.GetComponentsInChildren<Button>()) {
				if (button.name != "Yes") {
					button.onClick.AddListener (delegate {
					_uiManager.OpenCloseMenu(ResetAsker, false);
					});
				}
			}

			AdMenuButton.onClick.AddListener (delegate {
				_uiManager.OpenCloseMenu(SettingsMenu,false);
				_uiManager.OpenCloseMenu(AdAsker,true);
			});

			foreach (var button in AdAsker.GetComponentsInChildren<Button>()) {
				button.onClick.AddListener (delegate {
					_uiManager.OpenCloseMenu(AdAsker, false);
				});
			}

			foreach (var button in ResetButtons) {
				button.onClick.AddListener (delegate {
					_uiManager.OpenCloseMenu(ResetAsker, false);
					_uiManager.OpenCloseMenu(MageMenu, false);
					_uiManager.DestroyMainMenuCloser();
				});
			}
        }

        private void OnMageButtonsMenuClosed()
        {
            gameObject.GetComponent<ToggleGroup>().SetAllTogglesOff();
            MageMenuOpen = false;
            _uiManager.DestroyMainMenuCloser();
        }

        private void Update()
        {
            if (_openProfilePage == null) return;
            if (_upgradeMagePriceGetter != null)
            {
                var upgradeMagePrice = _upgradeMagePriceGetter.Invoke();
                var upgradeButton1 = _openProfilePage.GetComponentsInChildren<Button>()[0];
                upgradeButton1.GetComponentInChildren<Text>().text = "Level Up (" + upgradeMagePrice + ")";
                upgradeButton1.interactable = _player.Data.GetCurrency() >= upgradeMagePrice;
            }

			if (_upgradeIdleIncomeGetter != null)
			{
				var upgradeIdleIncome = _upgradeIdleIncomeGetter.Invoke();
				var upgradeButton2 = _openProfilePage.GetComponentsInChildren<Button>()[1];
				upgradeButton2.GetComponentInChildren<Text>().text = "Idle Income Level Up (" + upgradeIdleIncome + ")";
				upgradeButton2.interactable = _player.Data.GetCurrency() >= upgradeIdleIncome;
			}

            var currentInfo = _infoGetter.Invoke();
            _info = _openProfilePage.GetComponentsInChildren<Text>();
            _info[0].text = currentInfo[0] + "\n" + "Level " + currentInfo[1] + " " + currentInfo[2] + " Mage";
            _info[1].text = "'" + currentInfo[3] + "'";
            _info[2].text = "Damage: " + currentInfo[4] + "\n" + "Rate: " + currentInfo[5] + "\n" + "Range: " + currentInfo[6];
        }

        public void SetScroll(int buttonIndex)
        {
            var nameHeight = MageButtonPrefab.GetComponentsInChildren<LayoutElement>()[1].preferredHeight;
            var profileHeight = MageButtonPrefab.GetComponentsInChildren<LayoutElement>()[2].preferredHeight;
            var spacing = gameObject.GetComponent<VerticalLayoutGroup>().spacing;
            var totalHeight = MageButtonsList.Count * nameHeight + profileHeight + (MageButtonsList.Count + 1) * spacing;
            var viewportHeight = Viewport.rect.height;
            var diff = totalHeight - viewportHeight;
            var above = buttonIndex * spacing + (buttonIndex - 1) * nameHeight;
            MageListScroll.verticalNormalizedPosition = (diff - above) / diff;
        }

        private void SetPerson(GameObject profilePage, MageData mage)
        {
            foreach (var mageButton in MageButtonsList)
            {
                if (mageButton.GetComponentInChildren<Renderer>())
                {
                    mageButton.GetComponentInChildren<Renderer>().enabled = false;
                }
            }
            _openProfilePage = profilePage;
            if (mage != null)
            {
                //Mage Button
                _upgradeMagePriceGetter = mage.GetUpgradePrice;
                _upgradeIdleIncomeGetter = null;
                _infoGetter = mage.GetProfileInfo;
                TvMageManager.SetMage(mage);

                if (profilePage.GetComponentInParent<ToggleGroup>().AnyTogglesOn())
                {
                    profilePage.GetComponentInChildren<Renderer>().enabled = true;
                }
            }
            else
            {
                //Player Button
                _upgradeMagePriceGetter = _player.Data.GetUpgradePrice;
                _upgradeIdleIncomeGetter = _player.Data.GetIdleUpgradePrice;
                _infoGetter = _player.Data.GetProfileInfo;

                var number = (int) _player.Data.GetElement() - 1;
                profilePage.transform.FindChild("Pp").GetComponent<Image>().sprite = PlayerPics[number];
            }
        }

        public void AddPlayerButton()
        {
            var mageButton = Instantiate(PlayerButtonPrefab);
            MageButtonsList.Add(mageButton);
            mageButton.transform.SetParent(transform, false);
            mageButton.GetComponent<UIAccordionElement>().SetAccordion();
            mageButton.GetComponentInChildren<Text>().text = _player.Data.GetPlayerName();
            SetButtonElement(mageButton, _player.Data.GetElement());
            var profilePage = mageButton.gameObject.transform.GetChild(1);
            var buttons = profilePage.GetComponentsInChildren<Button>();
            //buttons[0].onClick.AddListener(delegate
            //{
            //    Player.Data.UpgradePlayer();
            //});
			_player.AssignActions();
			foreach (var t in _player.upgrade1Actions)
			{
			    UIManager.SetButtonEvent(buttons[0], t);
			}
            //buttons[1].onClick.AddListener(delegate
            //{
            //    Player.Data.UpgradeIdleGenerated();
            //});
            foreach (var t in _player.upgrade2Actions)
            {
                UIManager.SetButtonEvent(buttons[1], t);
            }
			buttons [2].onClick.AddListener (delegate 
			{
				_uiManager.OpenCloseMenu(SettingsMenu,true);
			});
			buttons[3].onClick.AddListener(delegate
            {
				_uiManager.OpenCloseMenu(ResetAsker,true);
            });

            mageButton.GetComponent<UIAccordionElement>().onValueChanged.AddListener(delegate 
            {					
				_audioManager.PlayButtonClickSound();
               	SetPerson(profilePage.gameObject, null);
               	SetScroll(1);
        	});
        }
        
        public void AddMageButton(Mage mage)
        {
            var mageButton = Instantiate(MageButtonPrefab);
            MageButtonsList.Add(mageButton);
            mage.Data.ProfileButtonIndex = MageButtonsList.Count;
            mage.Data.ProfileButton = mageButton;

            mageButton.transform.SetParent(transform, false);
            mageButton.GetComponent<UIAccordionElement>().SetAccordion();
            mageButton.GetComponentInChildren<Renderer>().enabled = false;
            
            OnMagePrefabUpdated(mage);
        }

        public void OnMagePrefabUpdated(Mage mage)
        {
            var mageButton = MageButtonsList[mage.Data.ProfileButtonIndex - 1];
            
            mageButton.GetComponentInChildren<Text>().text = mage.Data.GetName();
            SetButtonElement(mageButton, mage.Data.GetElement());
            var profilePage = mageButton.gameObject.transform.GetChild(1);
            var buttons = profilePage.GetComponentsInChildren<Button>();
            mage.AssignActions();
            
            foreach (var t in mage.upgradeActions)
            {
                UIManager.SetButtonEvent(buttons[0], t);
            }
            
            mageButton.GetComponent<UIAccordionElement>().onValueChanged.AddListener(delegate
            {
                _audioManager.PlayButtonClickSound();
                SetPerson(profilePage.gameObject, mage.Data);
                if (mage.GetBuilding())
                {
                    if (mageButton.GetComponent<UIAccordionElement>().isOn)
                    {
                        // mage.GetBuilding().StartHighlighting(ElementController.Instance.GetColor(mage.Data.GetElement()));
                        mage.GetBuilding().StartHighlighting();
                        mage.GetBuilding().DisplayRangeObject();
                    }
                    else
                    {
                        mage.GetBuilding().StopHighlighting();
                        mage.GetBuilding().HideRangeObject();
                    }

                    if (mage.GetBuilding().MenuOpen)
                    {
                        mage.GetBuilding().MenuOpen = mageButton.GetComponent<UIAccordionElement>().isOn;
                        if (MageMenuOpen)
                        {
                            _uiManager.DestroyTowerMenuCloser();
                        }
                    }
                    else if (mageButton.GetComponent<UIAccordionElement>().isOn)
                    {
                        BuildingMenuSpawner.INSTANCE.SpawnMenu(mage.GetBuilding());
                    }
                }
                else
                {
                    mage.SetHightlighActive(mageButton.GetComponent<UIAccordionElement>().isOn);
                }
                SetScroll(mage.Data.ProfileButtonIndex);
            });
        }

        private static void SetButtonElement(GameObject mageButton, Element element)
        {
            var title = mageButton.gameObject.transform.GetChild(0);
            title.GetChild(1).GetComponent<Image>().color = ElementController.Instance.GetColor(element);
            title.GetChild(2).GetComponent<Image>().sprite = ElementController.Instance.GetIcon(element);
            title.GetChild(3).GetComponent<Image>().sprite = ElementController.Instance.GetIcon(element);
            var profilePage = mageButton.gameObject.transform.GetChild(1);
            profilePage.FindChild("Element Logo").GetComponent<Image>().sprite = ElementController.Instance.GetIcon(element);
        }

        public void ResetMageMenu()
        {
            foreach (var mageButton in MageButtonsList)
            {
                Destroy(mageButton.gameObject);
            }
            MageButtonsList.Clear();
            _openProfilePage = null;
        }
    }
}