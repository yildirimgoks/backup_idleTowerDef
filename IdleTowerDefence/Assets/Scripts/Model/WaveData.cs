using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Assets.Scripts.Manager;

namespace Assets.Scripts.Model
{
    public struct SingleWaveInfo
    {
        public string[] Type;
        public bool BossWave;
        public bool MageDropWave;
        public int[] Count;
        public float[] Speed;
        public BigIntWithUnit[] CurrencyOnDeath;
        public BigIntWithUnit[] Life;

        public bool IsValid()
        {
            return Count.Length + Speed.Length + CurrencyOnDeath.Length + Life.Length == Type.Length*4;
        }
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

        public string[] CurrentWaveMinionType
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
            return _waveInfos[CurrentWave%_waveInfos.Count].Count.Sum();
    }

        public int[] GetCurrentWaveLengths()
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

        public MinionData[] GetMinionDataForCurrentWave()
        {
            var currentWaveData = _waveInfos[CurrentWave];
            var waveMinionTypeCount = currentWaveData.Type.Length;
            var waveMinions = new MinionData[waveMinionTypeCount];

            var multiplierLife = System.Math.Pow(UpgradeManager.MinionLifeMultiplier, CurrentWave/_waveInfos.Count);
            var multiplierMoney = System.Math.Pow(UpgradeManager.MinionDeathRewardMultiplier, CurrentWave/_waveInfos.Count);
                
            for (int i = 0; i < waveMinionTypeCount; i++)
            {
                waveMinions[i] = new MinionData(currentWaveData.Life[i] * multiplierLife, currentWaveData.CurrencyOnDeath[i] * multiplierMoney, currentWaveData.Speed[i]);
            }

            return waveMinions;
        }

        public void ResetWave()
        {
            _maxWave = 0;
            _currentWave = 0;
        }
    }
}
