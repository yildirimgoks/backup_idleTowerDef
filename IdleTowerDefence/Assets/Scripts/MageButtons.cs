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
				//var currentInfo = infoGetter.Invoke ();
				var UpgradeButton = openProfilePage.GetComponentInChildren<Button>();
				UpgradeButton.GetComponentInChildren<Text>().text = "Level Up (" + currentPrice + ")";
				UpgradeButton.interactable = Player.Data.GetCurrency() >= currentPrice;
				Info = openProfilePage.GetComponentsInChildren<Text>();
				//Info[0].text = currentInfo[0] + "\n" + "Level "+currentInfo[1]+ " " + currentInfo[2] + " Mage";
				//Info[1].text = "'"+currentInfo[3]+"'";
				//Info[2].text = "Damage: " + currentInfo[4]+ "\n" + "Rate: " +currentInfo[5]+ "\n" + "Range: "+currentInfo[6];
				//Bunlar hep infoGetter'dan dolayı
		    }
		}



		private void SetPerson(MageData mage, GameObject _profilePage){
			openProfilePage = _profilePage;
			priceGetter = mage.GetUpgradePrice;
			var profileInfo = mage.GetProfileInfo();
			var personinfo = new string[7];
			personinfo[0] = mage.GetName ();
			personinfo[1] = profileInfo [0];
			personinfo[2] = mage.GetElement ().ToString();
			personinfo[3] = mage.GetLine ();
			personinfo[4] = profileInfo [1];
			personinfo[5] = profileInfo [2];
			personinfo[6] = profileInfo [3];
			//infoGetter = personinfo; //neden olmuyor nedeen?
		}

		private void SetPerson(GameObject _profilePage){
			openProfilePage = _profilePage;
            priceGetter = Player.Data.GetUpgradePrice;
			var personinfo = new string[7];
			personinfo[0] = "Nabukadnezar";
			personinfo[1] = ((Player.Data.GetSpellData().GetDamage()-20)/5).ToString();
			personinfo[2] = Player.Data.GetSpellData().GetElement().ToString();
			personinfo[3] = "Meraba";
			personinfo[4] = Player.Data.GetSpellData().GetDamage().ToString();
			personinfo[5] = "As hard as you touch me";
			personinfo[6] = "Burdan taa karşıki dağlara kadar";
			//infoGetter = personinfo; //neden olmuyor nedeen?
		}



		public void AddPlayerButton(){
			var mageButton = Instantiate(MageButtonPrefab);
			mageButton.transform.SetParent(transform, false);
			mageButton.GetComponentInChildren<Text>().text = "Player";
			var ProfilePage = mageButton.gameObject.transform.GetChild(1);
			ProfilePage.GetComponent<Image> ().color = Color.white;
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
			mageButton.transform.SetParent(transform, false);
			mageButton.GetComponentInChildren<Text>().text = mage.Data.GetName();
			var ProfilePage = mageButton.gameObject.transform.GetChild(1);
			var Info = ProfilePage.GetComponentsInChildren<Text>();
			var profileInfo = mage.Data.GetProfileInfo();
			Info[0].text = mage.Data.GetName() + "\n" + "Level "+profileInfo[0]+ " " + mage.Data.GetElement() + " Mage";
			Info[1].text = "'"+mage.Data.GetLine()+"'";
			Info[2].text = "Damage: " + profileInfo[1]+ "\n" + "Rate: " +profileInfo[2]+ "\n" + "Range: "+profileInfo[3];
			ProfilePage.GetComponentInChildren<Button> ().onClick.AddListener (delegate {
				mage.UpgradeMage();	
			});
			ProfilePage.GetComponent<Image>().color = ElementController.Instance.GetColor(mage.Data.GetElement());
			mageButton.GetComponent<UIAccordionElement>().onValueChanged.AddListener(delegate {
				SetPerson(mage.Data,ProfilePage.gameObject);
				if(mage.GetBuilding()){
					mage.GetBuilding().Highlight.enabled=!mage.GetBuilding().Highlight.enabled;
				} else {
					mage.Highlight.enabled=!mage.Highlight.enabled;
				}
			});
		}
    }
}