using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
	public class MageButtons : MonoBehaviour {

		public static MageButtons Ins;

        public Button MageButtonPrefab;
        public RectTransform MageUpgradePanel;

        Button[] ButtonList = new Button[10];

        public int MageCount;
		public bool UpgradesOpen;

		private void Awake() {
			Ins = this;
		}

		private void Start() {
			MageCount = 0;
			UpgradesOpen = false;
		}
        
		public void OpenCloseUpgrades(GameObject Upgrades) {
            Upgrades.SetActive(!Upgrades.activeInHierarchy);
		}

		public void UpdatePrices(Mage mage, GameObject Upgrades){
			var upgradeButtons = Upgrades.GetComponentsInChildren<Button>();
			upgradeButtons[0].GetComponentInChildren<Text>().text="Upgrade Mage Damage (" + mage._damagePrice + ")";
			upgradeButtons[1].GetComponentInChildren<Text>().text="Upgrade Mage Range (" + mage._rangePrice + ")";
			upgradeButtons[2].GetComponentInChildren<Text>().text="Upgrade Mage Fire Rate (" + mage._ratePrice + ")";
		}

		public void HideOtherButtons(int buttonNumber){
			if (!UpgradesOpen) {
				for (int i = 0; i < ButtonList.Length; i++) {
					if (i != buttonNumber) {
						ButtonList [i].interactable = false;
						ButtonList [i].gameObject.GetComponent<RectTransform> ().localScale = new Vector3 (0, 0, 0);
						UpgradesOpen = true;
					}
				}
			} else {
				for (int i =0; i < ButtonList.Length; i++) {
					if (i != buttonNumber) {
						ButtonList [i].interactable = true;
						ButtonList [i].gameObject.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
						UpgradesOpen = false;
					}
				}
			}
		}

		public void AddMageButton(Mage mage)
		{
			Button MageButton = Instantiate (MageButtonPrefab) as Button;
			MageButton.transform.SetParent (transform, false);
			MageButton.transform.localPosition = new Vector3 (0f, -50f, 0f);
			MageButton.GetComponentInChildren<Text> ().text = mage.Name;
			ButtonList[MageCount] = MageButton;
			int a = MageCount;
			GameObject upgrades = MageButton.gameObject.transform.GetChild(1).gameObject;
			MageButton.onClick.AddListener (delegate {
				OpenCloseUpgrades(upgrades);
				UpdatePrices(mage,upgrades);
				HideOtherButtons(a);
			});
			MageCount++;
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