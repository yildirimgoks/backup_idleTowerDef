using UnityEngine;

namespace Assets.Scripts
{
	public class BuildingMenuSpawner : MonoBehaviour {
	
		public static BuildingMenuSpawner INSTANCE;
		public BuildingMenu MenuPrefab;
		public BuildingMenu OpenMenu;
	
		void Awake(){
			INSTANCE = this;
		}

		void Start(){
			OpenMenu = null;
		}
	
		public void SpawnMenu(MageAssignableBuilding building){
			if (OpenMenu) {
				OpenMenu.AttachedBuilding.MenuOpen = false;
				OpenMenu = null;
			}
			BuildingMenu newMenu = Instantiate (MenuPrefab);
			newMenu.transform.SetParent (transform, false);
			newMenu.transform.position = new Vector3(building.gameObject.transform.position.x,13f, building.gameObject.transform.position.z);
			newMenu.AttachedBuilding = building;
			newMenu.SpawnButtons(building);
            OpenMenu = newMenu;
            building.MenuOpen = true;
		}
	}
}