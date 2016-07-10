using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class IdleManager : MonoBehaviour
    {
        //Idle Income Calculation
        BigIntWithUnit _maxPotentialWaveDmg;
        BigIntWithUnit MageDPS;
        int MageAttackDuration = 35; //Duration it takes a wave to travel the maze
        Player _player;
        Minion _minion;
        WaveManager _waveManager;
        double multiplierMoney; 

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

            //Calculate Total Idle Damage
            MageDPS = _player.Data.CumulativeDps();
            _maxPotentialWaveDmg = MageAttackDuration * MageDPS;

            //Establish Idle Currency Formula
            multiplierMoney = System.Math.Pow(1.03, _waveManager.Data.CurrentWave);
            var currencyGained = BigIntWithUnit.MultiplyPercent(WaveData.BaseCurrencyGivenOnDeath, multiplierMoney);
            currencyGained = BigIntWithUnit.MultiplyPercent(currencyGained, _waveManager.Data.GetCurrentWaveLength());

            //Idle Currency Gaining
            while (idleTimeInSeconds % MageAttackDuration > MageAttackDuration) {
                double _ratioKilled = _maxPotentialWaveDmg / _waveManager.WaveLife;
                if (_ratioKilled >=1){
                    _ratioKilled = 1;
                    if (!_waveManager.Data.IsNextWaveBossWave) {
                        _waveManager.Data.IncreaseCurrentWaveIfLessThanMax();
                    }  
                }
                currencyGained = BigIntWithUnit.MultiplyPercent(currencyGained, _ratioKilled);
                _player.Data.IncreaseCurrency(currencyGained);
                _waveManager.Data.IncreaseCurrentWaveIfLessThanMax();
                idleTimeInSeconds -= MageAttackDuration;
            }

            if (_waveManager.Data.IsNextWaveBossWave) {
                Debug.Log("A Boss is attacking your castle!"); //send notification
            }
        }
    }
}
