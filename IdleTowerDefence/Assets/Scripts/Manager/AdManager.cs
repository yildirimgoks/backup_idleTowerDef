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
		public float BonusTime = 3600f;
        public Player Player;

        private bool _finished;

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
            if (_finished)
            {
                Timer.Cooldown(BonusTime, Time.time); //burda mı olmalı?
                _finished = false;
            }
            else
            {
                Timer.Cooldown(5 , Time.time); //no cooldown if not finished
            }
            
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
                    _finished = true;
                    var curmodifier = 0.5f;
                    var dmgmodifier = 0.2f;
                    Player.SetIncomeModifier(curmodifier, BonusTime);
                    Player.SetDamageModifier(dmgmodifier, BonusTime);
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