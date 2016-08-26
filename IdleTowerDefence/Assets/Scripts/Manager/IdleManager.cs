using System;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class IdleManager
    {
        //Idle Income Calculation
        private readonly int _roadLength = 290; //calculated as distances between each waypoint
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
			var mageAttackDuration = _roadLength / (int)_waveManager.WaveSpeed;
            BigIntWithUnit totalIncome = 0;
            killedCreatures = 0;
            passedLevels = 0;
            //Calculate Total Idle Damage
            var maxPotentialWaveDmg = _player.Data.CumulativeDps() * mageAttackDuration;

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
                totalIncome += GetCurrencyGainedForWave() * waveKilledPercent * timeMultiplier;
                killedCreatures += (int)(_waveManager.Data.GetCurrentWaveLength() * waveKilledPercent * timeMultiplier);
                idleTimeInSeconds -= mageAttackDuration;

                if (waveKilledPercent >= 1 && !_waveManager.Data.IsNextWaveBossWave) //calculate currency gain of next wave if it comes
                {
                    passedLevels++;
                    _waveManager.Data.IncreaseCurrentWaveAndMaxWave();
                    _waveManager.SendWave();
                }
            }
            return totalIncome;
        }

        private BigIntWithUnit GetCurrencyGainedForWave()
        {
            //Establish Idle Currency Formula
            var multiplierMoney = Math.Pow(UpgradeManager.MinionDeathRewardMultiplier, _waveManager.Data.CurrentWave);
            var singleCreepReward = UpgradeManager.MinionDeathRewardInitial * multiplierMoney;
            return singleCreepReward * _waveManager.Data.GetCurrentWaveLength();
        }
    }
}
