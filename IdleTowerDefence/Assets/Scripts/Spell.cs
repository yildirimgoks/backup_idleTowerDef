using UnityEngine;
using System.Collections;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
	public class Spell : MonoBehaviour
	{
        public Minion TargetMinion;
	    private SpellData _data;

        // Use this for initialization
        private void Start()
		{
			gameObject.GetComponent<Renderer>().material.color = ElementController.Instance.GetColor(_data.GetElement());
		}

		//Update is called once per frame
		private void Update()
		{
			if (TargetMinion == null || TargetMinion.gameObject == null || !TargetMinion.Data.IsAlive() || TargetMinion.gameObject.tag == "Untagged")
			{
				Destroy(gameObject);
			}
			else
			{
				transform.position = Vector3.MoveTowards (transform.position, TargetMinion.transform.position, _data.GetSpeed() * Time.deltaTime);
			}
		}

		public static void Clone(Spell playerSpellPrefab, SpellData data, Vector3 position, Minion targetMinion)
		{
			var spell = (Spell) Instantiate(playerSpellPrefab, position, Quaternion.identity);
		    spell._data = data;
			spell.TargetMinion = targetMinion;
		}

		private void OnCollisionEnter(Collision coll)
		{
			if (coll.gameObject.tag == "Minion" || coll.gameObject.tag == "Boss")
			{
				Destroy(gameObject);
				coll.gameObject.GetComponent<Minion>().Data.DecreaseLife(_data.GetDamage());
			}
		}
	}
}

