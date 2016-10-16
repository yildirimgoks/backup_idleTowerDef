using Assets.Scripts.Manager;
using Assets.Scripts.Model;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class MageButtons : MonoBehaviour
    {
        public static MageButtons Instance;
        public UIManager UIManager;

        public GameObject MageButtonPrefab;
        public GameObject PlayerButtonPrefab;
        public GameObject openProfilePage;

        public Button OpenCloseButton;
        public ScrollRect MageListScroll;
        private Text[] Info;

        public RectTransform Viewport;

        public ProfilePictureMage TVMage;

        public Sprite[] PlayerPics;

        public Player Player;

        public bool MageMenuOpen;

        private int _buttonCount;

		public GameObject SettingsMenu;
		public GameObject ResetAsker;
		public Button[] ResetButtons;

        private Func<BigIntWithUnit> upgradeMagePriceGetter;
        private Func<string[]> infoGetter;
		private Func<BigIntWithUnit> upgradeIdleIncomeGetter;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            MageMenuOpen = false;
            openProfilePage = null;
            OpenCloseButton.onClick.AddListener(delegate
            {
                UIManager.OpenCloseMenu(OpenCloseButton.GetComponentInParent<Animator>().gameObject, true);
                if (MageMenuOpen)
                {
                    gameObject.GetComponent<ToggleGroup>().SetAllTogglesOff();
                    UIManager.DestroyMainMenuCloser();
                }
                else
                {
                    UIManager.CreateMainMenuCloser(delegate
                    {
                        RaycastHit towerHit;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out towerHit))
                        {
                            if (towerHit.collider.tag != "Tower" && towerHit.collider.tag != "Shrine" && towerHit.collider.tag != "Mage")
                            {
								UIManager.OpenCloseMenu(OpenCloseButton.GetComponentInParent<Animator>().gameObject, true);
                                gameObject.GetComponent<ToggleGroup>().SetAllTogglesOff();
                                MageMenuOpen = !MageMenuOpen;
                                UIManager.DestroyMainMenuCloser();
                            }
                            if (towerHit.collider.tag == "Tower" || towerHit.collider.tag == "Shrine")
                            {
                                if (towerHit.collider.gameObject.GetComponent<MageAssignableBuilding>().InsideMage == null)
                                {
									UIManager.OpenCloseMenu(OpenCloseButton.GetComponentInParent<Animator>().gameObject, true);
                                    gameObject.GetComponent<ToggleGroup>().SetAllTogglesOff();
                                    MageMenuOpen = !MageMenuOpen;
                                    UIManager.DestroyMainMenuCloser();
                                }
                            }
                        }
                    });
                }
                MageMenuOpen = !MageMenuOpen;
            });

			var SettingsCloser = SettingsMenu.GetComponentInChildren<Button>();
			SettingsCloser.onClick.AddListener(delegate {
				UIManager.OpenCloseMenu(SettingsMenu,true);
			});

			foreach (var button in ResetAsker.GetComponentsInChildren<Button>()) {
				if (button.name != "Yes") {
					button.onClick.AddListener (delegate {
					UIManager.OpenCloseMenu(ResetAsker, true);
					});
				}
			}
			foreach (var button in ResetButtons) {
				button.onClick.AddListener (delegate {
					UIManager.OpenCloseMenu(ResetAsker, true);
					UIManager.OpenCloseMenu(OpenCloseButton.GetComponentInParent<Animator>().gameObject, true);
					UIManager.DestroyMainMenuCloser();
				});
			}
        }

        private void Update()
        {
            if (openProfilePage == null) return;
            if (upgradeMagePriceGetter != null)
            {
                var upgradeMagePrice = upgradeMagePriceGetter.Invoke();
                var UpgradeButton1 = openProfilePage.GetComponentsInChildren<Button>()[0];
                UpgradeButton1.GetComponentInChildren<Text>().text = "Level Up (" + upgradeMagePrice + ")";
                UpgradeButton1.interactable = Player.Data.GetCurrency() >= upgradeMagePrice;
            }

			if (upgradeIdleIncomeGetter != null)
			{
				var upgradeIdleIncome = upgradeIdleIncomeGetter.Invoke();
				var UpgradeButton2 = openProfilePage.GetComponentsInChildren<Button>()[1];
				UpgradeButton2.GetComponentInChildren<Text>().text = "Idle Income Level Up (" + upgradeIdleIncome + ")";
				UpgradeButton2.interactable = Player.Data.GetCurrency() >= upgradeIdleIncome;
			}

            var currentInfo = infoGetter.Invoke();
            Info = openProfilePage.GetComponentsInChildren<Text>();
            Info[0].text = currentInfo[0] + "\n" + "Level " + currentInfo[1] + " " + currentInfo[2] + " Mage";
            Info[1].text = "'" + currentInfo[3] + "'";
            Info[2].text = "Damage: " + currentInfo[4] + "\n" + "Rate: " + currentInfo[5] + "\n" + "Range: " + currentInfo[6];
        }

        public void SetScroll(int buttonIndex)
        {
            var nameHeight = MageButtonPrefab.GetComponentsInChildren<LayoutElement>()[1].preferredHeight;
            var profileHeight = MageButtonPrefab.GetComponentsInChildren<LayoutElement>()[2].preferredHeight;
            var spacing = gameObject.GetComponent<VerticalLayoutGroup>().spacing;
            var totalHeight = _buttonCount * nameHeight + profileHeight + (_buttonCount + 1) * spacing;
            var viewportHeight = Viewport.rect.height;
            var diff = totalHeight - viewportHeight;
            var above = buttonIndex * spacing + (buttonIndex - 1) * nameHeight;
            MageListScroll.verticalNormalizedPosition = (diff - above) / diff;
        }

        private void SetPerson(MageData mage, GameObject profilePage)
        {
            for (int i = 0; i < _buttonCount; i++)
            {
                var mageButton = gameObject.transform.GetChild(i);
                if (mageButton.GetComponentInChildren<Renderer>())
                {
                    mageButton.GetComponentInChildren<Renderer>().enabled = false;
                }
            }
            openProfilePage = profilePage;
            upgradeMagePriceGetter = mage.GetUpgradePrice;
			upgradeIdleIncomeGetter = null;
            infoGetter = mage.GetProfileInfo;

            foreach (var rend in TVMage.gameObject.GetComponentsInChildren<Renderer>())
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

        private void SetPerson(GameObject _profilePage)
        {
            for (var i = 0; i < _buttonCount; i++)
            {
                var mageButton = gameObject.transform.GetChild(i);
                if (mageButton.GetComponentInChildren<Renderer>())
                {
                    mageButton.GetComponentInChildren<Renderer>().enabled = false;
                }
            }
            openProfilePage = _profilePage;
            upgradeMagePriceGetter = Player.Data.GetUpgradePrice;
			upgradeIdleIncomeGetter = Player.Data.GetIdleUpgradePrice;
            infoGetter = Player.Data.GetProfileInfo;

            var number = (int)Player.Data.GetElement() - 1;
            _profilePage.transform.FindChild("Pp").GetComponent<Image>().sprite = PlayerPics[number];
        }

        public void AddPlayerButton()
        {
            var mageButton = Instantiate(PlayerButtonPrefab);
            _buttonCount = 1;
            mageButton.transform.SetParent(transform, false);
            mageButton.GetComponent<UIAccordionElement>().SetAccordion();
            mageButton.GetComponentInChildren<Text>().text = Player.Data.GetPlayerName();
            var title = mageButton.gameObject.transform.GetChild(0);
            title.GetChild(1).GetComponent<Image>().color = ElementController.Instance.GetColor(Player.Data.GetElement());
            title.GetChild(2).GetComponent<Image>().sprite = ElementController.Instance.GetIcon(Player.Data.GetElement());
            title.GetChild(3).GetComponent<Image>().sprite = ElementController.Instance.GetIcon(Player.Data.GetElement());
            var profilePage = mageButton.gameObject.transform.GetChild(1);
            profilePage.FindChild("Element Logo").GetComponent<Image>().sprite = ElementController.Instance.GetIcon(Player.Data.GetElement());
            var buttons = profilePage.GetComponentsInChildren<Button>();
            //buttons[0].onClick.AddListener(delegate
            //{
            //    Player.Data.UpgradePlayer();
            //});
			Player.AssignActions();
			for ( var j = 0 ; j < Player.upgrade1Actions.Length ; j++){
				if ( Player.upgrade1Actions[j] == null) break;
				ActionWithEvent action = Player.upgrade1Actions[j];
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
			for ( var j = 0 ; j < Player.upgrade2Actions.Length ; j++){
				if ( Player.upgrade2Actions[j] == null) break;
				ActionWithEvent action = Player.upgrade2Actions[j];
				EventTrigger trigger = buttons[1].GetComponent<EventTrigger>();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = action.triggerType;
				entry.callback.AddListener(action.function);
				// entry.callback.AddListener(call);
				trigger.triggers.Add(entry);
			}
			buttons [2].onClick.AddListener (delegate 
			{
				UIManager.OpenCloseMenu(SettingsMenu,true);
			});
			buttons[3].onClick.AddListener(delegate
            {
				UIManager.OpenCloseMenu(ResetAsker,true);
            });

            mageButton.GetComponent<UIAccordionElement>().onValueChanged.AddListener(delegate
            {
                SetPerson(profilePage.gameObject);
                SetScroll(1);
            });
        }

        public void AddMageButton(Mage mage)
        {
            var mageButton = Instantiate(MageButtonPrefab);
            _buttonCount++;
            mage.ProfileButtonIndex = _buttonCount;
            mage.ProfileButton = mageButton;
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
			for ( var j = 0 ; j < mage.upgradeActions.Length ; j++){
				if ( mage.upgradeActions[j] == null) break;
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
                SetPerson(mage.Data, ProfilePage.gameObject);
                if (mage.GetBuilding())
                {
                    mage.GetBuilding().Highlight.enabled = mageButton.GetComponent<UIAccordionElement>().isOn;
                    if (mage.GetBuilding().MenuOpen)
                    {
                        mage.GetBuilding().MenuOpen = mageButton.GetComponent<UIAccordionElement>().isOn;
                        if (MageMenuOpen)
                        {
                            UIManager.DestroyTowerMenuCloser();
                        }
                    }
                }
                else
                {
                    mage.Highlight.enabled = mageButton.GetComponent<UIAccordionElement>().isOn;
                }
                SetScroll(mage.ProfileButtonIndex);
            });
        }

        public void ResetMageMenu()
        {
            var Buttons = GameObject.FindGameObjectsWithTag("MageButton");
            foreach (var mageButton in Buttons)
            {
                Destroy(mageButton);
            }
            _buttonCount = 0;
            openProfilePage = null;
        }
    }
}