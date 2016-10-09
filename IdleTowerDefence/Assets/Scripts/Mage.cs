using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Manager;
using Assets.Scripts.Model;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class Mage : MonoBehaviour
    {
        private const double DEFAULTMULTIPLIER = 1.0;
        public TowerSpell TowerSpellPrefab;
        public MageData Data;
        public Animator animator;
        private float _spellTime;

        private float _cooldown;

        //Drag & Drop
        private Vector3 _screenPoint;
        private Vector3 _offset;
        private Vector3 _basePosition;
        private Quaternion _baseRotation;

        public float DragHeight;

        public LayerMask MageDropMask;
        public LayerMask FloorMask;

        private MageAssignableBuilding _building;
		public GameObject ProfileButton;
		public int ProfileButtonIndex;

        public Player Player;
		public Behaviour Highlight;

        private double damageMultiplier = DEFAULTMULTIPLIER;
        private double rangeMultiplier = DEFAULTMULTIPLIER;
        private double delayMultiplier = DEFAULTMULTIPLIER;
        private float damageChangeTime = 0f;
        private float rangeChangeTime = 0f;
        private float delayChangeTime = 0f;

		public ActionWithEvent[] upgradeActions;
		private float clickTime;
		private bool _startedUpgrading;
        private float _lastUpgradeTime;
        private readonly float _autoUpgradeInterval = 0.1f;

        private AudioManager _audioManager;

        // Use this for initialization
        private void Start()
        {

            if (Data == null)
            {
                Data = new MageData(MageFactory.GetRandomName(), MageFactory.GetRandomLine(), MageFactory.GetRandomElement());
                Data.SetState(MageState.Idle);
            }
            animator = this.GetComponent<Animator>();
            _basePosition = transform.position;
            _baseRotation = transform.rotation;
			StartCoroutine (GenerateCurrency());
            Highlight = (Behaviour)GetComponent("Halo");
			if (Player == null) {
				Player = Camera.main.GetComponent<Player> ();
			}
            StartAnimation();
            _audioManager = Camera.main.GetComponent<AudioManager>();
			_startedUpgrading = false;

			AssignActions ();
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateTimers();
            //Incase of animation rotations
            transform.rotation = _baseRotation;
            if ( !Data.IsDragged() && ( !animator.GetCurrentAnimatorStateInfo(0).IsName("Havalan") && 
                                        !animator.GetCurrentAnimatorStateInfo(0).IsName("Havalan 0") && 
                                        !animator.GetCurrentAnimatorStateInfo(0).IsName("Havalan 1"))){
                transform.position = _basePosition;
            }

            var _tower = _building as Tower;
            var _shrine = _building as Shrine;

            // Cast spell with delay
            if (_tower && Data.IsActive() && Time.time > _spellTime)
            {
                var minionToHit = FindFirstMinion();
                if(minionToHit && Time.timeScale != 0)
                { 
                    _spellTime = Data.NextSpellTime() + (Data.GetDelay() * ((float)delayMultiplier-1));
				    var pos = _building.transform.Find("SpellSpawn").transform.position;
				    //pos.y = 20;
					Spell.Clone(ElementController.Instance.GetParticle(Data.GetElement()), Data.GetSpellData(), pos, FindFirstMinion(), this, damageMultiplier);
                    _audioManager.PlaySpellCastingSound(Data.GetElement());
				}
            }

            if (_startedUpgrading)
            {
                if (_lastUpgradeTime > _autoUpgradeInterval)
                {
                    _lastUpgradeTime = 0;
                    UpgradeMage();
                }
                else
                {
                    _lastUpgradeTime += Time.deltaTime;
                }
            }

            if (Data.IsDragged())
            {
                if (Input.GetMouseButtonUp(0))
                {
                    ReleaseDraggedMage();
                }
                else
                {
                    DragMageWithMouse();
                }
            }
        }

        private void UpdateTimers(){
            if ( damageChangeTime <= 0){
                damageMultiplier = DEFAULTMULTIPLIER;
            }else{
                damageChangeTime -= Time.deltaTime;
            }

            if ( rangeChangeTime <= 0){
                rangeMultiplier = DEFAULTMULTIPLIER;
            }else{
                rangeChangeTime -= Time.deltaTime;
            }

            if ( delayChangeTime <= 0){
                delayMultiplier = DEFAULTMULTIPLIER;
            }else{
                delayChangeTime -= Time.deltaTime;
            }
        }

		public static Mage Clone(Mage magePrefab, MageData data, Vector3 position, Quaternion rotation)
        {
            var mage = (Mage)Instantiate(magePrefab, position, Quaternion.Euler(0,90,0));
            mage.Data = data;
			foreach (var r in mage.gameObject.GetComponentsInChildren<Renderer>())
			{
				if (r.name.Contains ("Body")) {
					r.material.mainTexture = ElementController.Instance.GetMage (mage.Data.GetElement ())[0];
				} else {
					r.material.mainTexture = ElementController.Instance.GetMage (mage.Data.GetElement ())[1];
				}
			}            
			return mage;
        }

        private void OnMouseDown()
        {
			clickTime = Time.time;

            if (Data.IsIdle() && !_building){
                animator.SetTrigger("MouseDown");
                _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

                _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
                Data.SetState(MageState.Dragged);
				SetBuildingActive(false);
            }     
        }

        private void DragMageWithMouse()
        {
            var curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
            var screenRay = Camera.main.ScreenPointToRay(curScreenPoint);

            foreach (var building in Player.AllAssignableBuildings)
            {
                if (building.InsideMage == null)
                {
                    building.Slot.SetActive(true);
                }
            }

            RaycastHit distance;

            Physics.Raycast(screenRay, out distance, Mathf.Infinity, FloorMask);
            transform.position = screenRay.GetPoint(distance.distance - DragHeight) + _offset;
        }

        private void OnMouseUp()
        {
			if (Time.time - clickTime < 0.25) {
				Highlight.enabled = !Highlight.enabled;
				ProfileButton.GetComponent<Toggle> ().isOn=!ProfileButton.GetComponent<Toggle> ().isOn;
			}

            if (Data.IsDragged())
            {
                ReleaseDraggedMage();
            }
        }

        private void ReleaseDraggedMage()
        {
            StartAnimation();
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
                    PutIntoBuilding(building);
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

            foreach (var building in Player.AllAssignableBuildings)
            {
                building.Slot.SetActive(false);
            }
        }

        public void PutIntoBuilding(MageAssignableBuilding building)
        {
            if (building.SetMageInside(this))
            {
                Data.OccupyBuilding(building.GetId());
                Data.SetState(MageState.Active);
                _building = building;
                SetBuildingActive(true);
                if (Highlight != null && Highlight.enabled)
                {
                    Highlight.enabled = false;
                    _building.Highlight.enabled = true;
                }

				if (Player == null) {
					Player = Camera.main.GetComponent<Player> ();
				}


				_building.options [1].actions = upgradeActions;

                var shrine = building as Shrine;
                if (shrine)
                {

                    ActionWithEvent skillAction = new ActionWithEvent();
                    skillAction.function = delegate {
				    	Player.SkillCall(this);
				    };
                    skillAction.triggerType = EventTriggerType.PointerDown;
                    _building.options[2].actions[0] = skillAction;

                    ActionWithEvent skillAction2 = new ActionWithEvent();
                    skillAction2.function = delegate {
				    	Player.CastSkill();
                        _building.Menu.CloseMenu(_building.Menu);
				    };
                    skillAction2.triggerType = EventTriggerType.PointerUp;
                    _building.options[2].actions[1] = skillAction2;
                    //_building.options[2].sprite=skillSprite
           		    //putting skill in options[]
				}
            }
            else
            {
                transform.position = _basePosition;
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

        public void Eject(bool withDrag){
			if (_building && _building.IsOccupied()) {
                StartAnimation();
                transform.position = _basePosition;
			    if (withDrag)
			    {
                    Data.SetState(MageState.Dragged);
			    }
			    else
			    {
			        Data.SetState(MageState.Idle);
			    }
			    _building.EjectMageInside();
                SetBuildingActive(false);
                _building = null;
                Data.EjectFromOccupiedBuilding();
                StartCoroutine(GenerateCurrency());
                
				if (ProfileButton.GetComponent<Toggle>().isOn && MageButtons.Instance.MageMenuOpen) {
					Highlight.enabled = true;
				}
            }
		}

		IEnumerator GenerateCurrency() {
		    while (Data.IsIdle())
		    {
				yield return new WaitForSeconds(1f);
				Player.IncreaseCurrency(Data.GetIdleCurrency());
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
		    return Mathf.Sqrt(distanceSq) < (Data.GetSpellRange()*rangeMultiplier);
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

        private void StartAnimation(){
            animator.SetTrigger("Initial");
        }

        private void SelectRandomAnimation(){
            int rand = Random.Range(1,3);
            animator.SetTrigger("Animation"+rand.ToString());
        }

        public void SetBasePosition(Vector3 pos){
            _basePosition = pos;
            transform.position = _basePosition;
        }

        public bool ChangeDamage(double multiplier){
            damageMultiplier = DEFAULTMULTIPLIER * multiplier;
            damageChangeTime = 5f;
            return true;
        }

        public bool ChangeRange(double multiplier){
            rangeMultiplier = DEFAULTMULTIPLIER * multiplier;
            rangeChangeTime = 5f;
            return true;
        }

        public bool ChangeDelay(double multiplier){
            delayMultiplier = DEFAULTMULTIPLIER * multiplier;
            Debug.Log(delayMultiplier);
            delayChangeTime = 5f;
            return true;
        }

        public bool CanCast()
        {
            if(_cooldown < Time.time)
            {
                _cooldown = Time.time + ElementController.Instance.GetElementSkillCooldown(Data.GetElement());
                return true;
            } else
            {
                return false;
            }
        }

        public float GetRange()
        {
            return (float)(Data.GetSpellRange() * rangeMultiplier);
        }

		public void AssignActions(){
			upgradeActions = new ActionWithEvent[3];

			ActionWithEvent upgradeAction1 = new ActionWithEvent();
			upgradeAction1.function = delegate
			{
				_startedUpgrading = true;
				_lastUpgradeTime = 0;
			};
			upgradeAction1.triggerType = EventTriggerType.PointerDown;

			ActionWithEvent upgradeAction2 = new ActionWithEvent();
			upgradeAction2.function = delegate {
				_startedUpgrading = false;
			};
			upgradeAction2.triggerType = EventTriggerType.PointerUp;

			upgradeActions[0] = upgradeAction1;
			upgradeActions[1] = upgradeAction2;
		}
    }
}