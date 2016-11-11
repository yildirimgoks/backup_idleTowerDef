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

        public ProfilePictureMage TvMage;

        public Sprite[] PlayerPics;
        
        public bool MageMenuOpen;

		public GameObject SettingsMenu;
		public GameObject AchievementsButton;
		public GameObject ResetAsker;
		public GameObject AchievementsMenu;
		public GameObject AchievementsBackButton;
		public Button[] ResetButtons;

        private Func<BigIntWithUnit> _upgradeMagePriceGetter;
        private Func<string[]> _infoGetter;
		private Func<BigIntWithUnit> _upgradeIdleIncomeGetter;

        private void Awake()
        {
            Instance = this;

            _uiManager = Camera.main.GetComponent<UIManager>();
            _player = Camera.main.GetComponent<Player>();
        }

        private void Start()
        {
            MageMenuOpen = false;
            _openProfilePage = null;
            OpenCloseButton.onClick.AddListener(delegate
            {
                _uiManager.OpenCloseMenu(MageMenu, true);
                if (MageMenuOpen)
                {
                    gameObject.GetComponent<ToggleGroup>().SetAllTogglesOff();
                    _uiManager.DestroyMainMenuCloser();
                }
                else
                {
                    _uiManager.CreateMainMenuCloser(delegate
                    {
                        RaycastHit towerHit;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out towerHit))
                        {
                            if (towerHit.collider.tag != "Tower" && towerHit.collider.tag != "Shrine" && towerHit.collider.tag != "Mage")
                            {
								_uiManager.OpenCloseMenu(MageMenu, true);
                                gameObject.GetComponent<ToggleGroup>().SetAllTogglesOff();
                                MageMenuOpen = !MageMenuOpen;
                                _uiManager.DestroyMainMenuCloser();
                            }
                            if (towerHit.collider.tag == "Tower" || towerHit.collider.tag == "Shrine")
                            {
                                if (towerHit.collider.gameObject.GetComponent<MageAssignableBuilding>().InsideMage == null)
                                {
									_uiManager.OpenCloseMenu(MageMenu, true);
                                    gameObject.GetComponent<ToggleGroup>().SetAllTogglesOff();
                                    MageMenuOpen = !MageMenuOpen;
                                    _uiManager.DestroyMainMenuCloser();
                                }
                            }
                        }
                    });
                }
                MageMenuOpen = !MageMenuOpen;
            });

			_audioManager = _player.AudioManager;

			var SettingsCloser = SettingsMenu.GetComponentInChildren<Button>();
			SettingsCloser.onClick.AddListener(delegate {
				_uiManager.OpenCloseMenu(SettingsMenu,true);
			});

			var AchievementsCloser = AchievementsMenu.GetComponentInChildren<Button> ();
			AchievementsCloser.onClick.AddListener(delegate {
				_uiManager.OpenCloseMenu(AchievementsMenu,true);
			});

			AchievementsBackButton.GetComponent<Button>().onClick.AddListener (delegate {
				_uiManager.OpenCloseMenu(SettingsMenu,true);
				_uiManager.OpenCloseMenu(AchievementsMenu,true);
			});

			AchievementsButton.GetComponent<Button>().onClick.AddListener (delegate {
				_uiManager.OpenCloseMenu(SettingsMenu,true);
				_uiManager.OpenCloseMenu(AchievementsMenu,true);
			});
				
			foreach (var button in ResetAsker.GetComponentsInChildren<Button>()) {
				if (button.name != "Yes") {
					button.onClick.AddListener (delegate {
					_uiManager.OpenCloseMenu(ResetAsker, true);
					});
				}
			}
			foreach (var button in ResetButtons) {
				button.onClick.AddListener (delegate {
					_uiManager.OpenCloseMenu(ResetAsker, true);
					_uiManager.OpenCloseMenu(MageMenu, true);
					_uiManager.DestroyMainMenuCloser();
				});
			}
        }

        private void Update()
        {
            if (_openProfilePage == null) return;
            if (_upgradeMagePriceGetter != null)
            {
                var upgradeMagePrice = _upgradeMagePriceGetter.Invoke();
                var UpgradeButton1 = _openProfilePage.GetComponentsInChildren<Button>()[0];
                UpgradeButton1.GetComponentInChildren<Text>().text = "Level Up (" + upgradeMagePrice + ")";
                UpgradeButton1.interactable = _player.Data.GetCurrency() >= upgradeMagePrice;
            }

			if (_upgradeIdleIncomeGetter != null)
			{
				var upgradeIdleIncome = _upgradeIdleIncomeGetter.Invoke();
				var UpgradeButton2 = _openProfilePage.GetComponentsInChildren<Button>()[1];
				UpgradeButton2.GetComponentInChildren<Text>().text = "Idle Income Level Up (" + upgradeIdleIncome + ")";
				UpgradeButton2.interactable = _player.Data.GetCurrency() >= upgradeIdleIncome;
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

        private void SetPerson(MageData mage, GameObject profilePage)
        {
            foreach (var mageButton in MageButtonsList)
            {
                if (mageButton.GetComponentInChildren<Renderer>())
                {
                    mageButton.GetComponentInChildren<Renderer>().enabled = false;
                }
            }
            _openProfilePage = profilePage;
            _upgradeMagePriceGetter = mage.GetUpgradePrice;
			_upgradeIdleIncomeGetter = null;
            _infoGetter = mage.GetProfileInfo;

            foreach (var rend in TvMage.gameObject.GetComponentsInChildren<Renderer>())
            {
                if (rend.name.Contains("Body"))
                {
                    rend.material.mainTexture = ElementController.Instance.GetMage(mage.GetElement())[0];
                }
                else
                {
                    rend.material.mainTexture = ElementController.Instance.GetMage(mage.GetElement())[1];
                }
            }

            if (profilePage.GetComponentInParent<ToggleGroup>().AnyTogglesOn())
            {
                profilePage.GetComponentInChildren<Renderer>().enabled = true;
            }
        }

        private void SetPerson(GameObject profilePage)
        {
            foreach (var mageButton in MageButtonsList)
            {
                if (mageButton.GetComponentInChildren<Renderer>())
                {
                    mageButton.GetComponentInChildren<Renderer>().enabled = false;
                }
            }
            _openProfilePage = profilePage;
            _upgradeMagePriceGetter = _player.Data.GetUpgradePrice;
			_upgradeIdleIncomeGetter = _player.Data.GetIdleUpgradePrice;
            _infoGetter = _player.Data.GetProfileInfo;

            var number = (int)_player.Data.GetElement() - 1;
            profilePage.transform.FindChild("Pp").GetComponent<Image>().sprite = PlayerPics[number];
        }

        public void AddPlayerButton()
        {
            var mageButton = Instantiate(PlayerButtonPrefab);
            MageButtonsList.Add(mageButton);
            mageButton.transform.SetParent(transform, false);
            mageButton.GetComponent<UIAccordionElement>().SetAccordion();
            mageButton.GetComponentInChildren<Text>().text = _player.Data.GetPlayerName();
            var title = mageButton.gameObject.transform.GetChild(0);
            title.GetChild(1).GetComponent<Image>().color = ElementController.Instance.GetColor(_player.Data.GetElement());
            title.GetChild(2).GetComponent<Image>().sprite = ElementController.Instance.GetIcon(_player.Data.GetElement());
            title.GetChild(3).GetComponent<Image>().sprite = ElementController.Instance.GetIcon(_player.Data.GetElement());
            var profilePage = mageButton.gameObject.transform.GetChild(1);
            profilePage.FindChild("Element Logo").GetComponent<Image>().sprite = ElementController.Instance.GetIcon(_player.Data.GetElement());
            var buttons = profilePage.GetComponentsInChildren<Button>();
            //buttons[0].onClick.AddListener(delegate
            //{
            //    Player.Data.UpgradePlayer();
            //});
			_player.AssignActions();
			for ( var j = 0 ; j < _player.upgrade1Actions.Length ; j++){
				if ( _player.upgrade1Actions[j] == null) break;
				ActionWithEvent action = _player.upgrade1Actions[j];
				EventTrigger trigger = buttons[0].GetComponent<EventTrigger>();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = action.triggerType;
				entry.callback.AddListener(action.function);
				// entry.callback.AddListener(call);
				trigger.triggers.Add(entry);
			}
            //buttons[1].onClick.AddListener(delegate
            //{
            //    Player.Data.UpgradeIdleGenerated();
            //});
			for ( var j = 0 ; j < _player.upgrade2Actions.Length ; j++){
				if ( _player.upgrade2Actions[j] == null) break;
				ActionWithEvent action = _player.upgrade2Actions[j];
				EventTrigger trigger = buttons[1].GetComponent<EventTrigger>();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = action.triggerType;
				entry.callback.AddListener(action.function);
				// entry.callback.AddListener(call);
				trigger.triggers.Add(entry);
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
                	SetPerson(profilePage.gameObject);
                	SetScroll(1);
        	    });
        }

        public void AddMageButton(Mage mage)
        {
            var mageButton = Instantiate(MageButtonPrefab);
            MageButtonsList.Add(mageButton);
            mage.Data.ProfileButtonIndex = MageButtonsList.Count;
            mage.Data.ProfileButton = mageButton;
            OnMagePrefabUpdated(mage);
        }

        public void OnMagePrefabUpdated(Mage mage)
        {
            var mageButton = MageButtonsList[mage.Data.ProfileButtonIndex - 1];
            mageButton.transform.SetParent(transform, false);
            mageButton.GetComponent<UIAccordionElement>().SetAccordion();
            mageButton.GetComponentInChildren<Text>().text = mage.Data.GetName();
            mageButton.GetComponentInChildren<Renderer>().enabled = false;
            var Title = mageButton.gameObject.transform.GetChild(0);
            Title.GetChild(1).GetComponent<Image>().color = ElementController.Instance.GetColor(mage.Data.GetElement());
            Title.GetChild(2).GetComponent<Image>().sprite = ElementController.Instance.GetIcon(mage.Data.GetElement());
            Title.GetChild(3).GetComponent<Image>().sprite = ElementController.Instance.GetIcon(mage.Data.GetElement());
            var ProfilePage = mageButton.gameObject.transform.GetChild(1);
            ProfilePage.FindChild("Element Logo").GetComponent<Image>().sprite = ElementController.Instance.GetIcon(mage.Data.GetElement());
            var Buttons = ProfilePage.GetComponentsInChildren<Button>();
            mage.AssignActions();
            for (var j = 0; j < mage.upgradeActions.Length; j++)
            {
                if (mage.upgradeActions[j] == null) break;
                ActionWithEvent action = mage.upgradeActions[j];
                EventTrigger trigger = Buttons[0].GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = action.triggerType;
                entry.callback.AddListener(action.function);
                // entry.callback.AddListener(call);
                trigger.triggers.Add(entry);
            }
            mageButton.GetComponent<UIAccordionElement>().onValueChanged.AddListener(delegate
            {
                _audioManager.PlayButtonClickSound();
                SetPerson(mage.Data, ProfilePage.gameObject);
                if (mage.GetBuilding())
                {
                    if (mageButton.GetComponent<UIAccordionElement>().isOn)
                    {
                        mage.GetBuilding().StartHighlighting(ElementController.Instance.GetColor(mage.Data.GetElement()));
                    }
                    else
                    {
                        mage.GetBuilding().StopHighlighting();
                    }
                    if (mage.GetBuilding().isHighlightOn)
                    {
                        mage.GetBuilding().DisplayRangeObject();
                    }
                    else
                    {
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
                    else
                    {
                        if (mageButton.GetComponent<UIAccordionElement>().isOn)
                        {
                            BuildingMenuSpawner.INSTANCE.SpawnMenu(mage.GetBuilding());
                        }
                    }
                }
                else
                {
                    if (mageButton.GetComponent<UIAccordionElement>().isOn)
                    {
                        mage.StartHighlighting();
                    }
                    else
                    {
                        mage.StopHighlighting();
                    }
                    // mage.Highlight.enabled = mageButton.GetComponent<UIAccordionElement>().isOn;
                }
                SetScroll(mage.Data.ProfileButtonIndex);
            });
        }

        public void ResetMageMenu()
        {
            foreach (var mageButton in MageButtonsList)
            {
                Destroy(mageButton);
            }
            _openProfilePage = null;
        }
    }
}