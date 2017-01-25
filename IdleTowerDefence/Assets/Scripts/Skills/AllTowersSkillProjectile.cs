using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Skills
{
	public class AllTowersSkillProjectile : SkillProjectile {
		
		// Update is called once per frame
		void Update () {
			var targetPosition = _target.transform.position;
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, _data.GetSpeed() * Time.deltaTime);
			if ( transform.position.y <= targetPosition.y ){
				//Debug.Log("Projectile Position: "+ transform.position.y + "\t Target Position: "+ targetPosition.y);
				DoEffects(_target);
				Destroy();
			}
		}
	}
}
