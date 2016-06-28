using System;

namespace Assets.Scripts.Serialization
{
    [Serializable]
    public class MageSaveData
    {
        private string _name;
        private string _line;
        private BigIntWithUnit _spellDamage;
        private int _spellSpeed;
        private int _spellRange;
        private Element _element;
        private float _delay;
        private MageState _currentState;
        private Tower _tower;
        private Shrine _shrine;
        private int _mageLvl;
        private BigIntWithUnit _upgradePrice;
        private double DamageMultiplier;
        private double RangeMultiplier;
        private double RateMultiplier;

        public MageSaveData(Mage obj)
        {
            _name = obj.Name;
            _line = obj.Line;
            _spellDamage = obj.GetSpellDamage();
            _spellSpeed = obj.GetSpellSpeed();
            _spellRange = obj.GetSpellRange();
            _element = obj.Element;
            _delay = obj.Delay;
            _currentState = obj.CurrentState;
        }
    }
}