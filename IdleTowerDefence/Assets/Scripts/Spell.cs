using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
	public class Spell : MonoBehaviour
	{
		public BigIntWithUnit Damage; //20
		public int Speed; //100
		public Minion TargetMinion;
		public Element Element;

		// Use this for initialization
		private void Start()
		{
			gameObject.GetComponent<Renderer> ().material.color = ElementController.Instance.GetColor(Element);
		}

		//Update is called once per frame
		private void Update()
		{
			if (TargetMinion == null || TargetMinion.gameObject == null || TargetMinion.Life <= 0 || TargetMinion.gameObject.tag == "Untagged")
			{
				Destroy(gameObject);
			}
			else
			{
				transform.position = Vector3.MoveTowards (transform.position, TargetMinion.transform.position, Speed * Time.deltaTime);
			}
		}

		public static void Clone(Spell playerSpellPrefab, BigIntWithUnit Damage, int Speed, Element element, Vector3 position, Minion targetMinion)
		{
			var spell = (Spell) Instantiate(playerSpellPrefab, position, Quaternion.identity);
			spell.Damage = Damage;
			spell.Speed = Speed;
			spell.Element = element;
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

