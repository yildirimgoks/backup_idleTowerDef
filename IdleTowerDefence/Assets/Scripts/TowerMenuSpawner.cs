using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
	public class TowerMenuSpawner : MonoBehaviour {
	
		public static TowerMenuSpawner ins;	//Sor!!
		public TowerMenu menuPrefab;
		public TowerMenu openMenu;
	
		void Awake(){
			ins = this;
		}

		void Start(){
			openMenu = null;
		}
	
		public void SpawnMenu(Tower tower){
			if (openMenu) {
				openMenu.menuTower.menuOpen = false;
				openMenu = null;
			}
			TowerMenu newMenu = Instantiate (menuPrefab) as TowerMenu;
			newMenu.transform.SetParent (transform, false);
			newMenu.transform.position = new Vector3(tower.gameObject.transform.position.x,13f,tower.gameObject.transform.position.z); //Düzelt!!
			newMenu.menuTower = tower;
			newMenu.SpawnButtons (tower);
			newMenu = openMenu;
		}
	}
}