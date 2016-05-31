using UnityEngine;

namespace Assets.Scripts
{
    public class Mage : MonoBehaviour
    {
        public GameObject TowerSpellPrefab;
        public float Delay;
        private float _spellTime = 0.0f;

        public bool Active = false;

        private Vector3 _screenPoint;
        private Vector3 _offset;

        private Vector3 _basePosition;
        private Tower _tower;

        private bool _dragged = false;

        public LayerMask MageDropMask;

        // Use this for initialization
        private void Start()
        {
            _basePosition = transform.position;
        }

        // Update is called once per frame
        private void Update()
        {
            // Cast spell with delay
            if (Time.time > _spellTime && Active)
            {
                _spellTime = Time.time + Delay;
                Instantiate(TowerSpellPrefab,_tower.transform.position, Quaternion.identity);
            }
        }

        private void OnMouseDown()
        {
            _basePosition = transform.position;
            _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

            _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
            _dragged = true;
            DeactivateTower();
        }

        private void OnMouseDrag()
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);

            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;
            transform.position = curPosition;
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
                        ActivateTower();
                    }
                    else
                    {
                        transform.position = _basePosition;
                        ActivateTower();
                    }

                }
                else if (hit)
                {
                    DeactivateTower();
                    _tower = null;
                }
                else if (!hit)
                {
                    transform.position = _basePosition;
                    ActivateTower();
                }
            }
        }

        private void ActivateTower()
        {
            if (_tower)
            {
                Active = true;
                _tower.Occupied = true;
                foreach (Renderer r in GetComponentsInChildren(typeof(Renderer)))
                {
                    r.enabled = false;
                }
            } 
        }

        private void DeactivateTower()
        {
            Active = false;
            if (_tower)
            {
                _tower.Occupied = false;
                foreach (Renderer r in GetComponentsInChildren(typeof(Renderer)))
                {
                    r.enabled = true;
                }
            }
        }

    }
}