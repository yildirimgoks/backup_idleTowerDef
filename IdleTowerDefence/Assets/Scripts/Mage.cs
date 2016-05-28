using UnityEngine;

namespace Assets.Scripts
{
    public class Mage : MonoBehaviour
    {
        public GameObject TowerSpellPrefab;
        public float delay;
        private float spellTime = 0.0f;

        public bool active = false;

        private Vector3 screenPoint;
        private Vector3 offset;
        private Transform basePosition;

        private bool dragged = false;

        public LayerMask mageDropMask;

        // Use this for initialization
        private void Start()
        {
            basePosition = transform;
        }

        // Update is called once per frame
        private void Update()
        {
            // Cast spell with delay
            if (Time.time > spellTime && active)
            {
                spellTime = Time.time + delay;
                Instantiate(TowerSpellPrefab, gameObject.transform.position, Quaternion.identity);
            }
        }

        private void OnMouseDown()
        {
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            dragged = true;
        }

        private void OnMouseDrag()
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            transform.position = curPosition;
        }

        private void OnMouseUp()
        {
            if (dragged)
            {
                dragged = false;
                RaycastHit hitObject;
                if (Physics.Raycast(Camera.main.transform.position, transform.position - Camera.main.transform.position, out hitObject,Mathf.Infinity,
                    mageDropMask))
                {
                    Debug.Log(Camera.main.transform.position);
                    Debug.Log(hitObject.collider.gameObject.tag);
                    Debug.Log(hitObject.collider.gameObject.name);
                    Debug.Log(transform.position - Camera.main.transform.position);
                    Debug.Log("Tower buldum.");
                }
            }
        }

    }
}