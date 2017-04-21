using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Runtime.Serialization;
using Assets.Scripts.Manager;
using Assets.Scripts.UI;
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
        [DataMember]
        private float _fireBonus = 0;
        [DataMember]
        private float _earthBonus = 0;
        [DataMember]
        private float _airBonus = 0;
        [DataMember]
        private float _waterBonus = 0;
        [DataMember]
        private Dictionary<AchievementType, BigIntWithUnit> _achievementKeeper;
        [DataMember]
        private string _currentSceneName;

        private List<Mage> _mageObjectList;
       

        public PlayerData(Element element)
        {
            _fireBonus = UpgradeManager.BonusFireMultiplier;
            _airBonus = UpgradeManager.BonusAirMultiplier;
            _earthBonus = UpgradeManager.BonusEarthMultiplier;
            _waterBonus = UpgradeManager.BonusWaterMultiplier;

            _playerLevel = 1;
            _spellDamage = UpgradeManager.PlayerDamageInitial * ElementController.Instance.GetPlayerBonusMultiplier(element);
            _spellSpeed = 100;
            _currency = 0;
            _pricePlayerSpellUpgrade = UpgradeManager.MageUpgradePriceInitial;
            _element = element;
            _priceIdleGeneratedUpgrade = UpgradeManager.MageIdleGenerationUpgradePriceInitial;
            _mageCap = 10;
            _mageList = new List<MageData>();
            _mageObjectList = new List<Mage>();
            _currentSceneName = SceneLoader.DefaultStartScene;
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

		public BigIntWithUnit GetIdleUpgradePrice()
		{
			return _priceIdleGeneratedUpgrade;
		}
		public string[] GetProfileInfo()
		{
			var specs = new string[7];
			specs[0] = _name;
			specs[1] = _playerLevel.ToString();
			specs[2] = _element.ToString();
			specs[3] = "Hi";
			specs[4] = _spellDamage.ToString();
			specs[5] = "How fast can you tap?";
			specs[6] = "Pretty much infinite";
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
            _pricePlayerSpellUpgrade *= UpgradeManager.MageUpgradePriceMultiplier;
        }

        public void UpgradeIdleGenerated() {
           
            if (_currency < _priceIdleGeneratedUpgrade) return;
            //Upgrade
            foreach (var mage in _mageList) {
                mage.UpgradeIdleCurrency();
            }

            //Scaling
            _priceIdleGeneratedUpgrade *= UpgradeManager.MageUpgradePriceMultiplier;
        } 
           
        public void UpdateBonusMultipliers()
        {
            if (_fireBonus == 0)
            {
                return;
            }
            UpgradeManager.BonusFireMultiplier = _fireBonus;
            UpgradeManager.BonusAirMultiplier = _airBonus;
            UpgradeManager.BonusEarthMultiplier = _earthBonus;
            UpgradeManager.BonusWaterMultiplier = _waterBonus;
        }

        //resets the player data to beginning state
        public void ResetPlayer()
        {
            UpdateBonusMultipliers();

            _spellDamage = UpgradeManager.PlayerDamageInitial;
            if(_element == Element.Fire)
            {
                _spellDamage *= _fireBonus;
            }
            else if(_element == Element.Air)
            {
                _spellDamage *= _airBonus;
            }
            else if (_element == Element.Earth)
            {
                _spellDamage *= _earthBonus;
            }
            else if (_element == Element.Water)
            {
                _spellDamage *= _waterBonus;
            }
            _spellSpeed = 100;
			_playerLevel = 1;
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
                    _mageObjectList[i].Eject(false);
                }
                GameObject.Destroy(_mageObjectList[i].gameObject);
            }
            _mageObjectList.Clear();
            _mageList.Clear();

        }

        public void CreateMagesFromDataArray(MageFactory mageFactory)
        {
            _mageObjectList = new List<Mage>();
            for (var i = 0; i < _mageList.Count; i++)
            {
                var mage = mageFactory.CreateMage(i, _mageList[i]);
                _mageObjectList.Add(mage);
            }
        }

        public void PutMagesToBuildings(MageAssignableBuilding[] allAssignableBuildings)
        {
            foreach (var mage in _mageObjectList)
            {
                var buildingId = mage.Data.GetBuildingId();
                if (buildingId != null)
                    mage.PutIntoBuilding(allAssignableBuildings[buildingId.Value]);
            }
        }

        public Mage RecreateMage(int id, MageFactory mageFactory, MageAssignableBuilding[] allAssignableBuildings)
        {
            var mage = mageFactory.CreateMage(id, _mageList[id]);
            GameObject.Destroy(_mageObjectList[id].gameObject);
            _mageObjectList.RemoveAt(id);
            _mageObjectList.Insert(id, mage);
            var buildingId = mage.Data.GetBuildingId();
            if (buildingId != null)
            {
                allAssignableBuildings[buildingId.Value].EjectMageInside();
                mage.PutIntoBuilding(allAssignableBuildings[buildingId.Value]);
            }
            return mage;
        }

        //gets the player input at the beginning of the game and sets the element accordingly
        //also destroys the UI element for element selection
        public void SetPlayerElement(Element element)
        {
            _element = element;
            var menu = GameObject.FindGameObjectWithTag("ElementPanel");
            if(menu)
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

        public void SetFireBonus(float fireBonus)
        {
            _fireBonus = fireBonus;
        }

        public float GetFireBonus()
        {
            return _fireBonus;
        }

        public void SetAirBonus(float airBonus)
        {
            _airBonus = airBonus;
        }

        public float GetAirBonus()
        {
            return _airBonus;
        }

        public void SetWaterBonus(float waterBonus)
        {
            _waterBonus = waterBonus;
        }

        public float GetWaterBonus()
        {
            return _waterBonus;
        }

        public void SetEarthBonus(float earthBonus)
        {
            _earthBonus = earthBonus;
        }

        public float GetEarthBonus()
        {
            return _earthBonus;
        }

        public void SetAchievementData(Dictionary<AchievementType, BigIntWithUnit> achievementData)
        {
            _achievementKeeper = achievementData;
        }

        public Dictionary<AchievementType, BigIntWithUnit> GetAchievementData()
        {
            return _achievementKeeper;
        }

        public string GetLoadedScene()
        {
            return _currentSceneName;
        }

        public void SetCurrentScene(string sceneName)
        {
            _currentSceneName = sceneName;
        }
    }
}