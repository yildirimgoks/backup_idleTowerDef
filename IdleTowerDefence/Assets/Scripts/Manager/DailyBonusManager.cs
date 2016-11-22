using UnityEngine;
using System.Collections;
using System;

namespace Assets.Scripts.Manager
{
    public class DailyBonusManager : MonoBehaviour
    {
        public DailyBonusWindow DailyBonusWindow;
		private Player Player;

		public string rewardText;
		public BigIntWithUnit reward;

		void Start(){
			Player = Camera.main.GetComponent<Player>();
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
            if (cDays == 0 || cDays == 8)
            {
                ResetConsecutiveDays();
                cDays = 1;
            }
            return cDays;
        }

        public void ResetConsecutiveDays()
        {
            PlayerPrefs.SetInt("ConsecutiveDays", 1);
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
            reward = GetConsecutiveDays() * 10000;
			rewardText = "You have gained " + reward.ToString () + " Golds! Click here to claim your prize.";
        }

		public void DoRewarding(){
			Player.Data.IncreaseCurrency (reward);
			Debug.Log(reward + " coins given.");
		}

        public void InitiateRewardPage()
        {
            //double hours = GetHours();
            Debug.Log(GetTimeSpan());
            double seconds = GetTimeSpan().TotalSeconds;
            if (seconds >= 24)
            {
                if (seconds > 48)
                {
                    UpdateLastPlayDate();
                    ResetConsecutiveDays();
					DailyBonusWindow.LockAllDays();
                }
				UpdateLastPlayDate();
				UpdateConsecutiveDays();
				CalculateReward ();
				Debug.Log (GetConsecutiveDays());
                DailyBonusWindow.OpenBonusMenu();
				DailyBonusWindow.UnlockUntilDay (GetConsecutiveDays ());
				DailyBonusWindow.SetScrollToDay (GetConsecutiveDays ());
				DailyBonusWindow.SetDaysUntilDay (GetConsecutiveDays (), delegate {
					DoRewarding();
				},rewardText );
            }
        }
    }
}