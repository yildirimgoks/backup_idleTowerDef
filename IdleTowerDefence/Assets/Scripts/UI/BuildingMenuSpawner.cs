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
            var screenSize = gameObject.GetComponent<RectTransform>().sizeDelta;
            var posX = Camera.main.WorldToViewportPoint(building.transform.position).x * screenSize.x - screenSize.x / 2;
            var posY = Camera.main.WorldToViewportPoint(building.transform.position).y * screenSize.y - screenSize.y / 2 + 80;
            newMenu.transform.localPosition = new Vector3(posX, posY, 0);
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

        public void CloseCurrentTowerMenu()
        {
            if (OpenMenu)
            {
                CloseMenu(OpenMenu.AttachedBuilding);
            }
        }
	}
}