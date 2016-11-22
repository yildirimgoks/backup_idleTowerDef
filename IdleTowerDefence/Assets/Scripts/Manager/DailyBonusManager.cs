using UnityEngine;
using System.Collections;
using System;

namespace Assets.Scripts.Manager
{
    public class DailyBonusManager : MonoBehaviour
    {
        public DailyBonusWindow DailyBonusWindow;

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

        public BigIntWithUnit CalculateReward()
        {
            BigIntWithUnit reward = GetConsecutiveDays() * 10000;
            Debug.Log(reward + " coins given.");
            UpdateLastPlayDate();
            UpdateConsecutiveDays();
            return reward;
        }

        public BigIntWithUnit GetReward()
        {
            //double hours = GetHours();
            Debug.Log(GetTimeSpan());
            double seconds = GetTimeSpan().TotalSeconds;
            BigIntWithUnit reward = 0;
            if (seconds >= 24)
            {
                if (seconds > 48)
                {
                    UpdateLastPlayDate();
                    ResetConsecutiveDays();
                }
                DailyBonusWindow.OpenBonusMenu();
                DailyBonusWindow.LockAllDays();
                for (int i = 1; i <= GetConsecutiveDays(); i++)
                {
                    DailyBonusWindow.UnlockDay(i);
                }
                reward = CalculateReward();             
            }
            return reward;
        }
    }
}