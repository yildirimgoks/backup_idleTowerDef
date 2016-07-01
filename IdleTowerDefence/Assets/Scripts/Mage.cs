using System.Linq;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
    public class Mage : MonoBehaviour
    {
        public TowerSpell TowerSpellPrefab;
        public MageData Data;
        private float _spellTime;

        //Drag & Drop
        private Vector3 _screenPoint;
        private Vector3 _offset;
        private Vector3 _basePosition;

        public float DragHeight;

        public LayerMask MageDropMask;
        public LayerMask FloorMask;

        private Tower _tower;
        private Shrine _shrine;

        public Player Player;
		public Behaviour Highlight;

        // Use this for initialization
        private void Start()
        {
            if (Data == null)
            {
                Data = new MageData(MageFactory.GetRandomName(), MageFactory.GetRandomLine(), Element.Air);
            }
            _basePosition = transform.position;
			StartCoroutine (GenerateCurrency());
            Highlight = (Behaviour)GetComponent("Halo");
            Player = Camera.main.GetComponent<Player>();
        }

        // Update is called once per frame
        private void Update()
        {
            // Cast spell with delay
            if (Data.IsActive() && Time.time > _spellTime)
            {
                _spellTime = Data.NextSpellTime();
				if (Time.timeScale != 0) {
					Spell.Clone(TowerSpellPrefab, Data.GetSpellData(), _tower.transform.position, FindFirstMinion());
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
            var mage = (Mage)Instantiate(magePrefab, position, Quaternion.identity);
            mage.Data = data;
            return mage;
        }

        private void OnMouseDown()
        {
            if (Data.IsIdle() && !_tower){
                _basePosition = transform.position;
                _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

                _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
                Data.SetState(MageState.Dragged);
				SetTowerActive(false);
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
                Data.SetState(MageState.Idle);
                RaycastHit hitObject;
                var hit = Physics.Raycast(Camera.main.transform.position, transform.position - Camera.main.transform.position,
                out hitObject, Mathf.Infinity, MageDropMask);
                if (hit)
                {
                    if (hitObject.collider.gameObject.tag.Equals("Tower"))
                    {
                        var tower = hitObject.collider.gameObject.GetComponent<Tower>();
                        if (!tower.Occupied)
                        {
                            _tower = tower;
                            _tower.InsideMage = this;
                            foreach (var r in _tower.gameObject.GetComponentsInChildren<Renderer>())
                            {
								r.material.mainTexture = ElementController.Instance.GetTower(Data.GetElement());
                            }
                            Data.SetState(MageState.Active);
                        }
                        else
                        {
                            transform.position = _basePosition;
                        }
                        SetTowerActive(true);
                    }
                    else if (hitObject.collider.gameObject.tag.Equals("Shrine"))
                    {
                        var shrine = hitObject.collider.gameObject.GetComponent<Shrine>();
                        if (!shrine.Occupied)
                        {
                            _shrine = shrine;
                            _shrine.InsideMage = this;
                        }
                        else
                        {
                            transform.position = _basePosition;
                        }
                    }
                    else
                    {
                        SetTowerActive(false);
                        _tower = null;
                    }
                }
                else
                {
                    transform.position = _basePosition;
                    SetTowerActive(true);
                }

            } else if (Data.IsDropped())
            {
                WaveManager wavemanager = GameObject.Find("Main Camera").GetComponent<WaveManager>();
                transform.position = new Vector3(6.1f, 12.2f, 21f + (wavemanager.CurrentWave / 5 - 1) * 4f);
                Data.SetState(MageState.Idle);
                Time.timeScale = 1;
            }
        }

		private void SetTowerActive(bool active)
        {
            if (_tower)
            {
                _tower.Occupied = active;
                gameObject.GetComponent<Collider>().enabled = !active;
                foreach (var r in GetComponentsInChildren<Renderer>())
                {
                    r.enabled = !active;
                }
            }
        }

		public void Eject(){
			if (_tower && _tower.Occupied) {
                _tower.InsideMage.transform.position =_tower.InsideMage._basePosition;
                Data.SetState(MageState.Idle);
                _tower.InsideMage = null;
				foreach (var r in _tower.gameObject.GetComponentsInChildren<Renderer>()) {
					r.material.mainTexture = ElementController.Instance.textures[0];
				}
				_tower.Occupied = false;
                SetTowerActive (false);
                _tower = null;
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
    }
}