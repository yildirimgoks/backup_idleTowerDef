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
        ChangeDamage = 1,
        ChangeSpeed = 2,
        ChangeRange = 3,
        ChangeDelay = 4
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
        private float _spellMultiplier;

        public SkillData(Element element, BigIntWithUnit spellDamage, int spellRange, int spellSpeed, float spellMultiplier) {
            _element = element;
            _type = ElementController.Instance.GetSkillType(element);
            _spellDamage = spellDamage;
            _spellRange = spellRange;
            _spellSpeed = spellSpeed;
            _spellMultiplier = spellMultiplier;

            _minionEffects = ElementController.Instance.GetSkillEffectsToMinions(element);

            _towerEffects = ElementController.Instance.GetSkillEffectsToTowers(element);
        }

        public BigIntWithUnit GetDamage(){
            if (_minionEffects.Contains(SkillEffect.Damage)){
                return _spellDamage;
            }
            return 0;
        }

        public float GetRange(){
            return _spellRange;
        }

        public int GetSpeed(){
            return _spellSpeed;
        }

        public float GetMultiplier(SkillEffect effect){
            if ( _minionEffects.Contains(effect) || _towerEffects.Contains(effect)){
                return _spellMultiplier;
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
