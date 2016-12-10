using System;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class IdleManager
    {
        //Idle Income Calculation
        private readonly int _roadLength = 280; //calculated as distances between each waypoint
        private readonly Player _player;
        private readonly WaveManager _waveManager;

        public IdleManager(Player player, WaveManager waveManager)
        {
            _player = player;
            _waveManager = waveManager;
        }

        public BigIntWithUnit CalculateIdleIncome(out int killedCreatures, out int passedLevels)
        {
            var gameCloseTime = PlayerPrefs.GetString("_gameCloseTime");
            var idleTime = DateTime.Now - DateTime.Parse(gameCloseTime);
            var idleTimeInSeconds = idleTime.TotalSeconds;
            killedCreatures = 0;
            passedLevels = 0;
            if (idleTimeInSeconds > 7200)
            {
                //2hrs max
                idleTimeInSeconds = 7200;
            }
            else if (idleTimeInSeconds < 60)
            {
                return 0;
            }

            while (_waveManager.Data.IsBossWave)
            {
                _waveManager.Data.DecreaseCurrentWave();
            }
            _waveManager.ClearCurrentWave();
            _waveManager.CreateCurrentWave();
            var mageAttackDuration = _roadLength / _waveManager.WaveSpeed;
            BigIntWithUnit totalIncome = 0;
            //Calculate Total Idle Damage, use avarage attack of 6 towers
            var maxPotentialWaveDmg = _player.Data.CumulativeDps() * mageAttackDuration / 6;

            //Calculate the idle mage currency
            totalIncome += _player.Data.CumulativeIdleEarning() * idleTimeInSeconds;

            //Idle Currency Gaining
            while (idleTimeInSeconds > 0)
            {
                var timeMultiplier = 1.0;
                if (idleTimeInSeconds < mageAttackDuration) //not enough time left to kill the wave
                {
                    timeMultiplier = idleTimeInSeconds/mageAttackDuration;
                }
                var waveKilledPercent = maxPotentialWaveDmg / _waveManager.WaveLife;
                totalIncome += _waveManager.WaveReward * waveKilledPercent * timeMultiplier;
                killedCreatures += (int)(_waveManager.Data.GetCurrentWaveLength() * waveKilledPercent * timeMultiplier);
                idleTimeInSeconds -= mageAttackDuration;

                if (waveKilledPercent >= 1 &&  timeMultiplier >= 0.99 && !_waveManager.Data.IsNextWaveBossWave) //calculate currency gain of next wave if it comes
                {
                    passedLevels++;
                    _waveManager.Data.IncreaseCurrentWaveAndMaxWave();
                    _waveManager.ClearCurrentWave();
                    _waveManager.CreateCurrentWave();
                }
            }
            _waveManager.ClearCurrentWave();
            return totalIncome;
        }
    }
}
