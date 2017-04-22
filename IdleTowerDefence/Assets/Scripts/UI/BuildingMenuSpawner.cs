using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class BuildingMenuSpawner : MonoBehaviour {
		public UIManager UIManager;

		public BuildingMenu MenuPrefab;
		public BuildingMenu OpenMenu;
	
		public void SpawnMenu(MageAssignableBuilding building){
			if (OpenMenu) {
				OpenMenu.AttachedBuilding.MenuOpen = false;
				OpenMenu.AttachedBuilding.Menu = null;
				OpenMenu = null;
			}
			BuildingMenu newMenu = Instantiate (MenuPrefab);
			newMenu.transform.SetParent (transform, false);
			newMenu.transform.position = new Vector3(building.gameObject.transform.position.x, 13f, building.gameObject.transform.position.z);
            newMenu.transform.Translate(new Vector3(0,0,-45),Space.Self);
			newMenu.AttachedBuilding = building;
			newMenu.AttachedBuilding.Menu = newMenu;
			newMenu.SpawnButtons(building, UIManager);
            OpenMenu = newMenu;
            building.MenuOpen = true;
			UIManager.CreateTowerMenuCloser (delegate {
				RaycastHit towerHit;
				if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out towerHit)){
					if(towerHit.collider.tag=="Tower" || towerHit.collider.tag=="Shrine"){
						if(towerHit.collider.gameObject.GetComponent<MageAssignableBuilding>().InsideMage!=null){
							if(towerHit.collider.gameObject==building.gameObject){
                                CloseMenu(building);
                            }
                            else{
								building=towerHit.collider.gameObject.GetComponent<MageAssignableBuilding>();
							}
						}else{
                            CloseMenu(building);
                        }
					}else{
						CloseMenu(building);
					}
				}
			});
		}

	    private void CloseMenu(MageAssignableBuilding building)
	    {
	        UIManager.DestroyTowerMenuCloser();
	        OpenMenu.AttachedBuilding.HideRangeObject();
	        OpenMenu.AttachedBuilding.StopHighlighting();
	        building.MenuOpen = false;
	        building.Menu = null;
	        OpenMenu = null;
	        building.InsideMage.Data.ProfileButton.GetComponent<Toggle>().isOn = false;
	    }
	}
}