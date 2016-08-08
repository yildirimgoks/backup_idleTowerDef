using UnityEngine;
using System;

namespace Assets.Scripts
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

        public BigIntWithUnit CalculateIdleIncome()
        {
            var gameCloseTime = PlayerPrefs.GetString("_gameCloseTime");
            var gameClosedAt = DateTime.Parse(gameCloseTime);
            var now = DateTime.Now;
            var idleTime = now - gameClosedAt;
            var idleTimeInSeconds = idleTime.TotalSeconds;
			Debug.Log("roadlen: " + _roadLength + " and minion speed: " + (int)_waveManager.WaveSpeed);
			var mageAttackDuration = _roadLength / (int)_waveManager.WaveSpeed;
            BigIntWithUnit totalIncome = 0;

            //Calculate Total Idle Damage
            _mageDps = _player.Data.CumulativeDps();
            _maxPotentialWaveDmg = _mageDps * mageAttackDuration;

            //Establish Idle Currency Formula
            var multiplierMoney = Math.Pow(1.03, _waveManager.Data.CurrentWave); // %30 money multiplier
            var currencyGained = BigIntWithUnit.MultiplyPercent(WaveData.BaseCurrencyGivenOnDeath, multiplierMoney*100);
            currencyGained = BigIntWithUnit.MultiplyPercent(currencyGained, _waveManager.Data.GetCurrentWaveLength()*100);
            
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
                    _waveManager.Data.IncreaseCurrentWaveAndMaxWave();
                }
                currencyGained = BigIntWithUnit.MultiplyPercent(currencyGained, _maxPotentialWaveDmg / _waveManager.WaveLife*100);
                totalIncome += currencyGained;
                idleTimeInSeconds -= mageAttackDuration;
                if (waveKilled) //calculate currency gain of next wave if it comes
                {
                    currencyGained = BigIntWithUnit.MultiplyPercent(WaveData.BaseCurrencyGivenOnDeath, multiplierMoney*100);
                    currencyGained = BigIntWithUnit.MultiplyPercent(currencyGained, _waveManager.Data.GetCurrentWaveLength()*100);
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
            }
            return totalIncome;
        }
    }
}
