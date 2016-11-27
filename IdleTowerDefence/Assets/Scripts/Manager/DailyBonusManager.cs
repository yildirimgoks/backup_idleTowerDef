﻿using UnityEngine;
using System.Collections;
using System;

namespace Assets.Scripts.Manager
{
    public class DailyBonusManager : MonoBehaviour
    {
        public DailyBonusWindow DailyBonusWindow;
		private Player _player;

		private string _rewardText;
		private BigIntWithUnit _reward;

		void Start(){
			_player = Camera.main.GetComponent<Player>();
		}

        public DateTime GetLastPlayDate()
        {
            string lastDate = PlayerPrefs.GetString("LastPlayDate");
            if (lastDate == "")
            {
                UpdateLastPlayDate();
                lastDate = Convert.ToString(DateTime.Now);
            }
            return Convert.ToDateTime(lastDate);
        }

        public int GetConsecutiveDays()
        {
            int cDays = PlayerPrefs.GetInt("ConsecutiveDays");
            if (cDays > 7)
            {
                PlayerPrefs.SetInt("ConsecutiveDays", 1);
                cDays = 1;
            }
            return cDays;
        }

        public void ResetConsecutiveDays()
        {
            PlayerPrefs.SetInt("ConsecutiveDays", 0);
        }

        public void UpdateConsecutiveDays()
        {
            PlayerPrefs.SetInt("ConsecutiveDays", GetConsecutiveDays() + 1);
        }

        public void UpdateLastPlayDate()
        {
            string lastDate = Convert.ToString(DateTime.Now);
            PlayerPrefs.SetString("LastPlayDate", lastDate);
        }

        public void InitializePrefs()
        {
            ResetConsecutiveDays();
            UpdateLastPlayDate();
        }

        public TimeSpan GetTimeSpan()
        {
            return DateTime.Now - GetLastPlayDate();
        }

        public double GetHours()
        {
            return GetTimeSpan().TotalHours;
        }

        public void CalculateReward()
        {
            _reward = GetConsecutiveDays() * 10000;
			_rewardText = "You have gained " + _reward.ToString () + " Golds! Click here to claim your prize.";
        }

		public void DoRewarding(){
			_player.Data.IncreaseCurrency (_reward);
			Debug.Log(_reward + " coins given.");
		}

        public void InitiateRewardPage()
        {
            double hours = GetHours();
            if (hours >= 24)
            {
                if (hours > 48)
                {
                    UpdateLastPlayDate();
                    ResetConsecutiveDays();
					DailyBonusWindow.LockAllDays();
                }
				UpdateLastPlayDate();
				UpdateConsecutiveDays();
				CalculateReward ();
                DailyBonusWindow.OpenBonusMenu();
				DailyBonusWindow.UnlockUntilDay (GetConsecutiveDays ());
				DailyBonusWindow.SetScrollToDay (GetConsecutiveDays ());
				DailyBonusWindow.SetDaysUntilDay (GetConsecutiveDays (), delegate {
					DoRewarding();
				},_rewardText );
            }
        }
    }
}