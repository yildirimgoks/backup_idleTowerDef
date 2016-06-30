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
		private Button[] ProPageButtons;
		private Text[] Info;

	    public Player Player;

        private List<Button> _buttonList = new List<Button>();

		public bool _profileOpen;
		public bool MageMenuOpen;

		private BigIntWithUnit price;

		private void Awake() {
			Ins = this;
		}

		private void Start() {
			_profileOpen = false;
			MageMenuOpen = false;
			EnableDisableButtons (false);
			OpenCloseButton.onClick.AddListener (delegate {
				Player.OpenCloseMenu(OpenCloseButton.GetComponentInParent<Animator>().gameObject,true);
				Player.OpenCloseMenu(ProPage, false);
				EnableDisableButtons(!MageMenuOpen);
				MageMenuOpen=!MageMenuOpen;
			});
			ProPageButtons = ProPage.GetComponentsInChildren<Button>();
			Info = ProPage.GetComponentsInChildren<Text>();
		}

		private void Update(){
			ProPageButtons [1].GetComponentInChildren<Text> ().text = "Level Up (" + price + ")";
			ProPageButtons [1].interactable = (Player.GetCurrency () >= price);

		}

		public void EnableDisableButtons(bool open) {
			foreach (var button in _buttonList)
			{
				button.interactable = (!button.interactable && open);
			}
		}

		private void SetPerson(Mage mage){
			price = mage.GetUpgradePrice();
		}
		private void SetPerson(){
			price = Player.GetUpgradePrice();
		}

		public void UpdateProfile(Mage mage) {
			SetPerson(mage);
			Info[0].text = mage.Name + "\n" + "Level "+mage.GetSpecs()[0]+ " " + mage.Element + " Mage";
			Info[1].text = "'"+mage.Line+"'";
			Info[2].text = "Damage: " + mage.GetSpecs()[1]+ "\n" + "Rate: " +mage.GetSpecs()[2]+ "\n" + "Range: "+mage.GetSpecs()[3];
			ProPage.GetComponent<Image>().color = ElementController.Instance.GetColor(mage.Element);
			ProPageButtons[0].onClick.AddListener(delegate {
				Player.OpenCloseMenu(ProPage,true);
				mage.highlight.enabled=false;
				EnableDisableButtons(true);
				Debug.Log("This works");
				ProPageButtons[0].onClick.RemoveAllListeners();
				ProPageButtons[1].onClick.RemoveAllListeners();
			});
			ProPageButtons [1].onClick.AddListener (delegate {
				mage.UpgradeMage();
				Debug.Log("This works");
			});
		}

		public void UpdatePlayerProfile(){
			SetPerson();
			Info[0].text = "Nabukadnezar\nGrandmaster Wizard";
			Info[1].text = "Gidişime yollar, büyüşüme kızlar hasta.";
			Info[2].text = "Damage: " + "\n" + "Rate: " + "\n" + "Range: ";
			ProPage.GetComponent<Image> ().color = Color.white;
			ProPageButtons[0].onClick.AddListener(delegate {
				Player.OpenCloseMenu(ProPage,true);
				EnableDisableButtons(true);
				Debug.Log("This works");
				ProPageButtons[0].onClick.RemoveAllListeners();
				ProPageButtons[1].onClick.RemoveAllListeners();
			});
			ProPageButtons [1].onClick.AddListener (delegate {
				Player.UpgradePlayer();
				Debug.Log("This works");
			});
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
				TowerMenuSpawner.INSTANCE.OpenMenu.AttachedTower.MenuOpen=false;		//Burası Null reference veriyor, menu açık değilse de kapamaya çalıştığı için
			});
			MageUpgradePanel.offsetMin = new Vector2 (MageUpgradePanel.offsetMin.x, MageUpgradePanel.offsetMin.y - 55);
		}
    }
}