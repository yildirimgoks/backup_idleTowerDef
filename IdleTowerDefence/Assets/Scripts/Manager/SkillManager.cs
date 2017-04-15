using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.Skills;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Manager
{
	public class SkillManager : MonoBehaviour {

        public SkillProjectile[] SkillParticles;
        public SkillProjectile SkillPrefab;
		private bool castSkill;
		private List<SkillProjectile> _animationList = new List<SkillProjectile>();
        public LayerMask IgnoreSkill;

        public void StopAnimations(){
			foreach (SkillProjectile projectile in _animationList) {
				projectile.Destroy();
			}
			_animationList.Clear();
		}


		public void DeleteAnimation(SkillProjectile item){
			//Debug.Log("Deleted Animation");
			_animationList.Remove(item);
		}

		public void ExitSkillCancel(){
			castSkill = true;
		}

		public void EnterSkillCancel(){
			castSkill = false;
		}

		public void CallSkill(Mage mage){
			switch (mage.Data.GetSkillData().GetSkillType()) {
                case SkillType.AreaTop:
                case SkillType.AreaBottom:
                case SkillType.PathFollower:
                case SkillType.AllMinions:
					var minions = mage.Player.WaveManager.GetMinionList();
					foreach ( var minion in minions ){
						minion.StartHighlighting(ElementController.Instance.GetColor(mage.Data.GetElement()));
					}
                    break;
                case SkillType.AllTowers:
					var assignableBuildings = mage.Player.GetSceneReferenceManager().AllAssignableBuildings;
					foreach ( var assignableBuilding in assignableBuildings ){
						Tower tower;
						if (tower = assignableBuilding as Tower ){
							tower.StartHighlighting(ElementController.Instance.GetColor(mage.Data.GetElement()));
						}
					}
                    break;
            }
		}

		public void StopCallingSkill(Mage mage){
			switch (mage.Data.GetSkillData().GetSkillType()) {
                case SkillType.AreaTop:
                case SkillType.AreaBottom:
                case SkillType.PathFollower:
                case SkillType.AllMinions:
					var minions = mage.Player.WaveManager.GetMinionList();
					foreach ( var minion in minions ){
						minion.StopHighlighting();
					}
                    break;
                case SkillType.AllTowers:
					var assignableBuildings = mage.Player.GetSceneReferenceManager().AllAssignableBuildings;
					foreach ( var assignableBuilding in assignableBuildings ){
						Tower tower;
						if (tower = assignableBuilding as Tower ){
							tower.StopHighlighting();
						}
					}
                    break;
            }
		}

        public bool CastSkill(Mage mage, LayerMask FloorMask, Vector3 mousePosition, bool isAnimation) {
            StopCallingSkill(mage);

            if (castSkill == false) return true;
            bool result = false;
                switch (mage.Data.GetSkillData().GetSkillType())
              {
                case SkillType.AreaTop:
                    result = CastAreaTopSkill(mage, FloorMask, mousePosition);
                    // result = CastAreaTopSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition, isAnimation);
                    break;
                case SkillType.AreaBottom:
                    result = CastAreaBottomSkill(mage, FloorMask, mousePosition);
                    // result = CastAreaBottomSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition, isAnimation);
                    break;
                case SkillType.PathFollower:
                    result = CastPathFollowerSkill(mage, FloorMask, mousePosition);
                    // result = CastPathFollowerSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition, isAnimation);
                    break;
                case SkillType.AllMinions:
                    result = CastAllMinionsSkill(mage, FloorMask, mousePosition);
                    // result = CastAllMinionsSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition, isAnimation);
                    break;
                case SkillType.AllTowers:
                    result = CastAllTowersSkill(mage, FloorMask, mousePosition);
                    // result = CastAllTowersSkill(mage, IgnorePlayerSpell, FloorMask, mousePosition, isAnimation);
                    break;
                }
            return result;
		}

		// private bool CastAreaTopSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition, bool isAnimation){
        private bool CastAreaTopSkill(Mage mage, LayerMask FloorMask, Vector3 mousePosition){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit floorHit;
			RaycastHit uiHit;

			
			// if ( isAnimation ){
			// 	Vector3 vec = new Vector3(Random.Range(-40,40), 50, 10f+Random.Range(-40,40));
			// 	SkillProjectile projectile = AreaTopSkillProjectile.Clone(ElementController.Instance.GetSkillProjectile(mage.Data.GetElement()), mage, vec, new Vector3(vec.x, 0, vec.z),isAnimation);
			// 	_animationList.Add(projectile);
			// 	return true;
			// }

			// if (!Physics.Raycast(ray, out uiHit, Mathf.Infinity, IgnorePlayerSpell) &&
			//     Physics.Raycast(ray, out floorHit, Mathf.Infinity, FloorMask) &&
			// 	!isAnimation)
			// {
			if (!Physics.Raycast(ray, out uiHit, Mathf.Infinity, IgnoreSkill) &&
			Physics.Raycast(ray, out floorHit, Mathf.Infinity, FloorMask))
			{
                // AreaTopSkillProjectile.Clone(ElementController.Instance.GetSkillProjectile(mage.Data.GetElement()), mage, new Vector3(floorHit.point.x, 50, floorHit.point.z), new Vector3(floorHit.point.x, 0, floorHit.point.z),isAnimation);

                SkillProjectile.Clone<AreaTopSkillProjectile>(GetSkillProjectile(mage.Data.GetElement()), mage,
                    new Vector3(floorHit.point.x, 50, floorHit.point.z), null, new Vector3(floorHit.point.x, 0, floorHit.point.z));
                return true;
			}else{
				return false;
			}
        }

		// private bool CastAreaBottomSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition, bool isAnimation){
        private bool CastAreaBottomSkill(Mage mage, LayerMask FloorMask, Vector3 mousePosition){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit floorHit;
			RaycastHit uiHit;

			// if ( isAnimation ){
			// 	Vector3 vec = new Vector3(Random.Range(-40,40), 50, 10f+Random.Range(-40,40));
			// 	SkillProjectile projectile = AreaBotSkillProjectile.Clone(ElementController.Instance.GetSkillProjectile(mage.Data.GetElement()), mage, vec, new Vector3(vec.x, 0, vec.z),isAnimation);
			// 	_animationList.Add(projectile);
			// 	return true;
			// }

			// if (!Physics.Raycast(ray, out uiHit, Mathf.Infinity, IgnorePlayerSpell) &&
			//     Physics.Raycast(ray, out floorHit, Mathf.Infinity, FloorMask) &&
			// 	!isAnimation)
			if (!Physics.Raycast(ray, out uiHit, Mathf.Infinity, IgnoreSkill) &&
			    Physics.Raycast(ray, out floorHit, Mathf.Infinity, FloorMask))
			{
				// SkillProjectile projectile = AreaBotSkillProjectile.Clone(ElementController.Instance.GetSkillProjectile(mage.Data.GetElement()), mage, new Vector3(floorHit.point.x, 0, floorHit.point.z), new Vector3(floorHit.point.x, 0, floorHit.point.z),isAnimation);
                SkillProjectile.Clone<AreaBotSkillProjectile>(GetSkillProjectile(mage.Data.GetElement()), mage,
                    new Vector3(floorHit.point.x, 0, floorHit.point.z), null, new Vector3(floorHit.point.x, 0, floorHit.point.z));
                return true;
			}else{
				return false;
			}
        }

		// private bool CastPathFollowerSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition, bool isAnimation){
        private bool CastPathFollowerSkill(Mage mage, LayerMask FloorMask, Vector3 mousePosition){
			// SkillProjectile projectile = PathFollowerSkillProjectile.Clone(ElementController.Instance.GetSkillProjectile(mage.Data.GetElement()), mage, mage.Player.WaveManager.EndWaypoint,isAnimation);
			PathFollowerSkillProjectile.Clone(GetSkillProjectile(mage.Data.GetElement()), mage, mage.Player.GetSceneReferenceManager().EndWaypoint);
			// if (isAnimation){
			// 	_animationList.Add(projectile);
			// }
			return true;
        }

		// private bool CastAllMinionsSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition, bool isAnimation){
        private bool CastAllMinionsSkill(Mage mage, LayerMask FloorMask, Vector3 mousePosition){
			mage.Player.WaveManager.GetMinionList().ForEach((Minion minion)=> {
				// SkillProjectile projectile = AllMinionsSkillProjectile.Clone(ElementController.Instance.GetSkillProjectile(mage.Data.GetElement()), mage, new Vector3(minion.transform.position.x, 50, minion.transform.position.z), minion.gameObject,isAnimation);
				
                SkillProjectile.Clone<AllMinionsSkillProjectile>(GetSkillProjectile(mage.Data.GetElement()), mage,
                    new Vector3(minion.transform.position.x, 50, minion.transform.position.z), minion.gameObject, new Vector3());
                // if (isAnimation){
                // 	_animationList.Add(projectile);
                // }
            });
			return true;
        }

		// private bool CastAllTowersSkill(Mage mage, LayerMask IgnorePlayerSpell, LayerMask FloorMask, Vector3 mousePosition, bool isAnimation){
        private bool CastAllTowersSkill(Mage mage, LayerMask FloorMask, Vector3 mousePosition){
			mage.Player.Data.GetMages().ToList().ForEach((Mage thisMage) => {	
				if (thisMage.GetBuilding() != null && thisMage.GetBuilding() is Tower){
					Tower tower = thisMage.GetBuilding() as Tower;
					// SkillProjectile projectile = AllTowersSkillProjectile.Clone(ElementController.Instance.GetSkillProjectile(mage.Data.GetElement()), mage, new Vector3(tower.gameObject.transform.position.x, 50, tower.gameObject.transform.position.z), tower.gameObject,isAnimation);
                    SkillProjectile.Clone<AllTowersSkillProjectile>(GetSkillProjectile(mage.Data.GetElement()), mage,
                        new Vector3(tower.gameObject.transform.position.x, 50, tower.gameObject.transform.position.z), tower.gameObject, new Vector3());
                    // if (isAnimation){
                    // 	_animationList.Add(projectile);
                    // }
                }
			});
			return true;
        }

        public SkillProjectile GetSkillProjectile(Element element)
        {
            //Debug.Log(element);
            switch (element)
            {
                case Element.Fire:
                    return SkillParticles[0];
                case Element.Water:
                    return SkillParticles[1];
                case Element.Earth:
                    return SkillParticles[2];
                case Element.Air:
                    return SkillParticles[3];
                default:
                    return SkillParticles[0];
            }
        }

    }
}
