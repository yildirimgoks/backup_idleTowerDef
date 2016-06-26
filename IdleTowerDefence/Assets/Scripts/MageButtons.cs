using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts
{
	public class MageButtons : MonoBehaviour {

		public static MageButtons Ins;

        public Button MageButtonPrefab;
        public RectTransform MageUpgradePanel;
		public GameObject ProPage;

	    public Player Player;

        private List<Button> _buttonList = new List<Button>();

		public bool UpgradesOpen;

		private void Awake() {
			Ins = this;
		}

		private void Start() {
			UpgradesOpen = false;
		}
        
		public void OpenCloseUpgrades(GameObject Upgrades) {
            Upgrades.SetActive(!Upgrades.activeInHierarchy);
		}

		public void UpdatePrices(Mage mage, GameObject Upgrades) {
			var upgradeButtons = Upgrades.GetComponentsInChildren<Button>();
			upgradeButtons[0].GetComponentInChildren<Text>().text="Upgrade Mage Damage (" + mage._damagePrice + ")";
			upgradeButtons[1].GetComponentInChildren<Text>().text="Upgrade Mage Range (" + mage._rangePrice + ")";
			upgradeButtons[2].GetComponentInChildren<Text>().text="Upgrade Mage Fire Rate (" + mage._ratePrice + ")";
		}

		public void HideOtherButtons(int buttonNumber) {
			if (!UpgradesOpen) {
				for (int i = 0; i < _buttonList.Count; i++) {
					if (i != buttonNumber) {
						_buttonList [i].interactable = false;
						_buttonList [i].gameObject.GetComponent<RectTransform>().localScale = new Vector3 (0, 0, 0);
						UpgradesOpen = true;
					}
				}
			} else {
				for (int i =0; i < _buttonList.Count; i++) {
					if (i != buttonNumber) {
						_buttonList [i].interactable = true;
						_buttonList [i].gameObject.GetComponent<RectTransform>().localScale = new Vector3 (1, 1, 1);
						UpgradesOpen = false;
					}
				}
			}
		}

		public void UpdateProfile(Mage mage) {
			var info = ProPage.GetComponentsInChildren<Text>();
			info[0].text = mage.Name + "\n" + "Level X " + mage.Element + " Mage";
			info[1].text = "Hellöööö!";
			info[2].text = "Damage: " + "\n" + "Rate: " + "\n" + "Range: ";
			ProPage.GetComponentInChildren<Button>().onClick.AddListener(delegate {
                Player.OpenCloseMenu(ProPage);
				EnableDisableButtons();
			});
		}

		public void EnableDisableButtons() {
		    foreach (var button in _buttonList)
		    {
		        button.interactable = !button.interactable;
		    }
		}

		public void AddMageButton(Mage mage)
		{
			var mageButton = Instantiate(MageButtonPrefab);
			mageButton.transform.SetParent(transform, false);
			mageButton.transform.localPosition = new Vector3 (0f, -50f, 0f);
			mageButton.GetComponentInChildren<Text>().text = mage.Name;
			_buttonList.Add(mageButton);
			mageButton.onClick.AddListener(delegate {

				Player.OpenCloseMenu(ProPage);
				EnableDisableButtons();
				UpdateProfile(mage);
			});
			
			GameObject upgrades = mageButton.gameObject.transform.GetChild(1).gameObject;
			var upgradeButtons = upgrades.GetComponentsInChildren<Button>();
			upgradeButtons[0].onClick.AddListener (delegate {
				mage.upgradeDamage();
				UpdatePrices(mage,upgrades);
			});
			upgradeButtons[1].onClick.AddListener (delegate {
				mage.upgradeRange();
				UpdatePrices(mage,upgrades);
			});
			upgradeButtons[2].onClick.AddListener (delegate {
				mage.upgradeRate();
				UpdatePrices(mage,upgrades);
			});
			MageUpgradePanel.offsetMin = new Vector2 (MageUpgradePanel.offsetMin.x, MageUpgradePanel.offsetMin.y - 55);
		}
    }
}