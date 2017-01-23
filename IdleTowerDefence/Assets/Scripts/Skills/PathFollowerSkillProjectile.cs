using UnityEngine;
using Assets.Scripts.Model;

namespace Assets.Scripts.Skills
{
	public class PathFollowerSkillProjectile : SkillProjectile {
		

		// Update is called once per frame
		void Update () {
			transform.Translate(Vector3.forward * _data.GetSpeed() * Time.deltaTime);
			transform.position = new Vector3(transform.position.x,gameObject.GetComponent<Collider>().bounds.size.y/2,transform.position.z);
			if ( transform.FindChild("Boulder") != null ){
				transform.FindChild("Boulder").Rotate(new Vector3(8*Time.deltaTime*_data.GetSpeed(),0,0));
			}
		}

		public void OnTriggerEnter(Collider Other){
			// if (_isAnimation)return;
			if ( Other.gameObject.GetComponent<Minion>()){
				_data.GetMinionEffects().ForEach((effect) => {
					switch (effect){
						case SkillEffect.Damage:
							Other.gameObject.GetComponent<Minion>().DecreaseLife(_data.GetDamage());
							break;
						case SkillEffect.ChangeSpeed:
							Other.gameObject.GetComponent<Minion>().ChangeSpeed(_data.GetMultiplier(effect));
							break;
					}
				});
			}
        }

		// public static PathFollowerSkillProjectile Clone(SkillProjectile skillPrefab, Mage mage, Waypoint EndWaypoint, bool isAnimation)
		public static PathFollowerSkillProjectile Clone(SkillProjectile skillPrefab, Mage mage, Waypoint EndWaypoint)
        {
            Vector3 pos = new Vector3(EndWaypoint.transform.position.x, EndWaypoint.transform.position.y,EndWaypoint.transform.position.z);
            var skillProjectile = (PathFollowerSkillProjectile)GetPoolable(skillPrefab);

		    skillProjectile.transform.position = pos;
            skillProjectile.transform.rotation = Quaternion.identity;		    
            skillProjectile.transform.LookAt(EndWaypoint.Previous.transform);
            skillProjectile._data = mage.Data.GetSkillData();
            skillProjectile._player = mage.Player;
			// skillProjectile._isAnimation = isAnimation;
			return skillProjectile;
        }
	}
}
