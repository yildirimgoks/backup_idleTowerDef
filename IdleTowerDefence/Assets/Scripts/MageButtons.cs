using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Manager;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
	public class MageButtons : MonoBehaviour {

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

		private Func<BigIntWithUnit> priceGetter;
		private Func<string[]> infoGetter;

		private void Awake() {
			Instance = this;
		}

		private void Start() {
			MageMenuOpen = false;
			openProfilePage = null;
			OpenCloseButton.onClick.AddListener (delegate {
				Player.OpenCloseMenu(OpenCloseButton.GetComponentInParent<Animator>().gameObject,true);
				if(MageMenuOpen){
					gameObject.GetComponent<ToggleGroup>().SetAllTogglesOff();
					UIManager.DestroyMainMenuCloser();
				}else{
					UIManager.CreateMainMenuCloser(delegate {
						RaycastHit towerHit;
						if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out towerHit)){
							if(towerHit.collider.tag!="Tower" && towerHit.collider.tag!="Shrine" && towerHit.collider.tag!="Mage"){
								Player.OpenCloseMenu(OpenCloseButton.GetComponentInParent<Animator>().gameObject,true);
								gameObject.GetComponent<ToggleGroup>().SetAllTogglesOff();
								MageMenuOpen=!MageMenuOpen;
								UIManager.DestroyMainMenuCloser();
							}
						}
					});
				}
				MageMenuOpen=!MageMenuOpen;
			});
		}

		private void Update()
		{
			if (openProfilePage != null && priceGetter != null)
		    {
		        var currentPrice = priceGetter.Invoke();
				var currentInfo = infoGetter.Invoke();
				var UpgradeButton1 = openProfilePage.GetComponentsInChildren<Button>()[0];
				var UpgradeButton2 = openProfilePage.GetComponentsInChildren<Button>()[1];
				UpgradeButton1.GetComponentInChildren<Text>().text = "Level Up (" + currentPrice + ")";
				UpgradeButton1.interactable = Player.Data.GetCurrency() >= currentPrice;
				//TO DO: 2nd Upgrade Labels;
				//UpgradeButton2.GetComponentInChildren<Text>().text =
				//UpgradeButton2.interactable = Player.Data.GetCurrency() >= 
				Info = openProfilePage.GetComponentsInChildren<Text>();
				Info[0].text = currentInfo[0] + "\n" + "Level "+currentInfo[1]+ " " + currentInfo[2] + " Mage";
				Info[1].text = "'"+currentInfo[3]+"'";
				Info[2].text = "Damage: " + currentInfo[4]+ "\n" + "Rate: " +currentInfo[5]+ "\n" + "Range: "+currentInfo[6];
			}
		}

		public void SetScroll(int buttonIndex){
			var nameHeight = MageButtonPrefab.GetComponentsInChildren<LayoutElement> () [1].preferredHeight;
			var profileHeight = MageButtonPrefab.GetComponentsInChildren<LayoutElement> () [2].preferredHeight;
			var spacing = gameObject.GetComponent<VerticalLayoutGroup> ().spacing;
			var totalHeight = _buttonCount*nameHeight+profileHeight+(_buttonCount+1)*spacing;
			var viewportHeight = Viewport.rect.height;
			var diff = totalHeight - viewportHeight;
			var above = buttonIndex * spacing + (buttonIndex - 1) * nameHeight;
			MageListScroll.verticalNormalizedPosition = (diff - above) / diff;
		}

		private void SetPerson(MageData mage, GameObject _profilePage){
			for(int i=0;i<_buttonCount;i++){
				var mageButton = gameObject.transform.GetChild(i);
				if (mageButton.GetComponentInChildren<Renderer> ()) {
					mageButton.GetComponentInChildren<Renderer> ().enabled = false;
				}
			}
			openProfilePage = _profilePage;
			priceGetter = mage.GetUpgradePrice;
			infoGetter = mage.GetProfileInfo;

			foreach (var rend in TVMage.gameObject.GetComponentsInChildren<Renderer>())
			{
				if (rend.name.Contains ("Body")) {
					rend.material.mainTexture = ElementController.Instance.GetMage (mage.GetElement ())[0];
				} else {
					rend.material.mainTexture = ElementController.Instance.GetMage (mage.GetElement ())[1];
				}
			}    
					
			if (_profilePage.GetComponentInParent<ToggleGroup> ().AnyTogglesOn ()) {
				_profilePage.GetComponentInChildren<Renderer> ().enabled = true;
			}
		}

		private void SetPerson(GameObject _profilePage){
			for(int i=0;i<_buttonCount;i++){
				var mageButton = gameObject.transform.GetChild (i);
				if (mageButton.GetComponentInChildren<Renderer> ()) {
					mageButton.GetComponentInChildren<Renderer> ().enabled = false;
				}
			}
			openProfilePage = _profilePage;
            priceGetter = Player.Data.GetUpgradePrice;
			infoGetter = Player.Data.GetProfileInfo;

			var number = (int)Player.Data.GetElement () - 1;
			_profilePage.transform.GetChild(0).GetComponent<Image> ().sprite = PlayerPics [number];
		}



		public void AddPlayerButton(){
			var mageButton = Instantiate(PlayerButtonPrefab);
			_buttonCount = 1;
			mageButton.transform.SetParent(transform, false);
			mageButton.GetComponent<UIAccordionElement> ().SetAccordion ();
			mageButton.GetComponentInChildren<Text>().text = Player.Data.GetPlayerName();
			var ProfilePage = mageButton.gameObject.transform.GetChild(1);
			ProfilePage.GetComponent<Image>().color = ElementController.Instance.GetColor(Player.Data.GetElement());
			var Buttons = ProfilePage.GetComponentsInChildren<Button> ();
			Buttons[0].onClick.AddListener (delegate {
				Player.Data.UpgradePlayer();
			});
			//Buttons [1].onClick.AddListener (delegate {
			//	2. upgrade
			//});
			Buttons [2].onClick.AddListener (delegate {
				Player.ResetGame();	
			});
			mageButton.GetComponent<UIAccordionElement> ().onValueChanged.AddListener (delegate {
				SetPerson(ProfilePage.gameObject);
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
			mageButton.GetComponent<UIAccordionElement> ().SetAccordion ();
			mageButton.GetComponentInChildren<Text>().text = mage.Data.GetName();
			mageButton.GetComponentInChildren<Renderer>().enabled=false;
			var ProfilePage = mageButton.gameObject.transform.GetChild(1);
			ProfilePage.GetComponent<Image>().color = ElementController.Instance.GetColor(mage.Data.GetElement());
			var Buttons=ProfilePage.GetComponentsInChildren<Button> ();
			Buttons[0].onClick.AddListener (delegate {
				mage.UpgradeMage();	
			});
			//Buttons[1].onClick.AddListener (delegate {
			//	2. upgrade	
			//});
			mageButton.GetComponent<UIAccordionElement>().onValueChanged.AddListener(delegate {
				SetPerson(mage.Data,ProfilePage.gameObject);
				if(mage.GetBuilding()){
					mage.GetBuilding().Highlight.enabled=mageButton.GetComponent<UIAccordionElement>().isOn;
					if(mage.GetBuilding().MenuOpen){
						mage.GetBuilding().MenuOpen=mageButton.GetComponent<UIAccordionElement>().isOn;
						if(MageMenuOpen){
							UIManager.DestroyTowerMenuCloser();
						}
					}
				} else {
					mage.Highlight.enabled=mageButton.GetComponent<UIAccordionElement>().isOn;
				}
				SetScroll(mage.ProfileButtonIndex);
			});
		}

		public void ResetMageMenu(){
			var Buttons = GameObject.FindGameObjectsWithTag ("MageButton");
			foreach (var mageButton in Buttons) {
				Destroy (mageButton);
			}
			_buttonCount = 0;
			openProfilePage = null;
		}
    }
}