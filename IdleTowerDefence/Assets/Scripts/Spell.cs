using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
	public class Spell : MonoBehaviour
	{
		public BigIntWithUnit Damage; //20
		public int Speed; //100
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
				transform.Translate(spellTarget.normalized * Speed * Time.deltaTime);
			}
		}

		public static void Clone(Spell playerSpellPrefab, BigIntWithUnit Damage, int Speed, Vector3 position, Minion targetMinion)
		{
			var spell = (Spell) Instantiate(playerSpellPrefab, position, Quaternion.identity);
			spell.Damage = Damage;
			spell.Speed = Speed;
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

