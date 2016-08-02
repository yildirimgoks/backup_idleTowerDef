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
		private Text[] Info;

	    public Player Player;

        //private List<Button> _buttonList = new List<Button>();

		public bool MageMenuOpen;

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



		private void SetPerson(MageData mage, GameObject _profilePage){
			openProfilePage = _profilePage;
			priceGetter = mage.GetUpgradePrice;
			infoGetter = mage.GetProfileInfo;
		}

		private void SetPerson(GameObject _profilePage){
			openProfilePage = _profilePage;
            priceGetter = Player.Data.GetUpgradePrice;
			infoGetter = Player.Data.GetProfileInfo;
		}



		public void AddPlayerButton(){
			var mageButton = Instantiate(MageButtonPrefab);
			mageButton.transform.SetParent(transform, false);
			mageButton.GetComponent<UIAccordionElement> ().SetAccordion ();
			mageButton.GetComponentInChildren<Text>().text = Player.Data.GetPlayerName();
            mageButton.GetComponentInChildren<Text>().color = Color.yellow;
			var ProfilePage = mageButton.gameObject.transform.GetChild(1);
			ProfilePage.GetComponent<Image>().color = ElementController.Instance.GetColor(Player.Data.GetElement());
			ProfilePage.GetComponentInChildren<Button> ().onClick.AddListener (delegate {
				Player.Data.UpgradePlayer();
			});
			mageButton.GetComponent<UIAccordionElement> ().onValueChanged.AddListener (delegate {
				SetPerson(ProfilePage.gameObject);
			});
		}

		public void AddMageButton(Mage mage)
		{
			var mageButton = Instantiate(MageButtonPrefab);
			mage.ProfileButton = mageButton;
			mageButton.transform.SetParent(transform, false);
			mageButton.GetComponent<UIAccordionElement> ().SetAccordion ();
			mageButton.GetComponentInChildren<Text>().text = mage.Data.GetName();
            mageButton.GetComponentInChildren<Text>().color = Color.white;
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
			});
		}
    }
}