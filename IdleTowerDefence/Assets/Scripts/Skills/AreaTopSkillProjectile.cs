using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Skills
{
	public class AreaTopSkillProjectile : SkillProjectile {

        private Vector3 _targetPosition;
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
			transform.position = Vector3.MoveTowards (transform.position, _targetPosition, _data.GetSpeed() * Time.deltaTime);
			if ( transform.position.y <= _targetPosition.y ){
				DoRangedEffects();
				Destroy(gameObject, 2);
			}
		}

		public static void Clone(SkillProjectile skillPrefab, Mage mage, Vector3 position, Vector3 targetPosition)
        {
            var skillProjectile = (AreaTopSkillProjectile)Instantiate(skillPrefab, position, Quaternion.identity);
            skillProjectile._data = mage.Data.GetSkillData();
            skillProjectile._player = mage.Player;
            skillProjectile._targetPosition = targetPosition;
        }

	}
}
