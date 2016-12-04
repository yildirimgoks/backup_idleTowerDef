using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

namespace Assets.Scripts
{
    public class AdManager : MonoBehaviour
    {
		public Text AdText;
		public CoolDown Timer;
		public float WaitTime;
        public Player Player;

		void Update()
		{
			Timer.GetComponent<Button> ().interactable = !Timer.IsCoolDown;
		}

        public void ShowRewardedAd()
        {
            if (Advertisement.IsReady("rewardedVideo"))
            {
                var options = new ShowOptions { resultCallback = HandleShowResult };
                Advertisement.Show("rewardedVideo", options);  
            }
			Timer.Cooldown (WaitTime, Time.time); //burda mı olmalı?
        }

        private void HandleShowResult(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    Debug.Log("The ad was successfully shown.");
                    //
                    // YOUR CODE TO REWARD THE GAMER
                    // Give coins etc.
                    Player = Camera.main.GetComponent<Player>();
                    var time = 3600;
                    var curmodifier = 0.5f;
                    var dmgmodifier = 0.2f;
                    Player.SetIncomeModifier(curmodifier, time);
                    Player.SetDamageModifier(dmgmodifier, time);
                    break;
                case ShowResult.Skipped:
                    Debug.Log("The ad was skipped before reaching the end.");
                    break;
                case ShowResult.Failed:
                    Debug.LogError("The ad failed to be shown.");
                    break;
            }
        }

		public void PrintReward(string rewardText){
			AdText.text = rewardText;
		}
    }
}