using System;
using System.Runtime.Serialization;
using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public enum MageState
    {
        Idle,
        //Working,
        Active,
        Dragged,
        Dropped
    }

    [DataContract]
    public class MageData
    {
        [DataMember]
        private string _name;
        [DataMember]
        private string _line;
        [DataMember]
        private BigIntWithUnit _spellDamage;
        [DataMember]
        private int _spellSpeed;
        [DataMember]
        private float _spellRange;
        [DataMember]
        private Element _element;
        [DataMember]
        private float _delay;
        [DataMember]
        private MageState _currentState;
        [DataMember]
        private int? _buildingId;
        [DataMember]
        private int _mageLevel;
        [DataMember]
        private BigIntWithUnit _upgradePrice;
        [DataMember]
        private float _damageMultiplier;
        [DataMember]
        private float _rangeMultiplier;
        [DataMember]
        private float _rateMultiplier;
        [DataMember]
        private int _maxRange;
        [DataMember]
        private float _minDelay;
		[DataMember]
		private BigIntWithUnit _idleCurrency;
        [DataMember]
        private float _skillCoolDown;
        [DataMember]
        private float _minSkillCoolDown;
        [DataMember]
        private BigIntWithUnit _skillDamage;
        [DataMember]
        private int _skillRange;
        [DataMember]
        private int _maxSkillRange;
        [DataMember]
        private BigIntWithUnit _skillUpgradePrice;
        [DataMember]
        private int _skillLevel;
        
        public MageData(string name, string line, Element element)
        {
            _name = name;
            _line = line;
            _element = element;
            _currentState = MageState.Dropped;
            _buildingId = null;
            _mageLevel = 1;

            _damageMultiplier = ElementController.Instance.GetDamageMultiplier(element);
            _rangeMultiplier = ElementController.Instance.GetRangeMultiplier(element);
            _rateMultiplier = ElementController.Instance.GetDelayMultiplier(element);

            _skillLevel = 1;
            
            _spellDamage = ElementController.Instance.GetDamageInitial(element);
            _spellRange = ElementController.Instance.GetRangeInitial(element);
            _delay = ElementController.Instance.GetDelayInitial(element);

            //ToDo: Make Element dependent
            _spellSpeed = 70;
            _upgradePrice = UpgradeManager.MageUpgradePriceInitial;
            _maxRange = 30;
            _minDelay = 0.1f;
			_idleCurrency = UpgradeManager.MageIdleGenerationInitial;
            _skillDamage = 50;
            _skillCoolDown = 10;
            _minSkillCoolDown = 1;
            _skillRange = 15;
            _maxSkillRange = 30;
            _skillUpgradePrice = 100;
        }

        public BigIntWithUnit GetSpellDamage()
        {
            return _spellDamage;
        }

        public int GetSpellSpeed()
        {
            return _spellSpeed;
        }

        public float GetSpellRange()
        {
            return _spellRange;
        }

        public MageState GetCurrentState()
        {
            return _currentState;
        }

        public int GetLevel()
        {
            return _mageLevel;
        }

        public BigIntWithUnit GetUpgradePrice()
        {
            return _upgradePrice;
        }

        public BigIntWithUnit GetSkillUpgradePrice()
        {
            return _skillUpgradePrice;
        }

        public bool IsInTower()
        {
            //I don't like this hack at all, but since we won't change the amount of towers this should be ok for now
            return _buildingId <= 5;
        }

        public bool IsActive()
        {
            return _currentState == MageState.Active;
        }

        public bool IsDropped()
        {
            return _currentState == MageState.Dropped;
        }

        public bool IsDragged()
        {
            return _currentState == MageState.Dragged;
        }

        public bool IsIdle()
        {
            return _currentState == MageState.Idle;
        }

        public void SetState(MageState state)
        {
            _currentState = state;
        }

        public float NextSpellTime()
        {
            return Time.time + _delay;
        }

        public SpellData GetSpellData()
        {
            return new SpellData(_spellDamage, _spellSpeed, _element);
        }

        public Element GetElement()
        {
            return _element;
        }

        public void IncreaseSpellDamage()
        {
            _spellDamage *= _damageMultiplier;
        }

        public void IncreaseSpellRate()
        {
            _delay /= _rateMultiplier;
            _delay = Math.Max(_delay, _minDelay);
        }

        public void IncreaseSpellRange()
        {
            _spellRange = (int) (_spellRange * _rangeMultiplier);
            _spellRange = Math.Min(_spellRange, _maxRange);
        }

        public BigIntWithUnit IndividualDps()
        {
            return _spellDamage / _delay;
        }

        public void UpgradeMage()
        {
            IncreaseSpellDamage();
            IncreaseSpellRange();
            IncreaseSpellRate();
        
            _mageLevel++;
            _upgradePrice = _upgradePrice * System.Math.Pow(UpgradeManager.MageUpgradePriceMultiplier, _mageLevel);
        }

        public void UpgradeSkill()
        {
            IncreaseSkillDamage();
            IncreaseSkillRange();
            DecreaseSkillCoolDown();
            _skillLevel++;
            _skillUpgradePrice *= System.Math.Pow(1.1, _skillLevel);
        }

        public void IncreaseSkillDamage()
        {
            _skillDamage += (int)(30 * System.Math.Pow(_damageMultiplier, _skillLevel));
        }

        public void IncreaseSkillRange()
        {
            _skillRange += (int)(3 * System.Math.Pow(_rangeMultiplier, _skillLevel));
            _skillRange = Math.Min(_skillRange, _maxSkillRange);
        }

        public void DecreaseSkillCoolDown()
        {
            _skillCoolDown /= (float)(1.1f * System.Math.Pow(_rateMultiplier, _skillLevel));
            _skillCoolDown = Math.Max(_skillCoolDown, _minSkillCoolDown);
        }

        public string[] GetProfileInfo()
        {
            var specs = new string[7];
			specs[0] = _name;
			specs[1] = _mageLevel.ToString();
			specs[2] = _element.ToString();
			specs[3] = _line;
			specs[4] = _spellDamage.ToString();
			specs[5] = (1 / _delay).ToString("F1");
			specs[6] = _spellRange.ToString();
            return specs;
        }

        public string GetName()
        {
            return _name;
        }

        public string GetLine()
        {
            return _line;
        }

        public SkillData GetSkillData()
        {
            return new SkillData(_element, _skillDamage, _skillRange, _spellSpeed);
        }

        public BigIntWithUnit GetIdleCurrency()
        {
			if (IsIdle()) {
				return _idleCurrency;
			}
			return 0;
        }

        public void UpgradeIdleCurrency()
        {
            _idleCurrency *= UpgradeManager.MageIdleGenerationMultiplier;
        }

        public bool OccupyBuilding(int id)
        {
            if (_buildingId != null) return false;
            _buildingId = id;
            return true;
        }

        public void EjectFromOccupiedBuilding()
        {
            _buildingId = null;
        }

        public int? GetBuildingId()
        {
            return _buildingId;
        }

        public float GetDelay(){
            return _delay;
        }
    }
}