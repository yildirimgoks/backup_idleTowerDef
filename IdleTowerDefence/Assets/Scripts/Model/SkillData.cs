using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{

    public enum SkillType {
        AreaTop = 0,
        AreaBottom = 1,
        PathFollower = 2,
        AllMinions = 3,
        AllTowers = 4
    }

    public enum SkillEffect {
        Damage = 0,
        IncreaseDamage = 1,
        DecreaseDamage = 2,
        IncreaseSpeed = 3,
        DecreaseSpeed = 4, //Slow
        IncreaseRange = 5,
        DecreaseRange = 6,
        IncreaseDelay = 7,
        DecreaseDelay = 8,
    }

    public class SkillData
    {
        private Element _element;
        private SkillType _type;

        private List<SkillEffect> _minionEffects = new List<SkillEffect>();
        private List<SkillEffect> _towerEffects = new List<SkillEffect>();

        private BigIntWithUnit _spellDamage;
        private int _spellRange;
        private int _spellSpeed;
        

        public SkillData(Element element, BigIntWithUnit spellDamage, int spellRange, int spellSpeed) {
            _element = element;
            _type = ElementController.Instance.GetSkillType(element);
            _spellDamage = spellDamage;
            _spellRange = spellRange;
            _spellSpeed = spellSpeed;

            _minionEffects = ElementController.Instance.GetSkillEffectsToMinions(element);

            _towerEffects = ElementController.Instance.GetSkillEffectsToTowers(element);
        }

        public BigIntWithUnit GetDamage(){
            if (_minionEffects.Contains(SkillEffect.Damage)){
                return _spellDamage;
            }
            return 0;
        }

        public int GetRange(){
            return _spellRange;
        }

        public int GetSpeed(){
            return 2*_spellSpeed;
        }

        public double GetMultiplier(SkillEffect effect){
            if ( _minionEffects.Contains(effect) || _towerEffects.Contains(effect)){
                switch (effect){
                    case SkillEffect.IncreaseDamage:
                    case SkillEffect.IncreaseRange:
                    case SkillEffect.IncreaseSpeed:
                    case SkillEffect.IncreaseDelay:
                    return 1.5;
                    case SkillEffect.DecreaseDamage:
                    case SkillEffect.DecreaseRange:
                    case SkillEffect.DecreaseSpeed:
                    case SkillEffect.DecreaseDelay:
                    return 0.5;
                }
            }
            return 1;
        }
        
        public Element GetElement(){
            return _element;
        }

        public SkillType GetSkillType(){
            return _type;
        }

        public bool DoesEffectMinion(){
            return _minionEffects.Count > 0;
        }

        public bool DoesEffectTower(){
            return _towerEffects.Count > 0;
        }

        public List<SkillEffect> GetMinionEffects(){
            return _minionEffects;
        }
        
        public List<SkillEffect> GetTowerEffects(){
            return _towerEffects;
        }
    }
}
