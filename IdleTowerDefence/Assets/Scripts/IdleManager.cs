﻿using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class IdleManager : MonoBehaviour
    {
        //Idle Income Calculation
        BigIntWithUnit _maxPotentialWaveDmg;
        BigIntWithUnit MageDPS;
        private int roadLength = 290; //calculated as distances between each waypoint
        Player _player;
        Minion _minion;
        WaveManager _waveManager;
        double multiplierMoney;
        int MageAttackDuration; //Duration it takes a wave to travel the maze

        // Use this for initialization
        void Awake() {
            CalculateIdleIncome();
        }

        // Update is called once per frame
        void Update() {

        }

        public void OnApplicationQuit() {
            PlayerPrefs.SetString("_gameCloseTime", System.DateTime.Now.ToString());
        }

        public void CalculateIdleIncome() {
           
            var gameCloseTime = PlayerPrefs.GetString("_gameCloseTime");
            var gameClosedAt = DateTime.Parse(gameCloseTime);
            var now = DateTime.Now;
            var idleTime = now - gameClosedAt;
            var idleTimeInSeconds = idleTime.TotalSeconds;
            MageAttackDuration = roadLength / (int)_minion.Data.GetSpeed();

            //Calculate Total Idle Damage
            MageDPS = _player.Data.CumulativeDps();
            _maxPotentialWaveDmg = MageDPS * (roadLength/(int)_minion.Data.GetSpeed());

            //Establish Idle Currency Formula
            multiplierMoney = System.Math.Pow(1.03, _waveManager.Data.CurrentWave); // %30 money multiplier
            var currencyGained = BigIntWithUnit.MultiplyPercent(WaveData.BaseCurrencyGivenOnDeath, multiplierMoney);
            currencyGained = BigIntWithUnit.MultiplyPercent(currencyGained, _waveManager.Data.GetCurrentWaveLength());

            //Idle Currency Gaining
            for (int i = 0; i < 5; i++)
            {
                if (idleTimeInSeconds < MageAttackDuration) //not enough time left to kill the wave
                {
                    break;
                }
                var _ratioKilled = _maxPotentialWaveDmg / _waveManager.WaveLife;
                if (_ratioKilled >= 1)
                {
                    _ratioKilled = 1;
                    if (!_waveManager.Data.IsNextWaveBossWave)
                    {
                        _waveManager.Data.IncreaseCurrentWaveIfLessThanMax();
                    }  
                }
                currencyGained = BigIntWithUnit.MultiplyPercent(currencyGained, _ratioKilled);
                _player.Data.IncreaseCurrency(currencyGained);
                idleTimeInSeconds -= MageAttackDuration;
                if (_ratioKilled == 1) //calculate currency gain of next wave if it comes
                {
                    currencyGained = BigIntWithUnit.MultiplyPercent(WaveData.BaseCurrencyGivenOnDeath, multiplierMoney);
                    currencyGained = BigIntWithUnit.MultiplyPercent(currencyGained, _waveManager.Data.GetCurrentWaveLength());
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

            while (idleTimeInSeconds > MageAttackDuration)
            {
                _player.Data.IncreaseCurrency(currencyGained);
                idleTimeInSeconds -= MageAttackDuration;
            }
        }
    }
}
