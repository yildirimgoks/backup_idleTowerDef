using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using Assets.Scripts.Manager;

public class DailyBonusWindow : MonoBehaviour {

	public UIManager UIManager;

	public GameObject BonusMenu;
	public GameObject Closer;
	public GameObject BonusPrefab;
	public ScrollRect BonusScroll;
	public RectTransform Viewport;

	// Use this for initialization
	void Start () {
		//SetDayBonus (1, delegate {
		//	Debug.Log ("Hello");
		//}, "It just debugs.");
		//UnlockDay (1);
		//SetScrollToDay (1);
		//UIManager.OpenCloseMenu (BonusMenu, true);
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void SetScrollToDay(int dayNumber){
		var bonusWidth = BonusPrefab.GetComponent<LayoutElement>().preferredWidth;
		var spacing = GetComponent<HorizontalLayoutGroup> ().spacing;
		var totalWidth = 7 * bonusWidth+8*spacing;
		var diff=totalWidth-Viewport.rect.width;
		var fromLeft = dayNumber * spacing + (dayNumber -1) * bonusWidth;
		BonusScroll.horizontalNormalizedPosition = fromLeft / diff;
	}

	public void UnlockDay(int daynumber){
		var dayWindow = transform.FindChild (daynumber.ToString ());
		dayWindow.FindChild ("Lock Image").gameObject.SetActive (false);
		dayWindow.GetComponent<Button> ().interactable = true;
	}

	public void SetDayBonus(int daynumber, UnityAction bonusaction, string bonusdescription){
		var dayWindow = transform.FindChild (daynumber.ToString ());
		dayWindow.GetComponent<Button> ().onClick.AddListener (bonusaction);
		dayWindow.GetComponent<Button> ().onClick.AddListener (delegate {
			UIManager.OpenCloseMenu(BonusMenu,true);
			dayWindow.GetComponent<Button> ().interactable=false;
		});
		dayWindow.FindChild ("Prize Name").GetComponent<Text> ().text = bonusdescription;
	}

	public void LockAllDays(){
		for(var day=1;day>=7;day++){
			var dayWindow = transform.FindChild (day.ToString ());
			dayWindow.FindChild ("Lock Image").gameObject.SetActive (true);
			dayWindow.GetComponent<Button> ().interactable = false;
		}
	}
}
