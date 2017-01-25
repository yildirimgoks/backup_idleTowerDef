using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Skills
{
	public class AreaTopSkillProjectile : SkillProjectile {
		
		// Update is called once per frame
		void Update () {
			transform.position = Vector3.MoveTowards (transform.position, _targetPosition, _data.GetSpeed() * Time.deltaTime);
			if ( transform.position.y <= _targetPosition.y ){
				DoRangedEffects();
                Destroy(2);
			}
		}
	}
}
