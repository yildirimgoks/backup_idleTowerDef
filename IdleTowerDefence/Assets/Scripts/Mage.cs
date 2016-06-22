using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
    public class Mage : MonoBehaviour
    {

		public static string[] NameList = { "Gandalf the Magenta", "Dumblebee", "Hayri", "Merlin", "İzzet", "Longbottom" };
		public string Name;

        public TowerSpell TowerSpellPrefab;
		private BigIntWithUnit SpellDamage = 20;	
		private int SpellSpeed = 70;
		private int SpellRange = 10;

		public Element Element;

        public float Delay;
        private float _spellTime;
		private bool _isIdle = true;

        public bool Active;

        private Vector3 _screenPoint;
        private Vector3 _offset;

        private Vector3 _basePosition;
        private Tower _tower;
        private Shrine _shrine;

        private bool _dragged;

        public LayerMask MageDropMask;

        public bool Dropped;

        private int _mageLvl, _lvlDmg, _lvlRng, _lvlRate;
        public BigIntWithUnit _damagePrice, _rangePrice, _ratePrice;

        public Player _player;

        // Use this for initialization
        private void Start()
        {
			Name = NameList[Random.Range(0,NameList.Length)];
            _basePosition = transform.position;
			StartCoroutine (GenerateCurrency());
            _lvlDmg = _lvlRng = _lvlRate = 1;
            _damagePrice = _rangePrice = _ratePrice = 100;
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
                Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;
                transform.position = curPosition;
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
                foreach (Renderer r in GetComponentsInChildren(typeof(Renderer)))
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

        public BigIntWithUnit individualDPS()
        {
            return BigIntWithUnit.MultiplyPercent(SpellDamage, 100 / Delay);
        }

        public void upgradeDamage()
        {
            if (_player.getCurrency() >= _damagePrice)
            {
                _player.DecreaseCurrency(_damagePrice);
                SpellDamage += 20;
                _lvlDmg++;
                _damagePrice = calcPrice(_damagePrice, 0);
				Debug.Log ("Damage of " + Name + " has upgraded.");
            }                
        }

        public void upgradeRange()
        {
            if (_player.getCurrency() >= _rangePrice)
            {
                _player.DecreaseCurrency(_rangePrice);
                SpellRange += 2;
                _lvlRng++;
                _rangePrice = calcPrice(_rangePrice, 1);
				Debug.Log ("Range of " + Name + " has upgraded.");
            }                 
        }

        public void upgradeRate()
        {
            if (_player.getCurrency() >= _ratePrice)
            {
                _player.DecreaseCurrency(_ratePrice);
                Delay /= 1.2f;
                _lvlRate++;
                _ratePrice = calcPrice(_ratePrice, 2);
				Debug.Log ("Rate of " + Name + " has upgraded.");
            }                  
        }

        public BigIntWithUnit calcPrice(BigIntWithUnit currentPrice, int x)
        {
            int tmp;
            switch(x)
            {
                case 0:
                    tmp = _lvlDmg;
                    break;
                case 1:
                    tmp = _lvlRng;
                    break;
                case 2:
                    tmp = _lvlRate;
                    break;
                default:
                    tmp = 0;
                    break;
            }
            double multiplier = System.Math.Pow(1.1, tmp) * 100;
            return BigIntWithUnit.MultiplyPercent(currentPrice, multiplier);
        }

        public int overallLevel()
        {
            return (_lvlDmg + _lvlRng + _lvlRate) / 3;
        }

    }
}