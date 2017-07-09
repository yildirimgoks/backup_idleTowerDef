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
        private float _fireBonus;
        [DataMember]
        private float _earthBonus;
        [DataMember]
        private float _airBonus;
        [DataMember]
        private float _waterBonus;
        [DataMember]
        private Dictionary<AchievementType, BigIntWithUnit> _achievementKeeper;
        [DataMember]
        private string _currentSceneName;
        [DataMember]
        private int _resetAmount;

        private List<Mage> _mageObjectList;
       

        public PlayerData(Element element)
        {
            _fireBonus = _airBonus = _earthBonus = _waterBonus = 1.0f;

            _playerLevel = 1;
            _spellDamage = UpgradeManager.PlayerDamageInitial;
            _spellSpeed = 10;
            _currency = 0;
            _pricePlayerSpellUpgrade = UpgradeManager.MageUpgradePriceInitial;
            _element = element;
            _priceIdleGeneratedUpgrade = UpgradeManager.MageIdleGenerationUpgradePriceInitial;
            _mageCap = 10;
            _mageList = new List<MageData>();
            _mageObjectList = new List<Mage>();
            _currentSceneName = SceneLoader.DefaultStartScene;
            _resetAmount = 0;
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

        //resets the player data to beginning state
        public void ResetPlayer(Element bonusElement)
        {
            _spellDamage = UpgradeManager.PlayerDamageInitial;
            _spellSpeed = 10;
            _playerLevel = 1;
            _resetAmount += 1;
            _pricePlayerSpellUpgrade = UpgradeManager.MageUpgradePriceInitial;
            _element = bonusElement;
            switch (bonusElement)
            {
                case Element.Air:
                    _airBonus *= 1.5f;
                    _spellDamage *= _airBonus;
                    break;
                case Element.Earth:
                    _earthBonus *= 1.5f;
                    _spellDamage *= _earthBonus;
                    break;
                case Element.Fire:
                    _fireBonus *= 1.5f;
                    _spellDamage *= _fireBonus;
                    break;
                case Element.Water:
                    _waterBonus *= 1.5f;
                    _spellDamage *= _waterBonus;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("bonusElement", bonusElement, null);
            }
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

        public void InitializeMageDataArrayForStartup(MageFactory mageFactory)
        {
            for (var i = 0; i < 1; i++)
            {
                var mage = mageFactory.CreateMageData(this._element);
                mage.SetState(MageState.Idle);
                _mageList.Add(mage);
            }
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
			var oldMage = _mageObjectList[id];
			mage.CopyAutoUpgradeTimers(oldMage);
			GameObject.Destroy(oldMage.gameObject);
	
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
            if (menu)
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

        public float GetElementBonus(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return _fireBonus;
                case Element.Water:
                    return _waterBonus;
                case Element.Earth:
                    return _earthBonus;
                case Element.Air:
                    return _airBonus;
                default:
                    throw new ArgumentOutOfRangeException("element", element, null);
            }
        }

        public int GetResetAmount()
        {
            return _resetAmount;
        }
    }
}