using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class BuildingMenu : MonoBehaviour {

		public QkButton ButtonPrefab;
		public MageAssignableBuilding AttachedBuilding;

        private UIManager _uiManager;
        private QkButton[] _buttonList;

		void Start () {
			_uiManager = Camera.main.GetComponent<UIManager>();
		}

		// Use this for initialization
		public void SpawnButtons (MageAssignableBuilding building) {
            AttachedBuilding = building;
			_buttonList = new QkButton[building.options.Length];

			for (var i = 0; i < _buttonList.Length; i++) {
				var newButton = Instantiate(ButtonPrefab);

                newButton.transform.SetParent(transform, false);
                var theta = (2 * Mathf.PI / building.options.Length) * i;

                newButton.Init(theta, building.options[i]);

                _buttonList[i] = newButton;
                
                //First button is always close button!
                if (i==0) {
				    newButton.GetButton().onClick.AddListener(
                        delegate {
						    CloseMenu(this);
                        }
					);
				}
                //Third button always have cooldown
				if (i == 2) {
                    newButton.GetComponent<CoolDown>().enabled = true;
                    if (!AttachedBuilding.InsideMage.CanCast()) {
						newButton.GetComponent<CoolDown>().Cooldown(ElementController.Instance.GetElementSkillCooldown(AttachedBuilding.InsideMage.Data.GetElement()), AttachedBuilding.InsideMage.CooldownStart);
					}
				}
			}
            AttachedBuilding.DisplayRangeObject();
		}

	    public void OnInsideMagePrefabChanged()
	    {
	        for (var i = 0; i < _buttonList.Length; i++)
	        {
                _buttonList[i].SetButtonActions(AttachedBuilding.options[i]);
	        }
	    }

	    public QkButton GetButton(int _buttonIndex){
			return _buttonList[_buttonIndex];
		}

		public void CloseMenu(BuildingMenu menu){
            menu.AttachedBuilding.HideRangeObject();
			menu.AttachedBuilding.MenuOpen = false;
			_uiManager.DestroyTowerMenuCloser();
		}

		void Update(){
		    if (!AttachedBuilding.MenuOpen)
		    {
		        AttachedBuilding.Menu = null;

		        AttachedBuilding.StopHighlighting();
		        for (var i = 0; i < _buttonList.Length; i++)
		        {
		            Destroy(_buttonList[i].gameObject);
		        }
		        Destroy(gameObject);
		    }
		    else
		    {
		        if (_buttonList[1])
		        {
		            _buttonList[1].GetButton().interactable =
		                !(_uiManager.Player.Data.GetCurrency() < AttachedBuilding.InsideMage.Data.GetUpgradePrice());
		        }

		        if (_buttonList.Length == 3 && _buttonList[2])
		        {
		            _buttonList[2].GetButton().interactable = !_buttonList[2].GetComponent<CoolDown>().IsCoolDown;
		        }
		    }
		}
	}
}
