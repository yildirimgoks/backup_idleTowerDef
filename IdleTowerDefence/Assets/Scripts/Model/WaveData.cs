using System.Collections.Generic;
using System.Runtime.Serialization;
using Assets.Scripts.Manager;

namespace Assets.Scripts.Model
{
    public struct SingleWaveInfo
    {
        public string Type;
        public bool BossWave;
        public bool MageDropWave;
        public int Count;
        public float Speed;
        public BigIntWithUnit CurrencyOnDeath;
        public BigIntWithUnit Life;
    }

    [DataContract]
    public class WaveData
    {
        [DataMember]
        private int _currentWave;

        [DataMember]
        private int _maxWave;

        [DataMember]
        private int _mageDropInterval;

        private List<SingleWaveInfo> _waveInfos;

        public WaveData()
        {
            _currentWave = 0;
            _maxWave = 0;
            _mageDropInterval = 10;
        }

        public void ReadWaveInfo(List<SingleWaveInfo> waveInfo )
        {
            _waveInfos = waveInfo;
        }
    
        public bool IsBossWave
        {
            get { return _waveInfos[CurrentWave % _waveInfos.Count].BossWave; }
        }

        public bool IsNextWaveBossWave
        {
            get { return _waveInfos[(CurrentWave + 1) % _waveInfos.Count].BossWave; }
        }

		public bool IsPreviousWaveBossWave
		{
			get { return _waveInfos[(CurrentWave - 1) % _waveInfos.Count].BossWave; }
		}

        public bool IsDropWave
        {
            get { return _waveInfos[(CurrentWave) % _waveInfos.Count].MageDropWave; }
        }

        public string CurrentWaveMinionType
        {
            get { return _waveInfos[CurrentWave%_waveInfos.Count].Type; }
        }

        public int CurrentWave
        {
            get { return _currentWave; }
        }

        public void IncreaseCurrentWaveAndMaxWave()
        {
            if (_maxWave == _currentWave)
            {
                _maxWave += 1;
            }
            _currentWave += 1;
        }

        public bool IncreaseCurrentWaveIfLessThanMax()
        {
            if (_maxWave <= _currentWave) return false;
            _currentWave += 1;
            return true;
        }

        public void DecreaseCurrentWave()
        {
            _currentWave -= 1;
        }

        public void DecreaseMaxWave()
        {
            _maxWave -= 1;
        }
        
        public int GetCurrentWaveLength()
        {
            return _waveInfos[CurrentWave % _waveInfos.Count].Count;
        }

        public int GetMaxReachedWave()
        {
            return _maxWave;
        }

        public int GetMageDropInterval()
        {
            return _mageDropInterval;
        }

        public MinionData GetMinionDataForCurrentWave()
        {
            if (CurrentWave <= _waveInfos.Count)
            {
                return new MinionData(_waveInfos[CurrentWave].Life, _waveInfos[CurrentWave].CurrencyOnDeath, _waveInfos[CurrentWave].Speed);
            } else {
                
                var multiplierLife = System.Math.Pow(UpgradeManager.MinionLifeMultiplier, CurrentWave);

                if (IsBossWave)
                {
                    var bossLife = (BigIntWithUnit) (System.Math.Ceiling(multiplierLife)*1000);
                    return new MinionData(bossLife, bossLife, 10f);
                }

                var life = UpgradeManager.MinionLifeInitial*multiplierLife;

                var multiplierMoney = System.Math.Pow(UpgradeManager.MinionDeathRewardMultiplier, CurrentWave);
                var currencyGivenOnDeath = UpgradeManager.MinionDeathRewardInitial*multiplierMoney;

                return new MinionData(life, currencyGivenOnDeath, 10f);
            }
        }

        public void ResetWave()
        {
            _maxWave = 0;
            _currentWave = 0;
        }
    }
}
