using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Skills
{
	public class AllTowersSkillProjectile : SkillProjectile {

		private GameObject _target;

		// Use this for initialization
		void Start () {

		}
		
		// Update is called once per frame
		void Update () {
			var targetPosition = _target.transform.position;
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, _data.GetSpeed() * Time.deltaTime);
			if ( transform.position.y <= targetPosition.y ){
				Debug.Log("Projectile Position: "+ transform.position.y + "\t Target Position: "+ targetPosition.y);
				DoEffects(_target);
				Destroy(gameObject);
			}
		}

		public static void Clone(SkillProjectile skillPrefab, Mage mage, Vector3 position, GameObject target)
        {
            var skillProjectile = (AllTowersSkillProjectile)Instantiate(skillPrefab, position, Quaternion.identity);
            skillProjectile._data = mage.Data.GetSkillData();
            skillProjectile._player = mage.Player;
            skillProjectile._target = target;
        }
	}
}
