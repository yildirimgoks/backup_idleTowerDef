using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Skills
{
	public class AreaTopSkillProjectile : SkillProjectile {

        private Vector3 _targetPosition;
		
		// Update is called once per frame
		void Update () {
			transform.position = Vector3.MoveTowards (transform.position, _targetPosition, _data.GetSpeed() * Time.deltaTime);
			if ( transform.position.y <= _targetPosition.y ){
				DoRangedEffects();
				Destroy(gameObject, 2);
			}
		}

		public static AreaTopSkillProjectile Clone(SkillProjectile skillPrefab, Mage mage, Vector3 position, Vector3 targetPosition, bool isAnimation)
        {
            var skillProjectile = (AreaTopSkillProjectile)Instantiate(skillPrefab, position, Quaternion.identity);
            skillProjectile._data = mage.Data.GetSkillData();
            skillProjectile._player = mage.Player;
            skillProjectile._targetPosition = targetPosition;
			skillProjectile._isAnimation = isAnimation;
			return skillProjectile;
        }

	}
}
