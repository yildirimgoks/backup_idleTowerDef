using UnityEngine;
using System.Collections;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
	public class Spell : MonoBehaviour
	{
        public Minion TargetMinion;
	    private SpellData _data;
        static ElementController ElementController;
        

		private double _damageMultiplier;
        


        private void Start()
		{
            AudioSource Audio;
            Audio = GetComponent<AudioSource>();
            Audio.clip = ElementController.Instance.GetSoundEffects(_data.GetElement());
            Audio.Play();

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

		public static void Clone(Spell playerSpellPrefab, SpellData data, Vector3 position, Minion targetMinion, double damageMultiplier = 1.0)
		{
			var spell = (Spell) Instantiate(playerSpellPrefab, position, Quaternion.identity);
		    spell._data = data;
			spell._damageMultiplier = damageMultiplier;
			spell.TargetMinion = targetMinion;
        }

		private void OnCollisionEnter(Collision coll)
		{
			if (coll.gameObject.tag == "Minion" || coll.gameObject.tag == "Boss")
			{
				Destroy(gameObject);
				coll.gameObject.GetComponent<Minion>().DecreaseLife(_data.GetDamage() * _damageMultiplier);
			}

            AudioSource Audio;
            Audio = GetComponent<AudioSource>();
            Audio.clip = ElementController.Instance.GetSpellCollisionEffects(_data.GetElement());
            Audio.Play();
        }
    }
}

