using System.Runtime.Serialization;
using Assets.Scripts.Manager;

namespace Assets.Scripts.Model
{
    [DataContract]
    public class WaveData
    {
        [DataMember]
        private int _currentWave;

        [DataMember]
        private int _maxWave;

        [DataMember]
        private int _mageDropInterval;

        public WaveData()
        {
            _currentWave = 0;
            _maxWave = 0;
            _mageDropInterval = 10;
        }
    
        public bool IsBossWave
        {
            get { return (_currentWave + 1) % 5 == 0; }
        }

        public bool IsNextWaveBossWave
        {
            get { return (_currentWave + 1) % 5 == 4; }
        }

        public bool IsDropWave
        {
            get { return (_currentWave + 1) % _mageDropInterval == 0; }
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

        //ToDo: fix when waves are customized
        public int GetCurrentWaveLength()
        {
            return 10;
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
            var multiplierLife = System.Math.Pow(UpgradeManager.MinionLifeMultiplier, CurrentWave);

            if (IsBossWave)
            {
                var bossLife = (BigIntWithUnit) (System.Math.Ceiling(multiplierLife) * 1000);
                return new MinionData(bossLife, bossLife, 10f);
            }

            var life = UpgradeManager.MinionLifeInitial * multiplierLife;

            var multiplierMoney = System.Math.Pow(UpgradeManager.MinionDeathRewardMultiplier, CurrentWave);
            var currencyGivenOnDeath = UpgradeManager.MinionDeathRewardInitial * multiplierMoney;

            return new MinionData(life, currencyGivenOnDeath, 10f);
        }

        public void ResetWave()
        {
            _maxWave = 0;
            _currentWave = 0;
        }
    }
}
