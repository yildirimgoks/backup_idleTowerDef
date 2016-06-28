using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
	public class TowerMenu : MonoBehaviour {
		
		public Button ButtonPrefab;
		public Tower AttachedTower;

		// Use this for initialization
		public void SpawnButtons (Tower tower) {
			AttachedTower = tower;
			for (int i = 0; i < tower.options.Length; i++) {
				Button newButton = Instantiate (ButtonPrefab) as Button;
				newButton.transform.SetParent (transform, false);
				float theta = (2 * Mathf.PI / tower.options.Length) * i;
				float x = Mathf.Sin (theta+Mathf.PI/2);
				float y = Mathf.Cos (theta+Mathf.PI/2);
				newButton.transform.localPosition = new Vector3 (x, y, 0f)*100f;
				newButton.onClick.AddListener(
                    delegate {
                        if (AttachedTower.InsideMage)
                        {
                        AttachedTower.InsideMage.Eject();
                        }
                    
                        CloseMenu(this);
						AttachedTower.highlight.enabled = false;
                    }
                );
			}
		}

		public void CloseMenu(TowerMenu menu){
			menu.AttachedTower.MenuOpen = false;
			Destroy (menu.gameObject);
		}

		void Update(){
			if (!AttachedTower.MenuOpen) {
				Destroy (gameObject);
				AttachedTower.highlight.enabled = false;
			}
		}
	}
}
