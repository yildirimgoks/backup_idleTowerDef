using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Model
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
        DecreaseRange = 6
    }

    public class SkillData
    {
        private Element _element;
        SkillType _type;
        bool _effectsMinion;
        List<SkillEffect> _minionEffects = new List<SkillEffect>();
        bool _effectsTower;
        List<SkillEffect> _towerEffects = new List<SkillEffect>();
        BigIntWithUnit _spellDamage;
        int _spellRange;
        int _spellSpeed;
        

        public SkillData(Element element, BigIntWithUnit spellDamage, int spellRange, int spellSpeed){
            _element = element;
            _type = ElementController.Instance.GetSkillType(element);
            _spellDamage = spellDamage;
            _spellRange = spellRange;
            _spellSpeed = spellSpeed;
            _minionEffects = ElementController.Instance.GetSkillEffectsToMinions(element);
            if ( _minionEffects.Count <= 0 ){
                _minionEffects = null;
                _effectsMinion = false;
            }else{
                _effectsMinion = true;
            }
            _towerEffects = ElementController.Instance.GetSkillEffectsToTowers(element);
            if ( _towerEffects.Count <= 0 ){
                _towerEffects = null;
                _effectsTower = false;
            }else{
                _effectsTower = true;
            }
        }

        public BigIntWithUnit GetDamage(){
            if (_minionEffects.Contains(SkillEffect.Damage)){
                return 2*_spellDamage;
            }
            return 0;
        }

        public int GetRange(){
            return 2*_spellRange;
        }

        public int GetSpeed(){
            return 2*_spellSpeed;
        }

        public double GetMultiplier(){
            if (_minionEffects.Contains(SkillEffect.DecreaseDamage) ||
                _minionEffects.Contains(SkillEffect.IncreaseDamage) ||
                _minionEffects.Contains(SkillEffect.DecreaseRange) ||
                _minionEffects.Contains(SkillEffect.IncreaseRange) ||
                _minionEffects.Contains(SkillEffect.DecreaseSpeed) ||
                _minionEffects.Contains(SkillEffect.IncreaseSpeed) ||
                _towerEffects.Contains(SkillEffect.DecreaseDamage) ||
                _towerEffects.Contains(SkillEffect.IncreaseDamage) ||
                _towerEffects.Contains(SkillEffect.DecreaseRange) ||
                _towerEffects.Contains(SkillEffect.IncreaseRange) ||
                _towerEffects.Contains(SkillEffect.DecreaseSpeed) ||
                _towerEffects.Contains(SkillEffect.IncreaseSpeed))
                {
                return 1.1; 
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
            return _effectsMinion;
        }

        public bool DoesEffectTower(){
            return _effectsTower;
        }

        public List<SkillEffect> GetMinionEffects(){
            return _minionEffects;
        }
        
        public List<SkillEffect> GetTowerEffects(){
            return _towerEffects;
        }

    }
}
