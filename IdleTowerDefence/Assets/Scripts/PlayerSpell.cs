using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerSpell : MonoBehaviour
    {
        public BigIntWithUnit Damage = 20;
        public int SpellSpeed = 100;
        public Minion TargetMinion;

        // Use this for initialization
        private void Start()
        {
        }

        //Update is called once per frame
        private void Update()
        {
			if (TargetMinion == null || TargetMinion.Life <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                var spellTarget = TargetMinion.transform.position - transform.position;
                transform.Translate(spellTarget.normalized * SpellSpeed * Time.deltaTime);
            }
        }

        public static void Clone(PlayerSpell playerSpellPrefab, Vector3 position, Minion targetMinion)
        {
            var spell = (PlayerSpell) Instantiate(playerSpellPrefab, position, Quaternion.identity);
            spell.TargetMinion = targetMinion;
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