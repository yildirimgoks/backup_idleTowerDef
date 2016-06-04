using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class TowerSpell : MonoBehaviour
    {
        public int Damage;
        public int Speed;
        public int Range;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            var target = FindFirstMinion();
            if (target != null)
            {
                var spellTarget = target.transform.position - transform.position;
                transform.Translate(spellTarget.normalized * Speed * Time.deltaTime);
            }
            else
            {
                Destroy(gameObject);
            }
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
            return (Mathf.Sqrt(distanceSq) < Range);
        }

        private void OnCollisionEnter(Collision coll)
        {
            if (coll.gameObject.tag == "Minion" || coll.gameObject.tag == "Boss")
            {
                Destroy(gameObject);
                coll.gameObject.GetComponent<Minion>().Life -= Damage;
            }
        }
    }
}