using UnityEngine;

namespace Assets.Scripts
{
    public class Mage : MonoBehaviour
    {
        public GameObject TowerSpellPrefab;
        public float delay;
        private float spellTime = 0.0f;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            // Cast spell with delay
            if (Time.time > spellTime)
            {
                spellTime = Time.time + delay;
                Instantiate(TowerSpellPrefab, gameObject.transform.position, Quaternion.identity);
                Debug.Log("Selamun Aleyküm");
            }
        }
    }
}