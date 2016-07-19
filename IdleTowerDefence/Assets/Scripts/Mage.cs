using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
    public class Mage : MonoBehaviour
    {
        public TowerSpell TowerSpellPrefab;
        public MageData Data;
        private float _spellTime;
        public Texture2D skill_aim_cursor;
        Player _player;	// altta da var?

        //Drag & Drop
        private Vector3 _screenPoint;
        private Vector3 _offset;
        private Vector3 _basePosition;

        public float DragHeight;

        public LayerMask MageDropMask;
        public LayerMask FloorMask;

        private MageAssignableBuilding _building;
		public GameObject ProfileButton;
        
        public Player Player;	//üstte de var?
		public Behaviour Highlight;

        // Use this for initialization
        private void Start()
        {
            if (Data == null)
            {
                Data = new MageData(MageFactory.GetRandomName(), MageFactory.GetRandomLine(), MageFactory.GetRandomElement());
            }
            _basePosition = transform.position;
			StartCoroutine (GenerateCurrency());
            Highlight = (Behaviour)GetComponent("Halo");
            Player = Camera.main.GetComponent<Player>();
        }

        // Update is called once per frame
        private void Update()
        {
            var _tower = _building as Tower;
            var _shrine = _building as Shrine;

            // Cast spell with delay
            if (_tower && Data.IsActive() && Time.time > _spellTime)
            {
                var minionToHit = FindFirstMinion();
                if(minionToHit && Time.timeScale != 0)
                { 
                    _spellTime = Data.NextSpellTime();
				    var pos = _building.transform.position;
				    pos.y = 20;
					Spell.Clone(TowerSpellPrefab, Data.GetSpellData(), pos, FindFirstMinion());
				}
            }
            // Temporary bug fix
            if (Time.time < 1.0f)
            {
                Data.SetState(MageState.Idle);
            }
        }

        public static Mage Clone(Mage magePrefab, MageData data, Vector3 position, Quaternion rotation)
        {
            var mage = (Mage)Instantiate(magePrefab, position, Quaternion.Euler(0,90,0));
            mage.Data = data;
            return mage;
        }

        private void OnMouseDown()
        {
            if (Data.IsIdle() && !_building){
                gameObject.GetComponent<Animator>().SetTrigger("MouseDown");
                _basePosition = transform.position;
                _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

                _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
                Data.SetState(MageState.Dragged);
				SetBuildingActive(false);
            }     
        }

        private void OnMouseDrag()
        {
            if (Data.IsDragged())
            {
                var curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
                var screenRay = Camera.main.ScreenPointToRay(curScreenPoint);
                
                RaycastHit distance;

                Physics.Raycast(screenRay, out distance, Mathf.Infinity, FloorMask);
                transform.position = screenRay.GetPoint(distance.distance-DragHeight) + _offset;
            }  
        }
        IEnumerator UseSkill() {
            bool clicked = false;
            if (clicked=true) {
                Cursor.SetCursor(skill_aim_cursor, Vector2.zero, CursorMode.Auto);
            }
            if (Input.GetMouseButtonDown(0)) {
                clicked = true;
                _player.TemporarySkillCall();
            }
            yield return null;
        }
        private void OnMouseUp()
        {
            if (Data.IsDragged())
            {
                gameObject.GetComponent<Animator>().SetTrigger("MouseUp");
                Data.SetState(MageState.Idle);
                StartCoroutine(GenerateCurrency());
                RaycastHit hitObject;
                var hit = Physics.Raycast(Camera.main.transform.position, transform.position - Camera.main.transform.position,
                out hitObject, Mathf.Infinity, MageDropMask);
                if (hit)
                {
                    if (hitObject.collider.gameObject.tag.Equals("Tower") || hitObject.collider.gameObject.tag.Equals("Shrine"))
                    {
                        var building = hitObject.collider.gameObject.GetComponent<MageAssignableBuilding>();
                        if (building.SetMageInside(this))
                        {
                            _building = building;
                            Data.SetState(MageState.Active);
                            SetBuildingActive(true);
							if (Highlight.enabled) {
								Highlight.enabled = false;
								_building.Highlight.enabled = true;
							}

                            if (hitObject.collider.gameObject.tag.Equals("Shrine")) {
                                _building.options[1].function = delegate {
                                    StartCoroutine(UseSkill());     //ya da herneyse
                                };
								//_building.options[1].sprite=skillSprite
							}			//putting skill in options[]
                        }
                        else
                        {
                            transform.position = _basePosition;
                        }
                    }
                    else
                    {
                        SetBuildingActive(false);
                        _building = null;
                    }
                }
                else
                {
                    transform.position = _basePosition;
                }
            }
        }

		private void SetBuildingActive(bool active)
        {
		    if (!_building) return;

		    gameObject.GetComponent<Collider>().enabled = !active;
		    foreach (var r in GetComponentsInChildren<Renderer>())
		    {
		        r.enabled = !active;
		    }
        }

        public void Eject(){
			if (_building && _building.IsOccupied()) {
                transform.position = _basePosition;
                Data.SetState(MageState.Idle);
			    _building.EjectMageInside();
                SetBuildingActive(false);
                _building = null;
                StartCoroutine(GenerateCurrency());
				if (ProfileButton.GetComponent<Toggle>().isOn && MageButtons.Instance.MageMenuOpen) {
					Highlight.enabled = true;
				}
            }
		}

		IEnumerator GenerateCurrency() {
		    while (true)
		    {
				yield return new WaitForSeconds(1f);
				if (!Data.IsIdle()) {
					break;
				}
				Player.Data.IncreaseCurrency(3);
			}
		}
        
		// Find leader minion
		public Minion FindFirstMinion()
		{
            if ( Player.WaveManager.AliveMinionCount <= 0 ) return null;
			var minions = Player.WaveManager.GetMinionList();
			var target = minions.First();
			var index = 1;
			while (!InRange(target))
			{
				if (index >= minions.Count)
				{
					return null;
				}
				target = minions.ElementAt(index);
				index++;
			}
			return target;
		}

		public bool InRange(Minion targetMinion)
		{
		    var _tower = _building as Tower;
		    if (!_tower) return false;

		    var deltaX = _tower.transform.position.x - targetMinion.transform.position.x;
		    var deltaZ = _tower.transform.position.z - targetMinion.transform.position.z;

		    var distanceSq = deltaX*deltaX + deltaZ*deltaZ;
		    return Mathf.Sqrt(distanceSq) < Data.GetSpellRange();
		}

        public void UpgradeMage()
        {
            if (Player.Data.GetCurrency() < Data.GetUpgradePrice()) return;
            Player.Data.DecreaseCurrency(Data.GetUpgradePrice());
            Data.UpgradeMage();
        }

		public MageAssignableBuilding GetBuilding(){
			return _building;
		}
    }
}