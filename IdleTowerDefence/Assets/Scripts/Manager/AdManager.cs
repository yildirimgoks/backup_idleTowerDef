using Assets.Scripts.Manager;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

namespace Assets.Scripts
{
    public class AdManager : MonoBehaviour
    {
		public Text AdText;
		public CoolDown Timer;
		public int BonusTime = 3600;
		public int WaitTime=5;
        public Player Player;
        public UIManager UIManager;

        private bool _finished;
#if UNITY_ADS
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
            /*if (_finished)
            {
                Timer.Cooldown(BonusTime, Time.time); //burda mı olmalı?
                _finished = false;
            }
            else
            {
                Timer.Cooldown(5, Time.time); //no cooldown if not finished
            }
		*/
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
                UIManager = Camera.main.GetComponent<UIManager>();
                var bonus = Player.CurrentBonus;
                switch (bonus)
                {
                    case Player.AdSelector.Damage:
                        var dmgmodifier = 1.2f;
                        Player.SetModifier(Player.AdSelector.Damage, dmgmodifier, BonusTime);
                        Player.CurrentBonus = Player.AdSelector.Currency;
                        UIManager.CreateNotificications("Congratulations!", "You have gained " + (int)(dmgmodifier * 100) + " percent damage bonus for " + BonusTime / 60 + " minutes!");
                        break;
                    case Player.AdSelector.Currency:
                        var curmodifier = 1.5f;
                        Player.SetModifier(Player.AdSelector.Currency, curmodifier, BonusTime);
                        Player.CurrentBonus = Player.AdSelector.Damage;
                        UIManager.CreateNotificications("Congratulations!", "You have gained " + (int)(curmodifier * 100) + " percent income bonus for " + BonusTime / 60 + " minutes!");
                        break;
                }
				Timer.Cooldown(BonusTime, Time.time); //burda mı olmalı?
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
				Timer.Cooldown(WaitTime, Time.time); //no cooldown if not finished
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
				Timer.Cooldown(WaitTime, Time.time); //no cooldown if not finished
                break;
        }
    }

    public void PrintReward(string rewardText){
			AdText.text = rewardText;
		}
#endif

    }
}