using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
	public class MageButtons : MonoBehaviour {

		public static MageButtons Instance;

		public GameObject MageButtonPrefab;
		public GameObject openProfilePage;

		public Button OpenCloseButton;
		public ScrollRect MageListScroll;
		private Text[] Info;

		public Mage TVMage;

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
				MageMenuOpen=!MageMenuOpen;
			});
		}

		private void Update()
		{
			if (openProfilePage != null && priceGetter != null)
		    {
		        var currentPrice = priceGetter.Invoke();
				var currentInfo = infoGetter.Invoke();
				var UpgradeButton = openProfilePage.GetComponentInChildren<Button>();
				UpgradeButton.GetComponentInChildren<Text>().text = "Level Up (" + currentPrice + ")";
				UpgradeButton.interactable = Player.Data.GetCurrency() >= currentPrice;
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
			var viewportHeight = 432;//supposed to be found from transform, or be changed whenever size changes(=1080*0.4)
			var diff = totalHeight - viewportHeight;
			var above = buttonIndex * spacing + (buttonIndex - 1) * nameHeight;
			MageListScroll.verticalNormalizedPosition = (diff - above) / diff;
		}

		private void SetPerson(MageData mage, GameObject _profilePage){
			for(int i=0;i<_buttonCount;i++){
				var mageButton = gameObject.transform.GetChild(i);
				mageButton.GetComponentInChildren<Renderer>().enabled=false;
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
				mageButton.GetComponentInChildren<Renderer>().enabled=false;
			}
			openProfilePage = _profilePage;
            priceGetter = Player.Data.GetUpgradePrice;
			infoGetter = Player.Data.GetProfileInfo;

			var number = (int)Player.Data.GetElement () - 1;
			_profilePage.transform.GetChild(0).GetComponent<Image> ().sprite = PlayerPics [number];
		}



		public void AddPlayerButton(){
			var mageButton = Instantiate(MageButtonPrefab);
			_buttonCount = 1;
			mageButton.transform.SetParent(transform, false);
			mageButton.GetComponent<UIAccordionElement> ().SetAccordion ();
			mageButton.GetComponentInChildren<Text>().text = Player.Data.GetPlayerName();
            mageButton.GetComponentInChildren<Text>().color = Color.yellow;
			mageButton.GetComponentInChildren<Renderer>().enabled=false;
			var ProfilePage = mageButton.gameObject.transform.GetChild(1);
			ProfilePage.GetComponent<Image>().color = ElementController.Instance.GetColor(Player.Data.GetElement());
			ProfilePage.GetComponentInChildren<Button> ().onClick.AddListener (delegate {
				Player.Data.UpgradePlayer();
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
            mageButton.GetComponentInChildren<Text>().color = Color.white;
			mageButton.GetComponentInChildren<Renderer>().enabled=false;
			var ProfilePage = mageButton.gameObject.transform.GetChild(1);
			ProfilePage.GetComponent<Image>().color = ElementController.Instance.GetColor(mage.Data.GetElement());
			ProfilePage.GetComponentInChildren<Button> ().onClick.AddListener (delegate {
				mage.UpgradeMage();	
			});
			mageButton.GetComponent<UIAccordionElement>().onValueChanged.AddListener(delegate {
				SetPerson(mage.Data,ProfilePage.gameObject);
				if(mage.GetBuilding()){
					mage.GetBuilding().Highlight.enabled=mageButton.GetComponent<UIAccordionElement>().isOn;
				} else {
					mage.Highlight.enabled=mageButton.GetComponent<UIAccordionElement>().isOn;
				}
				SetScroll(mage.ProfileButtonIndex);
			});
		}
    }
}