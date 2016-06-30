using System;

namespace Assets.Scripts.Serialization
{
    [Serializable]
    public class MageData
    {
        private string _name;
        private string _line;
        private BigIntWithUnit _spellDamage;
        private int _spellSpeed;
        private int _spellRange;
        private Element _element;
        private float _delay;
        private MageState _currentState;
        private TowerData _tower;
        private ShrineData _shrine;
        private int _mageLvl;
        private BigIntWithUnit _upgradePrice;
        private double _damageMultiplier;
        private double _rangeMultiplier;
        private double _rateMultiplier;

        public MageData(Mage obj)
        {
            _name = obj.Name;
            _line = obj.Line;
            _spellDamage = obj.GetSpellDamage();
            _spellSpeed = obj.GetSpellSpeed();
            _spellRange = obj.GetSpellRange();
            _element = obj.Element;
            _delay = obj.Delay;
            _currentState = obj.CurrentState;
            _tower = new TowerData(obj.GetTower());
            _shrine = new ShrineData(obj.GetShrine());
            _mageLvl = obj.GetLevel();
            _upgradePrice = obj.GetUpgradePrice();
            _damageMultiplier = obj.DamageMultiplier;
            _rangeMultiplier = obj.RangeMultiplier;
            _rateMultiplier = obj.RateMultiplier;
        }
    }
}