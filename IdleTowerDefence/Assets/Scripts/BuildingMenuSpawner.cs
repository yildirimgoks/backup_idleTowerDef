using UnityEngine;

namespace Assets.Scripts
{
	public class BuildingMenuSpawner : MonoBehaviour {
	
		public static BuildingMenuSpawner INSTANCE;
		public UIManager UIManager;

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
				OpenMenu.AttachedBuilding.Menu = null;
				OpenMenu = null;
			}
			BuildingMenu newMenu = Instantiate (MenuPrefab);
			newMenu.transform.SetParent (transform, false);
			newMenu.transform.position = new Vector3(building.gameObject.transform.position.x, 13f, building.gameObject.transform.position.z);
            newMenu.transform.Translate(new Vector3(0,0,-50),Space.Self);
			newMenu.AttachedBuilding = building;
			newMenu.AttachedBuilding.Menu = newMenu;
			newMenu.SpawnButtons(building);
            OpenMenu = newMenu;
            building.MenuOpen = true;
			UIManager.CreateMenuCloser (newMenu.gameObject, delegate {
				Destroy (newMenu.gameObject);
			},false);
		}
	}
}