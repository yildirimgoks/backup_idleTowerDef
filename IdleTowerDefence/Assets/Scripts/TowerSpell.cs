using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class TowerSpell : MonoBehaviour
    {

        public int damage;
        public int speed;
        private int range = 10;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Minion target = FindFirstMinion();
            if (target != null)
            {   
                var spellTarget = target.transform.position - transform.position;
                transform.Translate(spellTarget.normalized * speed * Time.deltaTime);
            } else
            {
                Destroy(gameObject);
            }
        }

        // Find leader minion
        public Minion FindFirstMinion()
        {
            GameObject cam = GameObject.Find("Main Camera");
            Player playerScript = cam.GetComponent<Player>();
            List<Minion> minions = playerScript.getList();
            Minion target = minions.First<Minion>();
            int index = 1;
            while (!inRange(target))
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

        public bool inRange(Minion metul)
        {
            float deltaX, deltaZ, distanceSq;
            deltaX = transform.position.x - metul.transform.position.x;
            deltaZ = transform.position.z - metul.transform.position.z;
            distanceSq = deltaX * deltaX + deltaZ * deltaZ;
            return (Mathf.Sqrt(distanceSq) < range);

        }

        private void OnCollisionEnter(Collision coll)
        {
            if (coll.gameObject.tag == "Minion")
            {
                Destroy(gameObject);
                coll.gameObject.GetComponent<Minion>().Life = coll.gameObject.GetComponent<Minion>().Life - damage;
            }
        }
    }
}

