using UnityEngine;

namespace Assets.Scripts
{
	public class TowerMenuSpawner : MonoBehaviour {
	
		public static TowerMenuSpawner INSTANCE;	//Sor!!
		public TowerMenu MenuPrefab;
		public TowerMenu OpenMenu;
	
		void Awake(){
			INSTANCE = this;
		}

		void Start(){
			OpenMenu = null;
		}
	
		public void SpawnMenu(Tower tower){
			if (OpenMenu) {
				OpenMenu.AttachedTower.MenuOpen = false;
				OpenMenu = null;
			}
			TowerMenu newMenu = Instantiate (MenuPrefab);
			newMenu.transform.SetParent (transform, false);
			newMenu.transform.position = new Vector3(tower.gameObject.transform.position.x,13f,tower.gameObject.transform.position.z);
			newMenu.AttachedTower = tower;
			newMenu.SpawnButtons (tower);
            OpenMenu = newMenu;
			tower.MenuOpen = true;
		}
	}
}