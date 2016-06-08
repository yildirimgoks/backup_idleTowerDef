using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
	public class TowerMenu : MonoBehaviour {
		
		public Button buttonPrefab;
		public Button selected;
		public Tower menuTower;

		// Use this for initialization
		public void SpawnButtons (Tower tower) {
			menuTower = tower;
			for (int i = 0; i < tower.options.Length; i++) {
				Button newButton = Instantiate (buttonPrefab) as Button;
				newButton.transform.SetParent (transform, false);
				float theta = (2 * Mathf.PI / tower.options.Length) * i;
				float x = Mathf.Sin (theta);
				float y = Mathf.Cos (theta);
				newButton.transform.localPosition = new Vector3 (x, y, 0f)*100f;
				newButton.onClick.AddListener(delegate() {
					CloseMenu(this);
					//Mage.Eject(menuTower);//Düzelt!!
				});
			menuTower.menuOpen = true;
			}
		}

		public void CloseMenu(TowerMenu menu){
			menu.menuTower.menuOpen = false;
			Destroy (menu.gameObject);
		}

		void Update(){
			if (!menuTower.menuOpen) {
				Destroy (gameObject);
			}
		}
	}
}
