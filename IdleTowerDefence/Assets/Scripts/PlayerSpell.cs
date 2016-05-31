using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerSpell : MonoBehaviour
    {
        public int Damage = 20;
        public int SpellSpeed = 100;
		public GameObject TargetMinion = null;


        // Use this for initialization
        private void Start()
        {
        }


        //Update is called once per frame
        private void Update()
        {
			if (TargetMinion == null)
            {
                Destroy(gameObject);
            }
            else
            {
				var spellTarget = TargetMinion.transform.position - transform.position;
                transform.Translate(spellTarget.normalized*SpellSpeed*Time.deltaTime);
            }
        }

		static public void Clone(GameObject PlayerSpellPrefab,Vector3 position){
			GameObject spell = (GameObject)Instantiate(PlayerSpellPrefab, position, Quaternion.identity);
			spell.GetComponent<PlayerSpell> ().TargetMinion = spell.GetComponent<PlayerSpell> ().FindClosestMinion ();
		}

        // Find closest minion's name
        public GameObject FindClosestMinion()
        {
            var minions = GameObject.FindGameObjectsWithTag("Minion");
            GameObject closestMinion = null;
            var distance = Mathf.Infinity;
            var position = transform.position;
            foreach (var minion in minions)
            {           			
                var curDistance = Vector3.Distance(minion.transform.position, position);
                if (curDistance < distance)
                {
                    closestMinion = minion;
                    distance = curDistance;
                }
            }
            var boss = GameObject.FindGameObjectWithTag("Boss");
            if (boss != null)
            {
                var bossDistance = Vector3.Distance(boss.transform.position, position);
                if (bossDistance < distance)
                {
                    closestMinion = boss;
                    distance = bossDistance;
                }
            }
            return closestMinion;
        }


		private void OnCollisionEnter(Collision coll)
		{
			if (coll.gameObject.tag == "Minion" || coll.gameObject.tag == "Boss")
			{
				Destroy(gameObject);
				coll.gameObject.GetComponent<Minion>().Life = coll.gameObject.GetComponent<Minion>().Life - Damage;
			}
		}
    }
}