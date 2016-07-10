using System.Collections.Generic;
//using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Policy;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [DataContract]
    public class PlayerData
    {
        [DataMember]
        private BigIntWithUnit _spellDamage;
        [DataMember]
        private int _spellSpeed;
        [DataMember]
        private Element _element;
        [DataMember]
        private readonly BigIntWithUnit _pricePlayerSpellUpgrade;
        [DataMember]
        private float _upgradeLevelPlayerSpell;
        [DataMember]
        private BigIntWithUnit _currency;
        [DataMember]
        private List<MageData> _mageList;
        [DataMember]
        private WaveData _waveData;

        private List<Mage> _mageObjectList;

        public PlayerData(BigIntWithUnit spellDamage, int spellSpeed, BigIntWithUnit currency, 
            BigIntWithUnit pricePlayerSpellUpgrade, float upgradeLevelPlayerSpell, Element element)
        {
            _spellDamage = spellDamage;
            _spellSpeed = spellSpeed;
            _currency = currency;
            _pricePlayerSpellUpgrade = pricePlayerSpellUpgrade;
            _upgradeLevelPlayerSpell = upgradeLevelPlayerSpell;
            _element = element;

            _mageList = new List<MageData>();
            _mageObjectList = new List<Mage>();
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

        public IEnumerable<Mage> GetMages()
        {
            return _mageObjectList;
        }

        public BigIntWithUnit GetUpgradePrice()
        {
            return _pricePlayerSpellUpgrade;
        }

        public WaveData GetWaveData()
        {
            return _waveData;
        }

        public void SetWaveData(WaveData data)
        {
            _waveData = data;
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

        public void AddMage(Mage mage)
        {
            _mageObjectList.Add(mage);
            _mageList.Add(mage.Data);
        }

        public void CreateMagesFromDataArray(MageFactory mageFactory)
        {
            _mageObjectList = new List<Mage>();
            for (int i = 0; i < _mageList.Count; i++)
            {
                var mage = mageFactory.CreateMage(6.1f, 13 + 4 * i, _mageList[i]);
                _mageObjectList.Add(mage);
            }
        }

        //gets the player input at the beginning of the game and sets the element accordingly
        //also destroys the UI element for element selection
        public void SetPlayerElement(Element element)
        {
            _element = element;
            var menu = GameObject.FindGameObjectWithTag("InitPanel");
            menu.SetActive(false);
        }
    }
}