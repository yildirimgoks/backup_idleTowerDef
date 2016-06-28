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

		public Button OpenCloseButton;

	    public Player Player;

        private List<Button> _buttonList = new List<Button>();

		public bool UpgradesOpen;
		public bool MageMenuOpen;

		private void Awake() {
			Ins = this;
		}

		private void Start() {
			UpgradesOpen = false;
			MageMenuOpen = true;
			OpenCloseButton.onClick.AddListener (delegate {
				Player.OpenCloseMenu(OpenCloseButton.GetComponentInParent<Animator>().gameObject,true);
				Player.OpenCloseMenu(ProPage, false);
				EnableDisableButtons(!MageMenuOpen);
				MageMenuOpen=!MageMenuOpen;
			});
		}
        
		public void OpenCloseUpgrades(GameObject Upgrades) {		//will be deleted
            Upgrades.SetActive(!Upgrades.activeInHierarchy);
		}

		public void HideOtherButtons(int buttonNumber) {		//will be deleted
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
			info[0].text = mage.Name + "\n" + "Level "+mage.GetSpecs()[0]+ " " + mage.Element + " Mage";
			info[1].text = "'"+mage.Line+"'";
			info[2].text = "Damage: " + mage.GetSpecs()[1]+ "\n" + "Rate: " +mage.GetSpecs()[2]+ "\n" + "Range: "+mage.GetSpecs()[3];
			ProPage.GetComponent<Image>().color = ElementController.Instance.GetColor(mage.Element);
			var Buttons = ProPage.GetComponentsInChildren<Button> ();
			Buttons[0].onClick.AddListener(delegate {
				Player.OpenCloseMenu(ProPage,true);
				mage.highlight.enabled=false;
				EnableDisableButtons(true);
				Debug.Log("This works");
				Buttons[0].onClick.RemoveAllListeners();
				Buttons[1].onClick.RemoveAllListeners();
			});
			Buttons [1].onClick.AddListener (delegate {
				//Buraya upgrade function gelecek
				Debug.Log("This works");
			});
		}

		public void UpdatePlayerProfile(){
			var info = ProPage.GetComponentsInChildren<Text>();
			info[0].text = "Nabukadnezar\nGrandmaster Wizard";
			info[1].text = "Gidişime yollar, büyüşüme kızlar hasta.";
			info[2].text = "Damage: " + "\n" + "Rate: " + "\n" + "Range: ";
			ProPage.GetComponent<Image> ().color = Color.white;
			ProPage.GetComponentInChildren<Button>().onClick.AddListener(delegate {
				Player.OpenCloseMenu(ProPage,true);
				EnableDisableButtons(true);
				Debug.Log("This works");
			});
		}

		public void EnableDisableButtons(bool open) {
		    foreach (var button in _buttonList)
		    {
				button.interactable = (!button.interactable && open);
		    }
		}

		public void AddPlayerButton(){
			var mageButton = Instantiate(MageButtonPrefab);
			mageButton.transform.SetParent(transform, false);
			mageButton.transform.localPosition = new Vector3 (0f, -50f, 0f);
			mageButton.GetComponentInChildren<Text>().text = "Player";
			_buttonList.Add(mageButton);
			mageButton.onClick.AddListener(delegate {
				Player.OpenCloseMenu(ProPage,true);
				EnableDisableButtons(true);
				UpdatePlayerProfile();
				TowerMenuSpawner.INSTANCE.OpenMenu.AttachedTower.MenuOpen=false;
			});
			MageUpgradePanel.offsetMin = new Vector2 (MageUpgradePanel.offsetMin.x, MageUpgradePanel.offsetMin.y - 55);
		}

		public void AddMageButton(Mage mage)
		{
			var mageButton = Instantiate(MageButtonPrefab);
			mageButton.transform.SetParent(transform, false);
			mageButton.transform.localPosition = new Vector3 (0f, -50f, 0f);
			mageButton.GetComponentInChildren<Text>().text = mage.Name;
			_buttonList.Add(mageButton);
			mageButton.onClick.AddListener(delegate {
				Player.OpenCloseMenu(ProPage,true);
				EnableDisableButtons(true);
				UpdateProfile(mage);
				mage.highlight.enabled=true;
				TowerMenuSpawner.INSTANCE.OpenMenu.AttachedTower.MenuOpen=false;		//Burası Null reference veriyordu, menu açık değilse de kapamaya çalıştığı için, instance olunca vermedi
			});
			MageUpgradePanel.offsetMin = new Vector2 (MageUpgradePanel.offsetMin.x, MageUpgradePanel.offsetMin.y - 55);
		}
    }
}