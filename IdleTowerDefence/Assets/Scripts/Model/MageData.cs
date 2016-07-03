using System;
using System.Runtime.Serialization;
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
        private int _spellRange;
        [DataMember]
        private Element _element;
        [DataMember]
        private float _delay;
        [DataMember]
        private MageState _currentState;
        [DataMember]
        private int? _towerId;
        [DataMember]
        private int? _shrineId;
        [DataMember]
        private int _mageLevel;
        [DataMember]
        private BigIntWithUnit _upgradePrice;
        [DataMember]
        private double _damageMultiplier;
        [DataMember]
        private double _rangeMultiplier;
        [DataMember]
        private double _rateMultiplier;
        [DataMember]
        private int _maxRange;
        [DataMember]
        private float _minDelay;
        
        public MageData(string name, string line, BigIntWithUnit spellDamage, int spellSpeed, int spellRange, 
            Element element, float delay, MageState currentState, int tower, int shrine, int mageLevel, 
            BigIntWithUnit upgradePrice, double damageMultiplier, double rangeMultiplier, double rateMultiplier, int maxRange, float minDelay)
        {
            _name = name;
            _line = line;
            _spellDamage = spellDamage;
            _spellSpeed = spellSpeed;
            _spellRange = spellRange;
            _element = element;
            _delay = delay;
            _currentState = currentState;
            _towerId = tower;
            _shrineId = shrine;
            _mageLevel = mageLevel;
            _upgradePrice = upgradePrice;
            _damageMultiplier = damageMultiplier;
            _rangeMultiplier = rangeMultiplier;
            _rateMultiplier = rateMultiplier;
            _maxRange = maxRange;
            _minDelay = minDelay;
        }

        public MageData(string name, string line, Element element)
        {
            _name = name;
            _line = line;
            _element = element;
            _currentState = MageState.Dropped;
            _towerId = null;
            _shrineId = null;
            _mageLevel = 1;

            _damageMultiplier = ElementController.Instance.GetDamageMultiplier(element);
            _rangeMultiplier = ElementController.Instance.GetRangeMultiplier(element);
            _rateMultiplier = ElementController.Instance.GetDelayMultiplier(element);

            //ToDo: Make Element dependent
            _spellDamage = 20;
            _spellSpeed = 70;
            _spellRange = 11;
            _delay = 1;
            _upgradePrice = 100;
            _maxRange = 30;
            _minDelay = 0.1f;
        }

        public BigIntWithUnit GetSpellDamage()
        {
            return _spellDamage;
        }

        public int GetSpellSpeed()
        {
            return _spellSpeed;
        }

        public int GetSpellRange()
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
            _spellDamage += (int)(20 * System.Math.Pow(_damageMultiplier, _mageLevel));
        }

        public void IncreaseSpellRate()
        {
            _delay /= (float)(1.2f * System.Math.Pow(_rateMultiplier, _mageLevel));
            _delay = Math.Max(_delay, _minDelay);
        }

        public void IncreaseSpellRange()
        {
            _spellRange += (int)(2 * System.Math.Pow(_rangeMultiplier, _mageLevel));
            _spellRange = Math.Min(_spellRange, _maxRange);
        }

        public BigIntWithUnit IndividualDps()
        {
            return BigIntWithUnit.MultiplyPercent(_spellDamage, 100 / _delay);
        }

        public void UpgradeMage()
        {
            IncreaseSpellDamage();
            IncreaseSpellRange();
            IncreaseSpellRate();
        
            _mageLevel++;
            _upgradePrice = BigIntWithUnit.MultiplyPercent(_upgradePrice, System.Math.Pow(1.1, _mageLevel) * 100);
        }

        public string[] GetProfileInfo()
        {
            var specs = new string[4];
            specs[0] = _mageLevel.ToString();
            specs[1] = _spellDamage.ToString();
            specs[2] = (1 / _delay).ToString();
            specs[3] = _spellRange.ToString();
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
    }
}