using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace Assets.Scripts.Manager
{
    public class AnalyticsManager {

        public static void SendStartMageChosen(Element element)
        {
            CustomEventWithElement("Game Started", element);
        }

        public static void MageSkillUsed(Element element)
        {
            CustomEventWithElement("Mage Skill Used", element);
        }

        private static void CustomEventWithElement(string eventName, Element element)
        {
            Analytics.CustomEvent(eventName, new Dictionary<string, object>
            {
                {"Device Id", SystemInfo.deviceUniqueIdentifier},
                {"Element", element.ToString()}
            });
        }

        public static void GameReset(Player player, Element bonusElement)
        {
            Analytics.CustomEvent("Game Reset", new Dictionary<string, object>
            {
                {"Device Id", SystemInfo.deviceUniqueIdentifier},
                {"Reset Level", player.WaveManager.Data.CurrentWave},
                {"Reset Element", bonusElement.ToString()},
                {"Reset Number", player.Data.GetResetAmount() + 1}
            });
        }

        public static void LevelPassed(WaveManager waveManager)
        {
            Analytics.CustomEvent("Level Passed", new Dictionary<string, object>
            {
                {"Device Id", SystemInfo.deviceUniqueIdentifier},
                {"Current Level", waveManager.Data.GetMaxReachedWave()}
            });
        }

        public static void MageUpgraded(Mage mage)
        {
            Analytics.CustomEvent("Mage Upgraded", new Dictionary<string, object>
            {
                {"Device Id", SystemInfo.deviceUniqueIdentifier},
                {"Mage Type", mage.Data.GetElement().ToString()},
                {"Mage Level", mage.Data.GetLevel()}
            });
        }

        

    }
}
