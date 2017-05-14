using UnityEngine;
using System.Collections;
using Assets.Scripts.Model;
using Assets.Scripts.Manager;
using Assets.Scripts.Pooling;

namespace Assets.Scripts
{
	public class Spell : PoolableMonoBehaviour
	{
        public Minion TargetMinion;
        private Mage _masterMage;
	    private SpellData _data;
		private double _damageMultiplier;
	    public Player Player;
        private float _speed;
        //private Vector3 collisionPosition;

        //Update is called once per frame
        private void OnEnable() {
            _speed = -75;
        }

        private void Update()
		{
            _speed += (400 - _speed) * 0.035f;

			if (TargetMinion == null || TargetMinion.gameObject == null || !TargetMinion.Data.IsAlive() || TargetMinion.gameObject.tag == "Untagged")
			{
                if (Player.WaveManager.AliveMinionCount <= 0)
                {
                    Destroy();
                }
                else
                {
                    if (_masterMage != null)
                    {
                        TargetMinion = _masterMage.FindFirstMinion();
                    }
                    else
                    {
                        TargetMinion = Player.WaveManager.FindClosestMinion(transform.position);
                    }
                }
            }
			else
			{
				transform.position = Vector3.MoveTowards (transform.position, TargetMinion.transform.position, _speed * Time.deltaTime);
			}
		}

		public static void Clone(Player player, Spell playerSpellPrefab, SpellData data, Vector3 position, Minion targetMinion, Mage masterMage, double damageMultiplier = 1.0)
		{
		    Spell spell = (Spell)GetPoolable(playerSpellPrefab);
		    spell.Player = player;
		    spell.transform.position = position;
            spell.transform.rotation = Quaternion.identity;
		    spell._data = data;
			spell._damageMultiplier = damageMultiplier;
			spell.TargetMinion = targetMinion;
        }

        

		private void OnCollisionEnter(Collision coll)
		{
            if (coll != null && TargetMinion != null && coll.gameObject.GetInstanceID() == TargetMinion.gameObject.GetInstanceID())
            {
                if (coll.gameObject.tag == "Minion" || coll.gameObject.tag == "Boss")
                {
                    Player._audioManager.PlaySpellCollisionSound(_data.GetElement());
                    Destroy();
                    //collisionPosition = coll.gameObject.transform.position;
                    coll.gameObject.GetComponent<Minion>().GotHitAnimPlayer();
                    coll.gameObject.GetComponent<Minion>().DecreaseLife(_data.GetDamage() * _damageMultiplier);
                }
            }
        }
	}
}

