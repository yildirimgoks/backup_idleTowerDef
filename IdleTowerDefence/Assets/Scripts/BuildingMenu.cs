﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
	public class BuildingMenu : MonoBehaviour {
		
		public Button ButtonPrefab;
		public MageAssignableBuilding AttachedBuilding;

		// Use this for initialization
		public void SpawnButtons (MageAssignableBuilding building) {
            AttachedBuilding = building;
			for (var i = 0; i < building.options.Length; i++) {
				var newButton = Instantiate (ButtonPrefab);
				newButton.transform.SetParent (transform, false);
				var theta = (2 * Mathf.PI / building.options.Length) * i;
				var x = Mathf.Sin (theta+Mathf.PI/2);
				var y = Mathf.Cos (theta+Mathf.PI/2);
				newButton.transform.localPosition = new Vector3 (x, y, 0f)*100f;
				var icon = newButton.transform.GetChild (0);		//direk getcomponentinchildren işe yaramadı nedense
				icon.GetComponent<Image>().sprite = building.options[i].sprite;
				newButton.onClick.AddListener(building.options[i].function);
				newButton.onClick.AddListener(
                    delegate {
                        CloseMenu(this);
                        AttachedBuilding.Highlight.enabled = false;
                    }
                );
			}
		}

		public void CloseMenu(BuildingMenu menu){
			menu.AttachedBuilding.MenuOpen = false;
			Destroy (menu.gameObject);
		}

		void Update(){
			if (!AttachedBuilding.MenuOpen) {
				Destroy (gameObject);
                AttachedBuilding.Highlight.enabled = false;
			}
		}
	}
}
