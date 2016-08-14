using UnityEngine;
using System.Linq;
using System.Collections;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
	public class SkillManager : MonoBehaviour {

		public SkillProjectile SkillPrefab;

		void Start(){

		}

		void Update(){

		}

		public bool CastSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition){
			bool result = false;
			switch (mage.Data.GetSkillData().GetSkillType()) {
                case SkillType.AreaTop:
                    result = CastAreaTopSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition);
                    break;
                case SkillType.AreaBottom:
                    result = CastAreaBottomSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition);
                    break;
                case SkillType.PathFollower:
                    result = CastPathFollowerSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition);
                    break;
                case SkillType.AllMinions:
                    result = CastAllMinionsSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition);
                    break;
                case SkillType.AllTowers:
                    result = CastAllTowersSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition);
                    break;
            }
			return result;
		}

        private bool CastAreaTopSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit floorHit;
			RaycastHit uiHit;
			if (!Physics.Raycast(ray, out uiHit, Mathf.Infinity, IgnorePlayerSpell) &&
			    Physics.Raycast(ray, out floorHit, Mathf.Infinity, FloorMask))
			{
				SkillProjectile.Clone(SkillPrefab, mage, new Vector3(floorHit.point.x, 50, floorHit.point.z), new Vector3(floorHit.point.x, 0, floorHit.point.z));
				return true;
			}else{
				return false;
			}
        }

        private bool CastAreaBottomSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit floorHit;
			RaycastHit uiHit;
			if (!Physics.Raycast(ray, out uiHit, Mathf.Infinity, IgnorePlayerSpell) &&
			    Physics.Raycast(ray, out floorHit, Mathf.Infinity, FloorMask))
			{
				SkillProjectile.Clone(SkillPrefab, mage, new Vector3(floorHit.point.x, 0, floorHit.point.z), new Vector3(floorHit.point.x, 0, floorHit.point.z));
				return true;
			}else{
				return false;
			}
        }

        private bool CastPathFollowerSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition){
			SkillProjectile.Clone(SkillPrefab, mage, mage.Player.WaveManager.EndWaypoint);
			return true;
        }

        private bool CastAllMinionsSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition){
			mage.Player.WaveManager.GetMinionList().ForEach((Minion minion)=> {
				SkillProjectile.Clone(SkillPrefab, mage, new Vector3(minion.transform.position.x, 50, minion.transform.position.z), minion.gameObject);
			});
			return true;
        }

        private bool CastAllTowersSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition){
			mage.Player.Data.GetMages().ToList().ForEach((Mage thisMage) => {	
				if (thisMage.GetBuilding() != null && thisMage.GetBuilding() is Tower){
					Tower tower = thisMage.GetBuilding() as Tower;
					SkillProjectile.Clone(SkillPrefab, mage, new Vector3(tower.gameObject.transform.position.x, 50, tower.gameObject.transform.position.z), tower.gameObject);
				}
			});
			return true;
        }

	}
}
