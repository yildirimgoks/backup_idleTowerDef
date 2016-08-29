using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts.Model;
using System.Collections.Generic;

namespace Assets.Scripts.Skills
{
	public class PathFollowerSkillProjectile : SkillProjectile {
		// Use this for initialization
		void Start () {

		}
		
		// Update is called once per frame
		void Update () {
			transform.Translate(Vector3.forward * _data.GetSpeed() * Time.deltaTime);
			transform.position = new Vector3(transform.position.x,gameObject.GetComponent<Collider>().bounds.size.y/2,transform.position.z);        
		}

		public void OnTriggerEnter(Collider Other){
			if ( Other.gameObject.GetComponent<Minion>()){
				_data.GetMinionEffects().ForEach((SkillEffect effect) => {
					switch (effect){
						case SkillEffect.Damage:
							Other.gameObject.GetComponent<Minion>().DecreaseLife(_data.GetDamage());
							break;
						case SkillEffect.IncreaseSpeed:
						case SkillEffect.DecreaseSpeed:
							Other.gameObject.GetComponent<Minion>().ChangeSpeed(_data.GetMultiplier(effect));
							break;
						default:
							break;
					}
				});
			}
        }

		public static void Clone(SkillProjectile skillPrefab, Mage mage, Waypoint EndWaypoint)
        {
			Debug.Log(skillPrefab);
            Vector3 pos = new Vector3(EndWaypoint.transform.position.x, EndWaypoint.transform.position.y,EndWaypoint.transform.position.z);
            var skillProjectile = (PathFollowerSkillProjectile)Instantiate(skillPrefab, pos, Quaternion.identity);
            skillProjectile.transform.LookAt(EndWaypoint.Previous.transform);
            skillProjectile._data = mage.Data.GetSkillData();
            skillProjectile._player = mage.Player;
        }
	}
}
