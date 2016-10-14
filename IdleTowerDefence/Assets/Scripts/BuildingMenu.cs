using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Manager;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
	public class BuildingMenu : MonoBehaviour {

		public UIManager UIManager;

		public Button ButtonPrefab;
		public MageAssignableBuilding AttachedBuilding;

		private bool[] conditions;
		private Func<bool[]> conditionGetter;

		private Button[] ButtonList;

		void Start () {
			UIManager=AttachedBuilding.Player.GetComponent<UIManager>();
		}

		// Use this for initialization
		public void SpawnButtons (MageAssignableBuilding building) {
            AttachedBuilding = building;
			ButtonList=new Button[building.options.Length];
			for (var i = 0; i < building.options.Length; i++) {
				var newButton = Instantiate (ButtonPrefab);
				newButton.transform.SetParent (transform, false);
				var theta = (2 * Mathf.PI / building.options.Length) * i;
				var x = Mathf.Sin (theta+Mathf.PI/2);
				var y = Mathf.Cos (theta+Mathf.PI/2);
				newButton.transform.localPosition = new Vector3 (x, y, 0f)*125f;
				ButtonList [i] = newButton;
				var icon = newButton.transform.GetChild (0);		//direk getcomponentinchildren işe yaramadı nedense
				icon.GetComponent<Image>().sprite = building.options[i].sprite;
				//conditions[i] = building.options[i].condition;

				for ( var j = 0 ; j < building.options[i].actions.Length ; j++){
					if ( building.options[i].actions[j] == null) break;
					ActionWithEvent action = building.options[i].actions[j];
					EventTrigger trigger = newButton.GetComponent<EventTrigger>();
					EventTrigger.Entry entry = new EventTrigger.Entry();
					entry.eventID = action.triggerType;
					entry.callback.AddListener(action.function);
					// entry.callback.AddListener(call);
					trigger.triggers.Add(entry);
				}

				// newButton.onClick.AddListener(building.options[i].actions[0].function);
				if(i==0){
				newButton.onClick.AddListener(
                    delegate {
						CloseMenu(this);
                    }
					);
				}

			}
            AttachedBuilding.DisplayRangeObject();
			//conditionGetter = conditions;
		}

		public void TestFunc( UnityEngine.EventSystems.BaseEventData baseEvent) {
        	Debug.Log("Lo and behold");
		}

		public void CloseMenu(BuildingMenu menu){
            menu.AttachedBuilding.HideRangeObject();
			menu.AttachedBuilding.MenuOpen = false;
			UIManager.DestroyTowerMenuCloser();
		}

		void Update(){
			if (!AttachedBuilding.MenuOpen) {
				AttachedBuilding.Menu = null;
				Destroy (gameObject);
                AttachedBuilding.Highlight.enabled = false;
				for (var i = 0; i < ButtonList.Length; i++) {
					ButtonList [i] = null;
				}
			}

			//var conditions = conditionGetter.Invoke();

			//for(var i=0;i<ButtonList.Length;i++) {
			//	ButtonList [i].interactable = conditions [i];
			//}

			if (ButtonList [1]) {
				ButtonList [1].interactable = !(UIManager.Player.Data.GetCurrency () < AttachedBuilding.InsideMage.Data.GetUpgradePrice ());
			}
		}
	}
}
