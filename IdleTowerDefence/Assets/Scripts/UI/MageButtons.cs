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
        public Player Player;

        public List<GameObject> MageButtonsList = new List<GameObject>();
        public GameObject MageButtonPrefab;
        public GameObject PlayerButtonPrefab;
        private GameObject _openProfilePage;

        public Button OpenCloseButton;
        public GameObject MageMenu;
        public ScrollRect MageListScroll;
        private Text[] _info;

        public RectTransform Viewport;

        public Sprite[] PlayerPics;

        public bool MageMenuOpen;

        public GameObject SettingsMenu;
        public Button AchievementsButton;
        public GameObject ResetAsker;
        public GameObject AchievementsMenu;
        public GameObject AchievementsBackButton;
        public Button[] ResetButtons;
        public GameObject AdAsker;
        public Text resetAskterText;

        public Button AdMenuButton;
        public Button ResetMenuButton;
        public Button SettingsMenuButton;
        public Button TutorialButton;

        private Func<BigIntWithUnit> _upgradeMagePriceGetter;
        private Func<string[]> _infoGetter;
        private Func<BigIntWithUnit> _upgradeIdleIncomeGetter;

        public void OnFirstSceneLoaded()
        {
            MageMenuOpen = false;
            _openProfilePage = null;
            OpenCloseButton.onClick.AddListener(delegate
            {
                MageMenuOpen = !MageMenuOpen;
                Player.UIManager.OpenCloseMenu(MageMenu, MageMenuOpen);
                if (!MageMenuOpen)
                {
                    OnMageButtonsMenuClosed();
                }
                else
                {
                    Player.UIManager.CreateMainMenuCloser(delegate
                    {
                        RaycastHit towerHit;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out towerHit))
                        {
                            if (towerHit.collider.tag != "Tower" && towerHit.collider.tag != "Shrine" && towerHit.collider.tag != "Mage"
                            || ((towerHit.collider.tag == "Tower" || towerHit.collider.tag == "Shrine") && towerHit.collider.gameObject.GetComponent<MageAssignableBuilding>().InsideMage == null))
                            {
                                Player.UIManager.OpenCloseMenu(MageMenu, false);
                                OnMageButtonsMenuClosed();
                            }
                        }
                    });
                }
            });

            var SettingsCloser = SettingsMenu.GetComponentInChildren<Button>();
            SettingsCloser.onClick.AddListener(delegate {
                Player.UIManager.OpenCloseMenu(SettingsMenu, false);
            });

            var AchievementsCloser = AchievementsMenu.GetComponentInChildren<Button>();
            AchievementsCloser.onClick.AddListener(delegate {
                Player.UIManager.OpenCloseMenu(AchievementsMenu, false);
            });

            AchievementsBackButton.GetComponent<Button>().onClick.AddListener(delegate {
                Player.UIManager.OpenCloseMenu(SettingsMenu, true);
                Player.UIManager.OpenCloseMenu(AchievementsMenu, false);
            });

            AchievementsButton.onClick.AddListener(delegate {
                Player.UIManager.OpenCloseMenu(SettingsMenu, false);
                Player.UIManager.OpenCloseMenu(AchievementsMenu, true);
            });

            foreach (var button in ResetAsker.GetComponentsInChildren<Button>()) {
                if (button.name != "Yes") {
                    button.onClick.AddListener(delegate {
                        Player.UIManager.OpenCloseMenu(ResetAsker, false);
                    });
                }
            }

            SettingsMenuButton.onClick.AddListener(delegate
               {
                   Player.UIManager.OpenCloseMenu(SettingsMenu, true);
               });

            TutorialButton.onClick.AddListener(delegate
                {
                    Player.UIManager.DirectlyOpenCloseMenu(SettingsMenu, false);
                });

            ResetMenuButton.interactable = Player.Data.GetWaveData().CanReset();
            ResetMenuButton.onClick.AddListener(delegate
                {
                    resetAskterText.text = "Resetting lets you choose your element again. You will also earn " + Player.Data.GetWaveData().GetAccumulativeResetBonus()*100 + "% damage bonus if you reset now.";
                    Player.UIManager.OpenCloseMenu(ResetAsker, true);
                });

            AdMenuButton.onClick.AddListener(delegate {
                Player.UIManager.OpenCloseMenu(SettingsMenu, false);
                Player.UIManager.OpenCloseMenu(AdAsker, true);
            });

            foreach (var button in AdAsker.GetComponentsInChildren<Button>()) {
                button.onClick.AddListener(delegate {
                    Player.UIManager.OpenCloseMenu(AdAsker, false);
                });
            }

            foreach (var button in ResetButtons) {
                button.onClick.AddListener(delegate {
                    Player.UIManager.OpenCloseMenu(ResetAsker, false);
                    Player.UIManager.OpenCloseMenu(MageMenu, false);
                    OnMageButtonsMenuClosed();
                });
            }
        }

        private void OnMageButtonsMenuClosed()
        {
            gameObject.GetComponent<ToggleGroup>().SetAllTogglesOff();
            MageMenuOpen = false;
            Player.UIManager.DestroyMainMenuCloser();
        }
        public void CloseAllMenus()
        {
            Player.UIManager.OpenCloseMenu(ResetAsker, false);
            Player.UIManager.OpenCloseMenu(MageMenu, false);
            Player.UIManager.OpenCloseMenu(SettingsMenu, false);
            Player.UIManager.OpenCloseMenu(AdAsker, false);
            Player.UIManager.OpenCloseMenu(AchievementsMenu, false);
        }
        public void CloseMageButtonsMenu()
		{
			if (MageMenuOpen) {
				Player.UIManager.OpenCloseMenu (MageMenu, false);
				OnMageButtonsMenuClosed ();
			}
		}

		public void DirectlyCloseMageButtonsMenu()
		{
			if (MageMenuOpen) {
				Player.UIManager.DirectlyOpenCloseMenu (MageMenu, false);
				OnMageButtonsMenuClosed ();
			}
		}

        private void Update()
		{
			bool playerCanReset = Player.Data.GetWaveData().CanReset();
			if (!ResetMenuButton.interactable && playerCanReset)
            {
                ResetMenuButton.interactable = true;
			} else if (ResetMenuButton.interactable && !playerCanReset)
			{
				ResetMenuButton.interactable = false;
			}

            if (_openProfilePage == null)
				return;
			if (_upgradeMagePriceGetter != null) {
				var upgradeMagePrice = _upgradeMagePriceGetter.Invoke ();
				var upgradeButton1 = _openProfilePage.GetComponentsInChildren<Button> () [0];
				upgradeButton1.GetComponentInChildren<Text> ().text = "Level Up (" + upgradeMagePrice + ")";
				upgradeButton1.interactable = Player.Data.GetCurrency () >= upgradeMagePrice;
			}

			if (_upgradeIdleIncomeGetter != null) {
				var upgradeIdleIncome = _upgradeIdleIncomeGetter.Invoke ();
				var upgradeButton2 = _openProfilePage.GetComponentsInChildren<Button> () [1];
				upgradeButton2.GetComponentInChildren<Text> ().text = "Idle Income Level Up (" + upgradeIdleIncome + ")";
				upgradeButton2.interactable = Player.Data.GetCurrency () >= upgradeIdleIncome;
			}

			var currentInfo = _infoGetter.Invoke ();
			_info = _openProfilePage.GetComponentsInChildren<Text> ();
			_info [0].text = currentInfo [0] + "\n" + "Level " + currentInfo [1] + " " + currentInfo [2] + " Mage";
			_info [1].text = "'" + currentInfo [3] + "'";
			_info [2].text = "Damage: " + currentInfo [4] + "\n" + "Rate: " + currentInfo [5] + "\n" + "Range: " + currentInfo [6];

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
                Player.TelevoleManager.SetMage(mage);

                if (profilePage.GetComponentInParent<ToggleGroup>().AnyTogglesOn())
                {
                    profilePage.GetComponentInChildren<Renderer>().enabled = true;
                }
            }
            else
            {
                //Player Button
                _upgradeMagePriceGetter = Player.Data.GetUpgradePrice;
                _upgradeIdleIncomeGetter = Player.Data.GetIdleUpgradePrice;
                _infoGetter = Player.Data.GetProfileInfo;

                var number = (int) Player.Data.GetElement() - 1;
                profilePage.transform.Find("Pp").GetComponent<Image>().sprite = PlayerPics[number];
            }
        }

        public void AddPlayerButton()
        {
            var mageButton = Instantiate(PlayerButtonPrefab);
            MageButtonsList.Add(mageButton);
            mageButton.transform.SetParent(transform, false);
            mageButton.GetComponent<UIAccordionElement>().SetAccordion();
            mageButton.GetComponentInChildren<Text>().text = Player.Data.GetPlayerName();
            SetButtonElement(mageButton, Player.Data.GetElement());
            var profilePage = mageButton.gameObject.transform.GetChild(1);
            var buttons = profilePage.GetComponentsInChildren<Button>();
            //buttons[0].onClick.AddListener(delegate
            //{
            //    Player.Data.UpgradePlayer();
            //});
			Player.AssignActions();
			foreach (var t in Player.upgrade1Actions)
			{
			    UIManager.SetButtonEvent(buttons[0], t);
			}
            //buttons[1].onClick.AddListener(delegate
            //{
            //    Player.Data.UpgradeIdleGenerated();
            //});
            foreach (var t in Player.upgrade2Actions)
            {
                UIManager.SetButtonEvent(buttons[1], t);
            }


            mageButton.GetComponent<UIAccordionElement>().onValueChanged.AddListener(delegate 
            {
                Player._audioManager.PlayButtonClickSound();
               	SetPerson(profilePage.gameObject, null);
               	SetScroll(1);
        	});
        }

        public void RemoveMageButtons()
        {
            //player update button is also a mage button and is in place one
            if (MageButtonsList.Count > 1)
            {
                for (int i = 1; i < MageButtonsList.Count; i++)
                {
                    MageButtonsList[i].gameObject.SetActive(false);
                }

                MageButtonsList.RemoveRange(1, MageButtonsList.Count - 2);
            }
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

            var uiAccordionElement = mageButton.GetComponent<UIAccordionElement>();
            if (uiAccordionElement.isOn)
            {
                Player.TelevoleManager.SetMage(mage.Data);
            }

            uiAccordionElement.onValueChanged.AddListener(delegate
            {
                Player._audioManager.PlayButtonClickSound();
                SetPerson(profilePage.gameObject, mage.Data);
                if (mage.GetBuilding())
                {
                    if (uiAccordionElement.isOn)
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
                        mage.GetBuilding().MenuOpen = uiAccordionElement.isOn;
                        if (MageMenuOpen)
                        {
                            Player.UIManager.DestroyTowerMenuCloser();
                        }
                    }
                    else if (uiAccordionElement.isOn)
                    {
                        Player.BuildingMenuSpawner.SpawnMenu(mage.GetBuilding());
                    }
                }
                else
                {
                    mage.SetHightlighActive(uiAccordionElement.isOn);
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
            profilePage.Find("Element Logo").GetComponent<Image>().sprite = ElementController.Instance.GetIcon(element);
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