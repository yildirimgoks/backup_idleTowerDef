using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.Skills;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Manager
{
	public class SkillManager : MonoBehaviour {

		public SkillProjectile SkillPrefab;

		private List<SkillProjectile> _animationList = new List<SkillProjectile>();

		public void StopAnimations(){
			foreach (SkillProjectile projectile in _animationList) {
				Destroy(projectile.gameObject);
			}
			_animationList.Clear();
		}

		public void DeleteAnimation(SkillProjectile item){
			Debug.Log("Deleted Animation");
			_animationList.Remove(item);
		}

		public bool CastSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition, bool isAnimation){
			bool result = false;
			switch (mage.Data.GetSkillData().GetSkillType()) {
                case SkillType.AreaTop:
                    result = CastAreaTopSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition, isAnimation);
                    break;
                case SkillType.AreaBottom:
                    result = CastAreaBottomSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition, isAnimation);
                    break;
                case SkillType.PathFollower:
                    result = CastPathFollowerSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition, isAnimation);
                    break;
                case SkillType.AllMinions:
                    result = CastAllMinionsSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition, isAnimation);
                    break;
                case SkillType.AllTowers:
                    result = CastAllTowersSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition, isAnimation);
                    break;
            }
			return result;
		}

        private bool CastAreaTopSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition, bool isAnimation){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit floorHit;
			RaycastHit uiHit;
			if (!Physics.Raycast(ray, out uiHit, Mathf.Infinity, IgnorePlayerSpell) &&
			    Physics.Raycast(ray, out floorHit, Mathf.Infinity, FloorMask))
			{
				SkillProjectile projectile = AreaTopSkillProjectile.Clone(ElementController.Instance.GetSkillProjectile(mage.Data.GetElement()), mage, new Vector3(floorHit.point.x, 50, floorHit.point.z), new Vector3(floorHit.point.x, 0, floorHit.point.z),isAnimation);
				if (isAnimation){
					_animationList.Add(projectile);
				}
				return true;
			}else{
				return false;
			}
        }

        private bool CastAreaBottomSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition, bool isAnimation){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit floorHit;
			RaycastHit uiHit;
			if (!Physics.Raycast(ray, out uiHit, Mathf.Infinity, IgnorePlayerSpell) &&
			    Physics.Raycast(ray, out floorHit, Mathf.Infinity, FloorMask))
			{
				SkillProjectile projectile = AreaBotSkillProjectile.Clone(ElementController.Instance.GetSkillProjectile(mage.Data.GetElement()), mage, new Vector3(floorHit.point.x, 0, floorHit.point.z), new Vector3(floorHit.point.x, 0, floorHit.point.z),isAnimation);
				if (isAnimation){
					_animationList.Add(projectile);
				}
				return true;
			}else{
				return false;
			}
        }

        private bool CastPathFollowerSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition, bool isAnimation){
			SkillProjectile projectile = PathFollowerSkillProjectile.Clone(ElementController.Instance.GetSkillProjectile(mage.Data.GetElement()), mage, mage.Player.WaveManager.EndWaypoint,isAnimation);
			if (isAnimation){
				_animationList.Add(projectile);
			}
			return true;
        }

        private bool CastAllMinionsSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition, bool isAnimation){
			mage.Player.WaveManager.GetMinionList().ForEach((Minion minion)=> {
				SkillProjectile projectile = AllMinionsSkillProjectile.Clone(ElementController.Instance.GetSkillProjectile(mage.Data.GetElement()), mage, new Vector3(minion.transform.position.x, 50, minion.transform.position.z), minion.gameObject,isAnimation);
				if (isAnimation){
					_animationList.Add(projectile);
				}
			});
			return true;
        }

        private bool CastAllTowersSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition, bool isAnimation){
			mage.Player.Data.GetMages().ToList().ForEach((Mage thisMage) => {	
				if (thisMage.GetBuilding() != null && thisMage.GetBuilding() is Tower){
					Tower tower = thisMage.GetBuilding() as Tower;
					SkillProjectile projectile = AllTowersSkillProjectile.Clone(ElementController.Instance.GetSkillProjectile(mage.Data.GetElement()), mage, new Vector3(tower.gameObject.transform.position.x, 50, tower.gameObject.transform.position.z), tower.gameObject,isAnimation);
					if (isAnimation){
						_animationList.Add(projectile);
					}
				}
			});
			return true;
        }

	}
}
