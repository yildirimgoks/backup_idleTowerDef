using UnityEngine;
using System.Collections;
using Assets.Scripts.Model;
using Assets.Scripts.Manager;

namespace Assets.Scripts
{
	public class Spell : MonoBehaviour
	{
        public Minion TargetMinion;
        private Mage _masterMage;
	    private SpellData _data;
		private double _damageMultiplier;
        private AudioManager _audioManager;
        private WaveManager _waveManager;

        void Start()
        {
            _audioManager = Camera.main.GetComponent<AudioManager>();
            _waveManager = Camera.main.GetComponent<WaveManager>();
        }

		//Update is called once per frame
		private void Update()
		{
			if (TargetMinion == null || TargetMinion.gameObject == null || !TargetMinion.Data.IsAlive() || TargetMinion.gameObject.tag == "Untagged")
			{
                if (_waveManager.AliveMinionCount <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    if (_masterMage != null)
                    {
                        TargetMinion = _masterMage.FindFirstMinion();
                    }
                    else
                    {
                        TargetMinion = _waveManager.FindClosestMinion(transform.position);
                    }
                }
            }
			else
			{
				transform.position = Vector3.MoveTowards (transform.position, TargetMinion.transform.position, _data.GetSpeed() * Time.deltaTime);
			}
		}

		public static void Clone(Spell playerSpellPrefab, SpellData data, Vector3 position, Minion targetMinion, Mage masterMage, double damageMultiplier = 1.0)
		{
			var spell = (Spell) Instantiate(playerSpellPrefab, position, Quaternion.identity);
		    spell._data = data;
			spell._damageMultiplier = damageMultiplier;
			spell.TargetMinion = targetMinion;
        }

		private void OnCollisionEnter(Collision coll)
		{
            if (coll.gameObject.GetInstanceID() == TargetMinion.gameObject.GetInstanceID())
            {
                if (coll.gameObject.tag == "Minion" || coll.gameObject.tag == "Boss")
                {
                    _audioManager.PlaySpellCollisionSound(_data.GetElement());
                    Destroy(gameObject);
                    coll.gameObject.GetComponent<Minion>().DecreaseLife(_data.GetDamage() * _damageMultiplier);
                }
            }
        }
    }
}

