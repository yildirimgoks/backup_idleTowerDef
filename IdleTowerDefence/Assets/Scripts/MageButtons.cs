using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
	public class MageButtons : MonoBehaviour {

		public static MageButtons Instance;

        public Button MageButtonPrefab;
        public RectTransform MageUpgradePanel;
		public GameObject ProfilePage;

		public Button OpenCloseButton;
		private Button[] ProfilePageButtons;
		private Text[] Info;

	    public Player Player;

        private List<Button> _buttonList = new List<Button>();

		public bool _profileOpen;
		public bool MageMenuOpen;

		private Func<BigIntWithUnit> priceGetter;

		private void Awake() {
			Instance = this;
		}

		private void Start() {
			_profileOpen = false;
			MageMenuOpen = false;
			EnableDisableButtons (false);
			OpenCloseButton.onClick.AddListener (delegate {
				Player.OpenCloseMenu(OpenCloseButton.GetComponentInParent<Animator>().gameObject,true);
				Player.OpenCloseMenu(ProfilePage, false);
				EnableDisableButtons(!MageMenuOpen);
				MageMenuOpen=!MageMenuOpen;
			});
			ProfilePageButtons = ProfilePage.GetComponentsInChildren<Button>();
			Info = ProfilePage.GetComponentsInChildren<Text>();
		}

		private void Update()
		{
		    if (ProfilePageButtons != null && priceGetter != null)
		    {
		        var currentPrice = priceGetter.Invoke();
		        ProfilePageButtons[1].GetComponentInChildren<Text>().text = "Level Up (" + currentPrice + ")";
		        ProfilePageButtons[1].interactable = Player.Data.GetCurrency() >= currentPrice;
		    }
		}

	    public void EnableDisableButtons(bool open) {
			foreach (var button in _buttonList)
			{
				button.interactable = !button.interactable && open;
			}
		}

		private void SetPerson(MageData mage){
			priceGetter = mage.GetUpgradePrice;
		}

		private void SetPerson(){
            priceGetter = Player.Data.GetUpgradePrice;
		}

		public void UpdateProfile(Mage mage) {
			SetPerson(mage.Data);
		    var profileInfo = mage.Data.GetProfileInfo();
		    Info[0].text = mage.Data.GetName() + "\n" + "Level "+profileInfo[0]+ " " + mage.Data.GetElement() + " Mage";
			Info[1].text = "'"+mage.Data.GetLine()+"'";
			Info[2].text = "Damage: " + profileInfo[1]+ "\n" + "Rate: " +profileInfo[2]+ "\n" + "Range: "+profileInfo[3];
			ProfilePage.GetComponent<Image>().color = ElementController.Instance.GetColor(mage.Data.GetElement());
			ProfilePageButtons[0].onClick.AddListener(delegate {
				Player.OpenCloseMenu(ProfilePage,true);
				mage.Highlight.enabled=false;
				EnableDisableButtons(true);
				//Debug.Log("This works");
				ProfilePageButtons[0].onClick.RemoveAllListeners();
				ProfilePageButtons[1].onClick.RemoveAllListeners();
			});
			ProfilePageButtons [1].onClick.AddListener (delegate {
				mage.UpgradeMage();
				//Debug.Log("This works");
			});
		}

		public void UpdatePlayerProfile(){
			SetPerson();
			Info[0].text = "Nabukadnezar\nGrandmaster Wizard";
			Info[1].text = "Gidişime yollar, büyüşüme kızlar hasta.";
			Info[2].text = "Damage: " + "\n" + "Rate: " + "\n" + "Range: ";
			ProfilePage.GetComponent<Image> ().color = Color.white;
			ProfilePageButtons[0].onClick.AddListener(delegate {
				Player.OpenCloseMenu(ProfilePage,true);
				EnableDisableButtons(true);
				//Debug.Log("This works");
				ProfilePageButtons[0].onClick.RemoveAllListeners();
				ProfilePageButtons[1].onClick.RemoveAllListeners();
			});
			ProfilePageButtons [1].onClick.AddListener (delegate {
				Player.Data.UpgradePlayer();
				//Debug.Log("This works");
			});
		}

		public void AddPlayerButton(){
			var mageButton = Instantiate(MageButtonPrefab);
			mageButton.transform.SetParent(transform, false);
			mageButton.transform.localPosition = new Vector3 (0f, -50f, 0f);
			mageButton.GetComponentInChildren<Text>().text = "Player";
			_buttonList.Add(mageButton);
			mageButton.onClick.AddListener(delegate {
				Player.OpenCloseMenu(ProfilePage,true);
				EnableDisableButtons(true);
				UpdatePlayerProfile();
				BuildingMenuSpawner.INSTANCE.OpenMenu.AttachedBuilding.MenuOpen=false;
			});
			MageUpgradePanel.offsetMin = new Vector2 (MageUpgradePanel.offsetMin.x, MageUpgradePanel.offsetMin.y - 55);
		}

		public void AddMageButton(Mage mage)
		{
			var mageButton = Instantiate(MageButtonPrefab);
			mageButton.transform.SetParent(transform, false);
			mageButton.transform.localPosition = new Vector3 (0f, -50f, 0f);
			mageButton.GetComponentInChildren<Text>().text = mage.Data.GetName();
			_buttonList.Add(mageButton);
			mageButton.onClick.AddListener(delegate {
				Player.OpenCloseMenu(ProfilePage,true);
				EnableDisableButtons(true);
				UpdateProfile(mage);
				mage.Highlight.enabled=true;
				BuildingMenuSpawner.INSTANCE.OpenMenu.AttachedBuilding.MenuOpen=false;		//Burası Null reference veriyor, menu açık değilse de kapamaya çalıştığı için
			});
			MageUpgradePanel.offsetMin = new Vector2 (MageUpgradePanel.offsetMin.x, MageUpgradePanel.offsetMin.y - 55);
		}
    }
}