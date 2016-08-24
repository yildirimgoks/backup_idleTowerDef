﻿using System;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class IdleManager
    {
        //Idle Income Calculation
        private BigIntWithUnit _maxPotentialWaveDmg;
        private BigIntWithUnit _mageDps;
        private readonly int _roadLength = 290; //calculated as distances between each waypoint
        private readonly Player _player;
        private readonly WaveManager _waveManager;
        private readonly Mage _mage;

        public IdleManager(Player player, WaveManager waveManager)
        {
            _player = player;
            _waveManager = waveManager;
        }

        public BigIntWithUnit CalculateIdleIncome(out int killedCreatures, out int passedLevels)
        {
            var gameCloseTime = PlayerPrefs.GetString("_gameCloseTime");
            var gameClosedAt = DateTime.Parse(gameCloseTime);
            var now = DateTime.Now;
            var idleTime = now - gameClosedAt;
            var idleTimeInSeconds = idleTime.TotalSeconds;
			var mageAttackDuration = _roadLength / (int)_waveManager.WaveSpeed;
            BigIntWithUnit totalIncome = 0;
            killedCreatures = 0;
            passedLevels = 0;
            //Calculate Total Idle Damage
            _mageDps = _player.Data.CumulativeDps();
            _maxPotentialWaveDmg = _mageDps * mageAttackDuration;

            //Establish Idle Currency Formula
            var multiplierMoney = Math.Pow(1.03, _waveManager.Data.CurrentWave); // %30 money multiplier
            var currencyGained = UpgradeManager.MinionDeathRewardInitial * multiplierMoney;
            currencyGained = currencyGained * _waveManager.Data.GetCurrentWaveLength();
            
            //find the idle mage count and give currency for them beforehand
            foreach (var mage in _player.Data.GetMages())
            {
                if (mage.Data.IsIdle())
                {
                    totalIncome += mage.Data.GetIdleCurrency()*(int)idleTimeInSeconds;
                }
            }

            //Idle Currency Gaining
            for (int i = 0; i < 5; i++)
            {
                if (idleTimeInSeconds < mageAttackDuration) //not enough time left to kill the wave
                {
                    break;
                }
                var waveKilled = _maxPotentialWaveDmg / _waveManager.WaveLife >= 1;
                if (waveKilled && !_waveManager.Data.IsNextWaveBossWave)
                {
                    passedLevels ++;
                    _waveManager.Data.IncreaseCurrentWaveAndMaxWave();
                    _waveManager.SendWave();
                }
                currencyGained = BigIntWithUnit.MultiplyPercent(currencyGained, _maxPotentialWaveDmg / _waveManager.WaveLife*100);
                totalIncome += currencyGained;
                idleTimeInSeconds -= mageAttackDuration;
                if (waveKilled) //calculate currency gain of next wave if it comes
                {
                    currencyGained = UpgradeManager.MinionDeathRewardInitial * multiplierMoney;
                    currencyGained = currencyGained * _waveManager.Data.GetCurrentWaveLength();
                    killedCreatures += _waveManager.Data.GetCurrentWaveLength();
                }
                else //next wave can't come 
                {
                    break;
                }
                if (_waveManager.Data.IsNextWaveBossWave) //next wave is boss, give money of the last wave continuously until time runs out
                {
                    Debug.Log("A Boss is attacking your castle!");
                    break; //send notification
                }
            }

            while (idleTimeInSeconds > mageAttackDuration)
            {
                //Calculate end currency amount and return
                totalIncome += currencyGained;
                idleTimeInSeconds -= mageAttackDuration;
                killedCreatures += _waveManager.Data.GetCurrentWaveLength();
            }
            return totalIncome;
        }
    }
}
