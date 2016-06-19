using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
	public class MageButtons : MonoBehaviour {

		public static MageButtons Ins;

		public int MageCount;
		public bool UpgradesOpen;

		private void Awake(){
			Ins = this;
		}

		private void Start(){
			MageCount = 0;
			UpgradesOpen = false;
		}

		public Button MageButtonPrefab;
		public RectTransform MageUpgradePanel;

		public void OpenCloseUpgrades(GameObject Upgrades){
			if (Upgrades.active)
				Upgrades.SetActive (false);
			else
				Upgrades.SetActive (true);
		}

		public void UpdatePrices(GameObject Upgrades){
			Button[] UpgradeButtons = Upgrades.GetComponentsInChildren<Button> ();
			var Playersc = Camera.main.GetComponent<Player>();
			UpgradeButtons[0].GetComponentInChildren<Text> ().text="Upgrade Mage Damage (" + Playersc._priceDamageUpgrade + ")";
			UpgradeButtons[1].GetComponentInChildren<Text> ().text="Upgrade Mage Range (" + Playersc._priceRangeUpgrade + ")";
			UpgradeButtons[2].GetComponentInChildren<Text> ().text="Upgrade Mage Fire Rate (" + Playersc._priceFirerateUpgrade + ")";
		}

		public void HideOtherButtons(int buttonNumber){
			if (!UpgradesOpen) {
				for (int i = buttonNumber+1; i < ButtonList.Length; i++) {
					ButtonList [i].interactable = false;
					ButtonList [i].gameObject.GetComponent<RectTransform> ().localScale = new Vector3 (0,0,0);
					UpgradesOpen= true;
				}
			} else {
				for (int i = buttonNumber+1; i < ButtonList.Length; i++) {
					ButtonList [i].interactable = true;
					ButtonList [i].gameObject.GetComponent<RectTransform> ().localScale = new Vector3 (1,1,1);
					UpgradesOpen = false;
				}
			}
		}

		Button[] ButtonList = new Button[10];

		public void AddMageButton(Mage mage)
		{
			Button MageButton = Instantiate (MageButtonPrefab) as Button;
			MageButton.transform.SetParent (transform, false);
			MageButton.transform.localPosition = new Vector3 (0f, -50f, 0f);
			MageButton.GetComponentInChildren<Text> ().text = mage.Name;
			ButtonList[MageCount] = MageButton;
			int a = MageCount;
			GameObject Upgrades = MageButton.gameObject.transform.GetChild(1).gameObject;
			MageButton.onClick.AddListener (delegate {
				OpenCloseUpgrades(Upgrades);
				UpdatePrices(Upgrades);
				HideOtherButtons(a);
			});
			MageCount++;
			Button[] UpgradeButtons = Upgrades.GetComponentsInChildren<Button> ();
			var Playersc = Camera.main.GetComponent<Player>();
			UpgradeButtons [0].onClick.AddListener (delegate {
				Playersc.UpgradeDamage();
				UpdatePrices(Upgrades);
			});
			UpgradeButtons [1].onClick.AddListener (delegate {
				Playersc.UpgradeRange();
				UpdatePrices(Upgrades);
			});
			UpgradeButtons [2].onClick.AddListener (delegate {
				Playersc.UpgradeRate();
				UpdatePrices(Upgrades);
			});
			MageUpgradePanel.offsetMin = new Vector2 (MageUpgradePanel.offsetMin.x, MageUpgradePanel.offsetMin.y - 55);
		}
	}
}