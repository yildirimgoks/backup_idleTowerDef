using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Manager;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
    public class Mage : MonoBehaviour
    {
        private const double DEFAULTMULTIPLIER = 1.0;
        public TowerSpell TowerSpellPrefab;
        public MageData Data;
        public Animator animator;
        private float _spellTime;

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
            Player = Camera.main.GetComponent<Player>();
            SelectRandomAnimation();
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
				    var pos = _building.transform.position;
				    pos.y = 20;
					Spell.Clone(ElementController.Instance.GetParticle(Data.GetElement()), Data.GetSpellData(), pos, FindFirstMinion(), damageMultiplier);
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
            if (Data.IsIdle() && !_building){
                animator.SetTrigger("MouseDown");
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

        private void OnMouseUp()
        {
            if (Data.IsDragged())
            {
                SelectRandomAnimation();
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
                var shrine = building as Shrine;
                if (shrine)
                {
                    _building.options[1].function = delegate {
                        Player.SkillCall(this);
                    };
                    //_building.options[1].sprite=skillSprite
                }           //putting skill in options[]
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

        public void Eject(){
			if (_building && _building.IsOccupied()) {
                SelectRandomAnimation();
                transform.position = _basePosition;
                Data.SetState(MageState.Idle);
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
				Player.Data.IncreaseCurrency(Data.GetIdleCurrency());
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
    }
}