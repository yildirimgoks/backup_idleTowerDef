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
        private float _skillEffect;

        public MageData(string name, string line, Element element)
        {
            _name = name;
            _line = line;
            _element = element;
            _currentState = MageState.Dropped;
            _buildingId = null;
            _mageLevel = 1;
            
            _spellDamage = ElementController.Instance.GetDamageInitial(element);
            _spellRange = ElementController.Instance.GetRangeInitial(element);
            _delay = ElementController.Instance.GetDelayInitial(element);
            _skillDamage = ElementController.Instance.GetSkillDamageInitial(element);
            _skillEffect = ElementController.Instance.GetSkillPowerInitial(element);

            //ToDo: Make Element dependent
            _spellSpeed = 70;
            _upgradePrice = UpgradeManager.MageUpgradePriceInitial;
            _maxRange = 30;
            _minDelay = 0.1f;
			_idleCurrency = UpgradeManager.MageIdleGenerationInitial;
            _skillCoolDown = 10;
            _minSkillCoolDown = 1;
            _skillRange = 15;
            _maxSkillRange = 30;
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

        private void IncreaseSpellDamage()
        {
            _spellDamage *= ElementController.Instance.GetDamageMultiplier(_element);
        }

        private void IncreaseSpellRate()
        {
            _delay /= ElementController.Instance.GetDelayMultiplier(_element);
            _delay = Math.Max(_delay, _minDelay);
        }

        private void IncreaseSpellRange()
        {
            _spellRange = (int) (_spellRange * ElementController.Instance.GetRangeMultiplier(_element));
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
            UpgradeSkill();
        
            _mageLevel++;
            _upgradePrice *= UpgradeManager.MageUpgradePriceMultiplier;
        }

        private void UpgradeSkill()
        {
            IncreaseSkillDamage();
            IncreaseSkillRange();
            IncreaseSkillEffect();
            DecreaseSkillCoolDown();
        }

        private void IncreaseSkillDamage()
        {
            _skillDamage *= ElementController.Instance.GetSkillDamageMultiplier(_element);
        }

        private void IncreaseSkillRange()
        {
            _skillRange = (int)(_skillRange * ElementController.Instance.GetRangeMultiplier(_element));
            _skillRange = Math.Min(_skillRange, _maxSkillRange);
        }

        private void IncreaseSkillEffect()
        {
            _skillEffect *= ElementController.Instance.GetSkillPowerMultiplier(_element);
        }

        private void DecreaseSkillCoolDown()
        {
            _skillCoolDown *= ElementController.Instance.GetDelayMultiplier(_element);
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
            return new SkillData(_element, _skillDamage, _skillRange, _spellSpeed, _skillEffect);
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