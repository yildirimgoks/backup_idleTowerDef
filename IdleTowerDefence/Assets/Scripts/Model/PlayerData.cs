using System.Collections.Generic;
//using System.Diagnostics;
using System.Runtime.Serialization;
using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [DataContract]
    public class PlayerData
    {
		[DataMember]
		private int _playerLevel;
        [DataMember]
        private BigIntWithUnit _spellDamage;
        [DataMember]
        private int _spellSpeed;
        [DataMember]
        private Element _element;
        [DataMember]
        private BigIntWithUnit _pricePlayerSpellUpgrade;
        [DataMember]
        private BigIntWithUnit _currency;
        [DataMember]
        private List<MageData> _mageList;
        [DataMember]
        private WaveData _waveData;
        [DataMember]
        private string _name;
        [DataMember]
        private BigIntWithUnit _priceIdleGeneratedUpgrade;
        [DataMember]
        private int _mageCap;

        private List<Mage> _mageObjectList;
       

        public PlayerData(Element element)
        {
			_playerLevel = 1;
            _spellDamage = UpgradeManager.PlayerDamageInitial;
            _spellSpeed = 50;
            _currency = 0;
            _pricePlayerSpellUpgrade = UpgradeManager.MageUpgradePriceInitial;
            _element = element;
            _priceIdleGeneratedUpgrade = UpgradeManager.MageIdleGenerationUpgradePriceInitial;
            _mageCap = 9;

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

		public string[] GetProfileInfo()
		{
			var specs = new string[7];
			specs[0] = _name;
			specs[1] = _playerLevel.ToString();
			specs[2] = _element.ToString();
			specs[3] = "Meraba";
			specs[4] = _spellDamage.ToString();
			specs[5] = "As hard as you touch me";
			specs[6] = "Burdan taa karşıki dağlara kadar";
			return specs;
		}

		public Element GetElement()
		{
			return _element;
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
            _spellDamage *= UpgradeManager.PlayerDamageMultiplier;
			_playerLevel += 1;

            //Scaling
            DecreaseCurrency(_pricePlayerSpellUpgrade);
            _pricePlayerSpellUpgrade *= UpgradeManager.MageUpgradePriceMultiplier;
        }

        public void UpgradeIdleGenerated() {
           
            if (_currency < _priceIdleGeneratedUpgrade) return;
            //Upgrade
            foreach (var mage in _mageList) {
                mage.UpgradeIdleCurrency();
            }
            
            //Scaling
            DecreaseCurrency(_priceIdleGeneratedUpgrade);
            _priceIdleGeneratedUpgrade *= UpgradeManager.MageUpgradePriceMultiplier;
        } 
           

        //resets the player data to beginning state
        public void ResetPlayer()
        {
            _spellDamage = UpgradeManager.PlayerDamageInitial;
            _spellSpeed = 100;
            _pricePlayerSpellUpgrade = UpgradeManager.MageUpgradePriceInitial;
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
                if (mage.IsInTower())
                {
                    result += mage.IndividualDps();
                }
            }
            return result;
        }

		public BigIntWithUnit CumulativeIdleEarning()
		{
			BigIntWithUnit result = 0;
			foreach (MageData mage in _mageList)
			{
				result += mage.GetIdleCurrency();
			}
			return result;
		}

        public void AddMage(Mage mage)
        {
            _mageObjectList.Add(mage);
            _mageList.Add(mage.Data);
        }

        public bool IsMageListFull()
        {
            return _mageList.Count == _mageCap;
        }

        public void DestroyMages()
        {
            var magecount = _mageList.Count;
            for (int i = 0; i < magecount; i++)
            {
                if (_mageObjectList[i].Data.IsActive())
                {
                    _mageObjectList[i].Eject();
                }
                GameObject.Destroy(_mageObjectList[i].gameObject);
            }
            _mageObjectList.Clear();
            _mageList.Clear();

        }

        public void CreateMagesFromDataArray(MageFactory mageFactory, MageAssignableBuilding[] allAssignableBuildings)
        {
            _mageObjectList = new List<Mage>();
            for (int i = 0; i < _mageList.Count; i++)
            {
                var mage = mageFactory.CreateMage(12.5f, 1 + 10 * i, _mageList[i]);
                _mageObjectList.Add(mage);
                var buildingId = mage.Data.GetBuildingId();
                if (buildingId != null)
                    mage.PutIntoBuilding(allAssignableBuildings[buildingId.Value]);
            }
        }

        //gets the player input at the beginning of the game and sets the element accordingly
        //also destroys the UI element for element selection
        public void SetPlayerElement(Element element)
        {
            _element = element;
            var menu = GameObject.FindGameObjectWithTag("ElementPanel");
            menu.SetActive(false);
        }

        public void SetPlayerName(string name)
        {
            _name = name;
            var menu = GameObject.FindGameObjectWithTag("NamePanel");
            menu.SetActive(false);
        }

        public string GetPlayerName()
        {
            return _name;
        }
    }
}