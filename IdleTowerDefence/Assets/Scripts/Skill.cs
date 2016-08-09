using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts
{
    
    public enum SkillType{
        AreaTop = 0,
        AreaBottom = 1,
        PathFollower = 2,
        AllMinions = 3,
        AllTowers = 4
    }

    public enum SkillEffect{
        Damage = 0,
        IncreaseDamage = 1,
        DecreaseDamage = 2,
        IncreaseSpeed = 3,
        DecreaseSpeed = 4, //Slow
        IncreaseRange = 5,
        DecreaseRange = 6,
        IncreaseDelay = 7,
        DecreaseDelay = 8
    }

    public class Skill
    {
        Element element;
        SkillType type;
        bool effectsMinion;
        List<SkillEffect> minionEffects = new List<SkillEffect>();
        bool effectsTower;
        List<SkillEffect> towerEffects = new List<SkillEffect>();

        public Skill(Element element){
            this.element = element;
            this.type = ElementController.Instance.GetSkillType(element);
            this.minionEffects = ElementController.Instance.GetSkillEffectsToMinions(element);
            if ( minionEffects.Count <= 0 ){
                minionEffects = null;
                effectsMinion = false;
            }else{
                effectsMinion = true;
            }
            this.towerEffects = ElementController.Instance.GetSkillEffectsToTowers(element);
            if ( towerEffects.Count <= 0 ){
                towerEffects = null;
                effectsTower = false;
            }else{
                effectsTower = true;
            }
        }

        void prepare(){
            switch (type) {
                case SkillType.AreaTop:
                    prepareAreaTopSkill();
                    break;
                case SkillType.AreaBottom:
                    prepareAreaBottomSkill();
                    break;
                case SkillType.PathFollower:
                    preparePathFollowerSkill();
                    break;
                case SkillType.AllMinions:
                    prepareAllMinionsSkill();
                    break;
                case SkillType.AllTowers:
                    prepareAllTowersSkill();
                    break;
            }
        }

        private void prepareAreaTopSkill(){
        }

        private void prepareAreaBottomSkill(){
        }

        private void preparePathFollowerSkill(){
        }

        private void prepareAllMinionsSkill(){
        }

        private void prepareAllTowersSkill(){
        }


    }
}