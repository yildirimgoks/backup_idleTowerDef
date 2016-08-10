using UnityEngine;
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
                    result = castAreaTopSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition);
                    break;
                case SkillType.AreaBottom:
                    result = castAreaBottomSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition);
                    break;
                case SkillType.PathFollower:
                    result = castPathFollowerSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition);
                    break;
                case SkillType.AllMinions:
                    result = castAllMinionsSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition);
                    break;
                case SkillType.AllTowers:
                    result = castAllTowersSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition);
                    break;
            }
			return result;
		}

        private bool castAreaTopSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition){
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

        private bool castAreaBottomSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition){
			// TODO
			return true;
        }

        private bool castPathFollowerSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition){
			// TODO
			return true;
        }

        private bool castAllMinionsSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition){
			// TODO
			return true;
        }

        private bool castAllTowersSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition){
			// TODO
			return true;
        }

	}
}
