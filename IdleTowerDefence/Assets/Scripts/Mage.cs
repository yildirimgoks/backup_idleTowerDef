using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Manager;
using Assets.Scripts.Model;
using Assets.Scripts.UI;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class Mage : MonoBehaviour
    {
        private const double DEFAULTMULTIPLIER = 1.0;
        public TowerSpell TowerSpellPrefab;
        public MageData Data;
        public int PrefabId;
        private Animator _animator;
        private float _spellTime;
        private bool _isCalling;
        // private float _cooldown;
		public float CooldownStart;

        //Drag & Drop
        private Vector3 _screenPoint;
        private Vector3 _offset;
        private Vector3 _basePosition;
        private Quaternion _baseRotation;

        private const float DragHeight = 30;

        public LayerMask MageDropMask;
        public LayerMask FloorMask;

        private MageAssignableBuilding _building;

        public Player Player;
        private bool _isHighlightOn;

        private double damageMultiplier = DEFAULTMULTIPLIER;
        private double rangeMultiplier = DEFAULTMULTIPLIER;
        private double delayMultiplier = DEFAULTMULTIPLIER;
        private float damageChangeTime = 0f;
        private float rangeChangeTime = 0f;
        private float delayChangeTime = 0f;
        private Vector3 originalSize;


		public ActionWithEvent[] upgradeActions;
		private float _clickTime;
		private bool _startedUpgrading;
        private float _lastUpgradeTime;
        private readonly float _autoUpgradeInterval = 0.1f;

        public void Initialize(Player player)
        {
            originalSize = this.transform.localScale;
            Player = player;
            if (Data == null)
            {
                var randomElement = MageFactory.GetRandomElement();
                Data = new MageData(MageFactory.GetRandomName(), MageFactory.GetRandomLine(), randomElement, Player.GetElementBonus(randomElement));
                Data.SetState(MageState.Idle);
            }
            _isCalling = false;
            CooldownStart = -100;
            _animator = GetComponent<Animator>();
            _basePosition = transform.position;
            _baseRotation = transform.rotation;
			StartCoroutine (GenerateCurrency());

            _isHighlightOn = false;
            StartAnimation();
			_startedUpgrading = false;
            Data.UpdateDps();
            enabled = true;
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateTimers();
            //Incase of animation rotations
            transform.rotation = _baseRotation;
            if ( !Data.IsDragged() && ( !_animator.GetCurrentAnimatorStateInfo(0).IsName("Havalan") && 
                                        !_animator.GetCurrentAnimatorStateInfo(0).IsName("Havalan 0") && 
                                        !_animator.GetCurrentAnimatorStateInfo(0).IsName("Havalan 1"))){
                transform.position = _basePosition;
            }

            var tower = _building as Tower;
            //var _shrine = _building as Shrine;

            // Cast spell with delay
            if (tower && Data.IsActive() && Time.time > _spellTime)
            {
                var minionToHit = FindFirstMinion();
                if(minionToHit && Time.timeScale != 0)
                { 
                    _spellTime = Data.NextSpellTime() + (Data.GetDelay() * ((float)delayMultiplier-1));
				    var pos = _building.transform.Find("SpellSpawn").transform.position;
				    //pos.y = 20;
					Spell.Clone(Player, ElementController.Instance.GetParticle(Data.GetElement()), Data.GetSpellData(), pos, minionToHit, this, damageMultiplier);
                    Player._audioManager.PlaySpellCastingSound(Data.GetElement());
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

            if (Data.GetPrefabId() != PrefabId)
            {
                Player.UpdateMagePrefab(this);
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
			if (Player.MageButtons.MageMenuOpen) {
                Player.MageButtons.CloseMageButtonsMenu();
			} else {
                Player.MageButtons.gameObject.GetComponent<ToggleGroup> ().SetAllTogglesOff ();
			}
			_clickTime = Time.time;

            if (Data.IsIdle() && !_building){
                _animator.SetTrigger("MouseDown");
                _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

                _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
                Data.SetState(MageState.Dragged);
				SetBuildingActive(false);
            }     
        }

        private void DragMageWithMouse()
        {
            this.transform.localScale = originalSize * 2;
            var curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
            var screenRay = Camera.main.ScreenPointToRay(curScreenPoint);

            foreach (var building in Player.GetSceneReferenceManager().AllAssignableBuildings)
            {
                if (building.InsideMage == null)
                {
                    building.StartHighlighting();
                    building.Slot.SetActive(true);
                }
            }

            RaycastHit distance;

            Physics.Raycast(screenRay, out distance, Mathf.Infinity, FloorMask);
            transform.position = screenRay.GetPoint(distance.distance - DragHeight) + _offset;
        }

        private void OnMouseUp()
        {
			if (Time.time - _clickTime < 0.25) {
                SetHightlighActive(_isHighlightOn);
                Data.ProfileButton.GetComponent<Toggle> ().isOn=!Data.ProfileButton.GetComponent<Toggle> ().isOn;
			}

            if (Data.IsDragged())
            {
                ReleaseDraggedMage();
            }
        }

        private void ReleaseDraggedMage()
        {
            this.transform.localScale = originalSize;
            StartAnimation();
            Data.SetState(MageState.Idle);
            StartCoroutine(GenerateCurrency());
            RaycastHit hitObject;
            var hit = Physics.Raycast(transform.position, Camera.main.transform.forward,
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

            foreach (var building in Player.GetSceneReferenceManager().AllAssignableBuildings)
            {
                building.Slot.SetActive(false);
                building.StopHighlighting();
            }
        }

        public void PutIntoBuilding(MageAssignableBuilding building)
        {
            if (building.SetMageInside(this))
            {
                AssignActions();
                Data.OccupyBuilding(building.GetId());
                Data.SetState(MageState.Active);
                _building = building;
                SetBuildingActive(true);
                if ( _isHighlightOn ){
                    SetHightlighActive(false);
                    // _building.StartHighlighting(ElementController.Instance.GetColor(this.Data.GetElement()));
                    _building.StartHighlighting();
                    _building.DisplayRangeObject();
                    Player.BuildingMenuSpawner.SpawnMenu(_building);
                }
				if (Player == null) {
					Player = Camera.main.GetComponent<Player> ();
				}

				_building.options [1].actions = upgradeActions;

                var shrine = building as Shrine;
                if (shrine)
                {

                    ActionWithEvent skillAction = new ActionWithEvent();
                    skillAction.Function = delegate {
                        if ( this.CanCast() ){
                            _isCalling = true;
                            Player.SkillCall(this);
                        }
				    };
                    skillAction.TriggerType = EventTriggerType.PointerDown;
                    _building.options[2].actions[0] = skillAction;

                    ActionWithEvent skillAction2 = new ActionWithEvent();
                    skillAction2.Function = delegate {
                        if ( this.CanCast() && _isCalling ){
                            _isCalling = false;
                            Player.CastSkill();
                            var Button=Player.BuildingMenuSpawner.OpenMenu.GetButton(2);
						    Button.GetComponent<CoolDown>().Cooldown(ElementController.Instance.GetElementSkillCooldown(Data.GetElement()), Time.time);
						    CooldownStart=Time.time;
                        }
						//_building.Menu.CloseMenu(_building.Menu);
				    };
                    skillAction2.TriggerType = EventTriggerType.PointerUp;
                    _building.options[2].actions[1] = skillAction2;

                    //_building.options[2].sprite=skillSprite
           		    //putting skill in options[]
				}

                if (building.MenuOpen)
                {
                    building.Menu.OnInsideMagePrefabChanged();
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
            if (!_building || !_building.IsOccupied()) return;

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
                
            if (Data.ProfileButton.GetComponent<Toggle>().isOn && Player.MageButtons.MageMenuOpen) {
                SetHightlighActive(true);
            }
        }

		IEnumerator GenerateCurrency() {
		    while (Data.IsIdle())
		    {
				yield return new WaitForSeconds(1f);
				Player.IncreaseCurrency(Data.GetIdleCurrency(), this.transform.position);
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
		    if (!targetMinion.OnMap) return false;
		    var tower = _building as Tower;
		    if (!tower) return false;

		    var deltaX = tower.transform.position.x - targetMinion.transform.position.x;
		    var deltaZ = tower.transform.position.z - targetMinion.transform.position.z;

		    var distanceSq = deltaX*deltaX + deltaZ*deltaZ;
		    return Mathf.Sqrt(distanceSq) < (Data.GetSpellRange()*rangeMultiplier);
		}

        public void UpgradeMage()
        {
            if (Player.Data.GetCurrency() < Data.GetUpgradePrice()) return;
            Player.DecreaseCurrency(Data.GetUpgradePrice());
            Data.UpgradeMage();
            switch (Data.GetElement())
            {
                case Element.Air:
                    Player.AchievementManager.RegisterEvent(AchievementType.AirMage, Data.GetLevel()+1);
                    break;
                case Element.Fire:
                    Player.AchievementManager.RegisterEvent(AchievementType.FireMage, Data.GetLevel()+1);
                    break;
                case Element.Earth:
                    Player.AchievementManager.RegisterEvent(AchievementType.EarthMage, Data.GetLevel()+1);
                    break;
                case Element.Water:
                    Player.AchievementManager.RegisterEvent(AchievementType.WaterMage, Data.GetLevel()+1);
                    break;
                default:
                    break;
            }
            //Do not spam with events
            if ((Data.GetLevel() < 50 && Data.GetLevel() % 10 == 0) || Data.GetLevel() % 5 == 0)
            {
                AnalyticsManager.MageUpgraded(this);
            }
            if (_building != null)
            {
                _building.DisplayRangeObject();
                _building.StartHighlighting();
            }
        }

		public MageAssignableBuilding GetBuilding(){
			return _building;
		}

        private void StartAnimation(){
            _animator.SetTrigger("Initial");
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
            var elementSkillCooldown = ElementController.Instance.GetElementSkillCooldown(Data.GetElement());
            return CooldownStart + elementSkillCooldown < Time.time;
        }

        public float GetRange()
        {
            return (float)(Data.GetSpellRange() * rangeMultiplier);
        }

		public void AssignActions(){
			upgradeActions = new ActionWithEvent[3];

		    var upgradeAction1 = new ActionWithEvent
		    {
		        Function = delegate
		        {
		            _startedUpgrading = true;
		            _lastUpgradeTime = 0;
		        },
		        TriggerType = EventTriggerType.PointerDown
		    };

		    var upgradeAction2 = new ActionWithEvent
		    {
		        Function = delegate { _startedUpgrading = false; },
		        TriggerType = EventTriggerType.PointerUp
		    };

		    var upgradeAction3 = new ActionWithEvent
		    {
		        Function = delegate { UpgradeMage(); },
		        TriggerType = EventTriggerType.PointerClick
		    };

		    upgradeActions[0] = upgradeAction1;
			upgradeActions[1] = upgradeAction2;
			upgradeActions[2] = upgradeAction3;
		}

        public void SetHightlighActive(bool active)
        {
            if (!this) return;
            var color = Color.black;
            var size = .0f;
            if (active)
            {
                color = ElementController.Instance.GetColor(this.Data.GetElement());
                size = 0.01f;
            }

            if (gameObject.GetComponent<Renderer>())
            {
                var r = gameObject.GetComponent<Renderer>();
                r.material.SetColor("_MainColor", color);
                r.material.SetFloat("_Dist", size);
            }
            else
            {
                foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
                {
                    if (r.name != "Slot")
                    {
                        r.material.SetColor("_MainColor", color);
                        r.material.SetFloat("_Dist", size);
                    }
                }
            }
            _isHighlightOn = active;
        }
    }
}