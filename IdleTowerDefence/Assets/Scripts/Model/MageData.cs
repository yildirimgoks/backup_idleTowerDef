using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public struct MageUpgradeInfo
    {
        public int Type;
        public BigIntWithUnit SpellDamage;
        public float SpellRange;
        public float Delay;
        public BigIntWithUnit UpgradePrice;
        public BigIntWithUnit SkillDamage;
    }
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
        private float _skillEffect;
        [DataMember]
        private int _prefabId;

        public GameObject ProfileButton;
        public int ProfileButtonIndex;

        //Upgrade Data
        private static readonly Dictionary<Element, List<MageUpgradeInfo>> UpgradeInfo = new Dictionary<Element, List<MageUpgradeInfo>>();

        public MageData(string name, string line, Element element)
        {
            _name = name;
            _line = line;
            _element = element;
            _currentState = MageState.Dropped;
            _buildingId = null;
            _mageLevel = 0;
            
            _skillEffect = ElementController.Instance.GetSkillPowerInitial(element);

            SetValuesForMageLevel();

            //ToDo: Make Element dependent
            _spellSpeed = 90;
            _maxRange = 30;
            _minDelay = 0.1f;
			_idleCurrency = UpgradeManager.MageIdleGenerationInitial;
            _skillCoolDown = 10;
            _minSkillCoolDown = 1;
            _skillRange = 15;
            _prefabId = 0;
        }

        private void SetValuesForMageLevel()
        {
            if (UpgradeInfo[_element].Count <= _mageLevel)
            {
                IncreaseSpellDamage();
                IncreaseSpellRange();
                IncreaseSpellRate();
                IncreaseSkillDamage();
                _upgradePrice *= UpgradeManager.MageUpgradePriceMultiplier;
                return;
            }
            _prefabId = UpgradeInfo[_element][_mageLevel].Type;
            _spellDamage = UpgradeInfo[_element][_mageLevel].SpellDamage;
            _spellRange = UpgradeInfo[_element][_mageLevel].SpellRange;
            _delay = UpgradeInfo[_element][_mageLevel].Delay;
            _skillDamage = UpgradeInfo[_element][_mageLevel].SkillDamage;
            _upgradePrice = UpgradeInfo[_element][_mageLevel].UpgradePrice;
            _spellDamage *= ElementController.Instance.GetPlayerBonusMultiplier(_element);

        }

        public static void ReadUpgradeInfo(Element element, List<MageUpgradeInfo> info)
        {
            UpgradeInfo.Add(element, info);
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
            _mageLevel++;
            SetValuesForMageLevel();
        }

        private void IncreaseSkillDamage()
        {
            _skillDamage *= ElementController.Instance.GetSkillDamageMultiplier(_element);
        }

        public string[] GetProfileInfo()
        {
            var specs = new string[7];
			specs[0] = _name;
			specs[1] = (_mageLevel + 1).ToString();
			specs[2] = _element.ToString();
			specs[3] = _line;
			specs[4] = _spellDamage.ToString();
			specs[5] = (1 / _delay).ToString("F1");
			specs[6] = _spellRange.ToString("F1");
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

        public float GetDelay()
        {
            return _delay;
        }

        public int GetPrefabId()
        {
            return _prefabId;
        }
    }
}