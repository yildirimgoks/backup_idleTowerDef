using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
    public class Mage : MonoBehaviour
    {

		public static string[] NameList = { "Gandalf the Magenta", "Dumblebee", "Hayri", "Merlin", "İzzet", "Longbottom" };
		public static string[] LineList = {
			"Do a barrel roll, you fools!",
			"Winter is coming.",
			"Say what?",
			"Hellööööö!",
			"I am your father!",
			"Kanka ben de hiç çalışmadım boşver"
		};
		public string Name;
		public string Line;

        public TowerSpell TowerSpellPrefab;
		private BigIntWithUnit SpellDamage = 20;	
		private int SpellSpeed = 70;
		private int SpellRange = 10;

		public Element Element;

        public float Delay;
        private float _spellTime;
		private bool _isIdle = true;

        public bool Active;

        //Drag & Drop
        private Vector3 _screenPoint;
        private Vector3 _offset;
        private Vector3 _basePosition;

        public float DragHeight;

        public LayerMask MageDropMask;
        public LayerMask FloorMask;

        private bool _dragged;

        private Tower _tower;
        private Shrine _shrine;
        
        public bool Dropped;

        private int _mageLvl;
        private BigIntWithUnit _upgradePrice;
        public double DamageMultiplier, RangeMultiplier, RateMultiplier;

        public Player _player;

		public Behaviour highlight;

        private int maxRange = 30;
        private float minDelay = 0.1f;

        // Use this for initialization
        private void Start()
        {
			Name = NameList[Random.Range(0,NameList.Length)];
			Line = LineList [Random.Range (0, LineList.Length)];
            _basePosition = transform.position;
			StartCoroutine (GenerateCurrency());
            _mageLvl = 1;
            _upgradePrice = 100;
            DamageMultiplier = RangeMultiplier = RateMultiplier = 1;
            highlight = (Behaviour)GetComponent("Halo");
        }

        // Update is called once per frame
        private void Update()
        {
            // Cast spell with delay
            if (Active && Time.time > _spellTime)
            {
                _spellTime = Time.time + Delay;
				if (Time.timeScale != 0) {
					Spell.Clone(TowerSpellPrefab, SpellDamage, SpellSpeed, Element, _tower.transform.position, FindFirstMinion ());
				}
            }
        }

        private void OnMouseDown()
        {
            if (!Dropped && !_tower){
                _basePosition = transform.position;
                _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

                _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
                _dragged = true;
				SetTowerActive(false);
            }     
        }

        private void OnMouseDrag()
        {
            if (_dragged)
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
            if (_dragged)
            {
                _dragged = false;
                RaycastHit hitObject;
                var hit = Physics.Raycast(Camera.main.transform.position, transform.position - Camera.main.transform.position,
                out hitObject, Mathf.Infinity, MageDropMask);
                if (hit && hitObject.collider.gameObject.tag.Equals("Tower"))
                {
                    var tower = hitObject.collider.gameObject.GetComponent<Tower>();
                    if (!tower.Occupied)
                    {
                        _tower = tower;
						_tower.InsideMage = this;
						foreach (var r in _tower.gameObject.GetComponentsInChildren<Renderer>()) {
							r.material.mainTexture = _tower.textures [(int)_tower.InsideMage.Element];
						}
                        _isIdle = false;
                    }
                  
                    else
                    {
                        transform.position = _basePosition;
                        _isIdle = true;
                    }
                    SetTowerActive(true);
                    
                }

                else if (hit && hitObject.collider.gameObject.tag.Equals("Shrine"))
                {
                    var shrine = hitObject.collider.gameObject.GetComponent<Shrine>();
                    if (!shrine.Occupied) {
                        _shrine = shrine;
                        _shrine.InsideMage = this;
                        _isIdle = false;
                        
                    }
                    else
                    {
                        transform.position = _basePosition;
                        _isIdle = true;
                    }
                    
                } 

                else if (hit)
                {
                    SetTowerActive(false);
                    _tower = null;
					_isIdle = true;
                }
                else
                {
                    transform.position = _basePosition;
                    SetTowerActive(true);
					_isIdle = true;
                }

            }
            if (Dropped)
            {
                WaveManager wavemanager = GameObject.Find("Main Camera").GetComponent<WaveManager>();
                transform.position = new Vector3(6.1f, 12.2f, 21f + (wavemanager.CurrentWave / 5 - 1) * 4f);
                Dropped = false;
                Time.timeScale = 1;
            }
        }

		private void SetTowerActive(bool active)
        {
            if (_tower)
            {
                Active = active;
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
                _isIdle = true;
                _tower.InsideMage = null;
				foreach (var r in _tower.gameObject.GetComponentsInChildren<Renderer>()) {
					r.material.mainTexture = _tower.textures [0];
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
				if (!_isIdle) {
					break;
				}
				Camera.main.GetComponent<Player>().IncreaseCurrency(3);
			}
		}

		public void IncreaseSpellDamage(int increment){
			SpellDamage += increment;
		}

		public void IncreaseSpellRate(float delayDecrement){
			Delay /= delayDecrement;
		}

		public void IncreaseSpellRange(int increment){
			SpellRange += increment;
		}

		// Find leader minion
		public Minion FindFirstMinion()
		{
			var cam = GameObject.Find("Main Camera");
			var playerScript = cam.GetComponent<Player>();
			var minions = playerScript.WaveManager.GetMinionList();
			var target = minions.First<Minion>();
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
			var deltaX = transform.position.x - targetMinion.transform.position.x;
			var deltaZ = transform.position.z - targetMinion.transform.position.z;

			var distanceSq = deltaX * deltaX + deltaZ * deltaZ;
			return (Mathf.Sqrt(distanceSq) < SpellRange);
		}

        public BigIntWithUnit IndividualDPS()
        {
            return BigIntWithUnit.MultiplyPercent(SpellDamage, 100 / Delay);
        }

        public void UpgradeMage()
        {
            if (_player.GetCurrency() >= _upgradePrice)
            {
                _player.DecreaseCurrency(_upgradePrice);
                SpellDamage += (int) (20 * System.Math.Pow(DamageMultiplier, _mageLvl));
                SpellRange += (int) (2 * System.Math.Pow(RangeMultiplier, _mageLvl));
                Delay /= (float) (1.2f * System.Math.Pow(RateMultiplier, _mageLvl));
                if (SpellRange > maxRange)
                {
                    SpellRange = maxRange;
                }
                if (Delay < minDelay)
                {
                    Delay = minDelay;
                }
                _mageLvl++;
                _upgradePrice = BigIntWithUnit.MultiplyPercent(_upgradePrice, System.Math.Pow(1.1, _mageLvl) * 100);
            }
        }

		public string[] GetSpecs(){
			string[] Specs = new string[4];
			Specs[0]=_mageLvl.ToString();
			Specs[1]=SpellDamage.ToString();
			Specs[2]=(1/Delay).ToString();
			Specs[3]=SpellRange.ToString();
			return Specs;
		}

    }
}