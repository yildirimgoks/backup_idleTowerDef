using UnityEngine;
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

        public WaveManager WaveManager;
        public UIManager UIManager;
        public AudioManager AudioManager;

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
            int consecutiveDays = GetConsecutiveDays();
            if (consecutiveDays != 2 && consecutiveDays != 5)
            {
                float multiplier = (float)(consecutiveDays) / 2 + 1;
                _reward = WaveManager.Data.GetTotalLoot() * multiplier;
                _rewardText = "You have gained " + _reward.ToString() + " Golds! Click here to claim your prize.";
            }
            else
            {
                _rewardText = "You have gained damage and income bonus! Click here to claim your prize.";
            }
        }

        public void DoRewarding()
        {
            int consecutiveDays = GetConsecutiveDays();
            if (consecutiveDays != 2 && consecutiveDays != 5)
            {
                _player.Data.IncreaseCurrency(_reward);
                UIManager.CreateFloatingText(_reward.ToString(), null, new Vector3(28, 75, 15), "c");
                AudioManager.PlayCoinSound();
                Debug.Log(_reward + " coins given.");
            }
            else
            {
                var time = 3600;
                var dmgmodifier = (float)(consecutiveDays - 1) * 0.2f;
                _player.SetModifier(Player.AdSelector.Damage, dmgmodifier, time);
            }
        }

        public void InitiateRewardPage()
        {

            if (PlayerPrefs.GetInt("TutorialShown1") == 1 && PlayerPrefs.GetInt("TutorialShown2") == 1)
            {
                double hours = GetHours();
                if (hours >= 24 || GetConsecutiveDays() == 0)
                {
                    if (hours > 48)
                    {
                        UpdateLastPlayDate();
                        ResetConsecutiveDays();
                        DailyBonusWindow.LockAllDays();
                    }
                    UpdateLastPlayDate();
                    UpdateConsecutiveDays();
                    CalculateReward();
                    DailyBonusWindow.OpenBonusMenu();
                    DailyBonusWindow.UnlockUntilDay(GetConsecutiveDays());
                    DailyBonusWindow.SetScrollToDay(GetConsecutiveDays());
                    DailyBonusWindow.SetDaysUntilDay(GetConsecutiveDays(), delegate {
                        DoRewarding();
                    }, _rewardText);
                }
            }         
        }
    }
}