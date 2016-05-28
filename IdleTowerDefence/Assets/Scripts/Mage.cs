using UnityEngine;

namespace Assets.Scripts
{
    public class Mage : MonoBehaviour
    {
        public GameObject TowerSpellPrefab;
        public float delay;
        private float spellTime = 0.0f;

        public bool active = false;  

        // Use this for initialization
        private void Start()
        {
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

         private Vector3 screenPoint;
         private Vector3 offset;
 
        void OnMouseDown()
        {
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
 
             offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
 
         }
 
         void OnMouseDrag()
         {
             Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
 
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            transform.position = curPosition;
 
         }

    }
}