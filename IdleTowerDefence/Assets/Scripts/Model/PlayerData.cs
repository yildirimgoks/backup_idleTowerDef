using System;
using System.Collections.Generic;
using Assets.Scripts.Model;

namespace Assets.Scripts.Serialization
{
    [Serializable]
    public class PlayerData
    {
        private BigIntWithUnit _spellDamage;
        private int _spellSpeed;
        private Element _element;
        private readonly BigIntWithUnit _pricePlayerSpellUpgrade;
        private float _upgradeLevelPlayerSpell;

        private BigIntWithUnit _currency;
        private List<MageData> _mageList;

        public PlayerData(BigIntWithUnit spellDamage, int spellSpeed, BigIntWithUnit currency,
            List<MageData> mageList, BigIntWithUnit pricePlayerSpellUpgrade, float upgradeLevelPlayerSpell, Element element)
        {
            _spellDamage = spellDamage;
            _spellSpeed = spellSpeed;
            _currency = currency;
            _mageList = mageList;
            _pricePlayerSpellUpgrade = pricePlayerSpellUpgrade;
            _upgradeLevelPlayerSpell = upgradeLevelPlayerSpell;
            _element = element;
        }

        public void IncreaseCurrency(BigIntWithUnit amount)
        {
            _currency += amount;
        }

        public void DecreaseCurrency(BigIntWithUnit amount)
        {
            _currency -= amount;
        }

        public BigIntWithUnit GetCurrency()
        {
            return _currency;
        }

        public IEnumerable<MageData> GetMages()
        {
            return _mageList;
        }

        public BigIntWithUnit GetUpgradePrice()
        {
            return _pricePlayerSpellUpgrade;
        }

        public void UpgradePlayer()
        {
            if (_currency < _pricePlayerSpellUpgrade) return;
            //Upgrade
            _spellDamage += 5;

            //Scaling
            DecreaseCurrency(_pricePlayerSpellUpgrade);
            _upgradeLevelPlayerSpell = _upgradeLevelPlayerSpell * 1.1f;
            _pricePlayerSpellUpgrade.IncreasePercent((int)((_upgradeLevelPlayerSpell - 1) * 100));
        }

        public SpellData GetSpellData()
        {
            return new SpellData(_spellDamage, _spellSpeed, _element);
        }

        public BigIntWithUnit CumulativeDps()
        {
            BigIntWithUnit result = 0;
            foreach (MageData mage in _mageList)
            {
                if (mage.IsActive())
                {
                    result += mage.IndividualDps();
                }
            }
            return result;
        }

        public void AddMage(MageData mage)
        {
            _mageList.Add(mage);
        }
    }
}