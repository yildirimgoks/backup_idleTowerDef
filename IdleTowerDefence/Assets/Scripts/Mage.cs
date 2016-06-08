using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Mage : MonoBehaviour
    {
        public TowerSpell TowerSpellPrefab;
        public float Delay;
        private float _spellTime;
		private bool _isIdle = true;

        public bool Active;

        private Vector3 _screenPoint;
        private Vector3 _offset;

        private Vector3 _basePosition;
        private Tower _tower;

        private bool _dragged;

        public LayerMask MageDropMask;

        public bool Dropped;

        // Use this for initialization
        private void Start()
        {
            _basePosition = transform.position;
			StartCoroutine (GenerateCurrency());
        }

        // Update is called once per frame
        private void Update()
        {
			
            // Cast spell with delay
            if (Active && Time.time > _spellTime)
            {
                _spellTime = Time.time + Delay;
                Instantiate(TowerSpellPrefab, _tower.transform.position, Quaternion.identity);
            }


        }

        private void OnMouseDown()
        {
            if (!Dropped){
                _basePosition = transform.position;
                _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

                _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
                _dragged = true;
                SetTowerAcive(false);
            }     
        }

        private void OnMouseDrag()
        {
            if (!Dropped)
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
                    }
                    else
                    {
                        transform.position = _basePosition;
                    }
                    SetTowerAcive(true);
					_isIdle = false;
					_tower.insideMage = this;
                }
                else if (hit)
                {
                    SetTowerAcive(false);
                    _tower = null;
					_isIdle = true;
                }
                else
                {
                    transform.position = _basePosition;
                    SetTowerAcive(true);
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

        private void SetTowerAcive(bool active)
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

		public void Eject(Tower tower){
			SetTowerAcive (false);
			tower.insideMage.transform.position = _basePosition;
			tower.insideMage._tower = null;
			_tower.insideMage = null;
		}

		IEnumerator GenerateCurrency() {
				for (int i = 0; i >= 0; i++) {
					yield return new WaitForSeconds (1f);
					if (!_isIdle) {
						break;
					}
					Camera.main.GetComponent<Player> ().IncreaseCurrency (3);
				}
		}
    }
}