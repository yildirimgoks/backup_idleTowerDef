using UnityEngine;
using System.Collections;
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
           
            string _gameCloseTime = PlayerPrefs.GetString("_gameCloseTime");
            DateTime _gameClosedAt = DateTime.Parse(_gameCloseTime);
            DateTime _now = DateTime.Now;
            TimeSpan _idleTime = _now - _gameClosedAt;
            double _idleTimeInSeconds = _idleTime.TotalSeconds;

            //Calculate Total Idle Damage
            MageDPS = _player.CumulativeDps();
            _maxPotentialWaveDmg = MageAttackDuration * MageDPS;

            //Establish Idle Currency Formula
            multiplierMoney = System.Math.Pow(1.03, _waveManager.CurrentWave);
            BigIntWithUnit _currencyGained = BigIntWithUnit.MultiplyPercent(Minion.BaseCurrencyGivenOnDeath, multiplierMoney);
            _currencyGained = BigIntWithUnit.MultiplyPercent(_currencyGained, _waveManager._waveLength);

            //Idle Currency Gaining
            while (_idleTimeInSeconds % MageAttackDuration > MageAttackDuration) {
                double _ratioKilled = _maxPotentialWaveDmg / _waveManager.WaveLife;
                if (_ratioKilled >=1){
                    _ratioKilled = 1;
                    if (!_waveManager.IsNextWaveBossWave) {
                        _waveManager._maxWave++;
                    }  
                }
                _currencyGained = BigIntWithUnit.MultiplyPercent(_currencyGained, _ratioKilled);
                _player.IncreaseCurrency(_currencyGained);
                _waveManager.CurrentWave = _waveManager._maxWave;
                _idleTimeInSeconds -= MageAttackDuration;
            }

            if (_waveManager.IsNextWaveBossWave) {
                Debug.Log("A Boss is attacking your castle!"); //send notification
            }
        }
    }
}
