using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Manager;
using Assets.Scripts;
using UnityEngine.UI;

public enum AchievementType
{
    WaterMage,
    AirMage,
    FireMage,
    EarthMage,
    Wave,
    Spend,
    Earn,
    Reset
};

static class AchievementTypeExtensions
{

    public static BigIntWithUnit updateAchievementCount(this AchievementType type, BigIntWithUnit oldValue, BigIntWithUnit update)
    {
        switch (type)
        {
            case AchievementType.AirMage:
                return update;
            case AchievementType.FireMage:
                return update;
            case AchievementType.WaterMage:
                return update;
            case AchievementType.EarthMage:
                return update;
            case AchievementType.Wave:
                return update;
            case AchievementType.Spend:
                return oldValue + update;
            case AchievementType.Earn:
                return oldValue + update;
            case AchievementType.Reset:
                return oldValue + update;
        }
        return 0;
    }

}

public class AchievementManager : MonoBehaviour {
    private Dictionary<AchievementType, List<Achievement>> _achievements;
    private Dictionary<AchievementType, BigIntWithUnit> _achievementKeeper;

    //should change earn and spend to BigInt
	public GameObject AchievementPrefab;
	public Transform AchievementsWindow;

    private void Awake()
    {
        Init();
		foreach (AchievementType type in Enum.GetValues(typeof(AchievementType)))
		{
			foreach (var kvp in _achievements.Where(a => a.Key == type))
			{

				foreach (var ach in kvp.Value)
				{

						SpawnAchievement (ach);
				}
			}
		}

    }

    public void Init()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("GameInput - Achievement", typeof(TextAsset));
        var lines = textAsset.text.Replace("\r", "").Split('\n');

        _achievements = new Dictionary<AchievementType, List<Achievement>>();
        var fireMageAchievements = new List<Achievement>();
        var earthMageAchievements = new List<Achievement>();
        var airMageAchievements = new List<Achievement>();
        var waterMageAchievements = new List<Achievement>();
        var waveAchievements = new List<Achievement>();
        var resetAchievements = new List<Achievement>();
        var earnAchievements = new List<Achievement>();
        var spendAchievements = new List<Achievement>();

        for (var i = 1; i < lines.Length; i++)
        {
            

            var values = lines[i].Split(',');
            var threshold = int.Parse(values[2]);
			var title = values [3];
			var subtitle = values [4];

            if (values[1].Equals("FireMage"))
            {
                fireMageAchievements.Add(new Achievement(threshold, title, subtitle));

            }
            else if (values[1].Equals("EarthMage"))
            {
                earthMageAchievements.Add(new Achievement(threshold, title, subtitle));

            }
            else if (values[1].Equals("AirMage"))
            {
                airMageAchievements.Add(new Achievement(threshold, title, subtitle));

            }
            else if (values[1].Equals("WaterMage"))
            {
                waterMageAchievements.Add(new Achievement(threshold, title, subtitle));

            }
            else if (values[1].Equals("Wave"))
            {
                waveAchievements.Add(new Achievement(threshold, title, subtitle));

            }
            else if (values[1].Equals("Reset"))
            {
                resetAchievements.Add(new Achievement(threshold, title, subtitle));

            }
            else if (values[1].Equals("Earn"))
            {
                earnAchievements.Add(new Achievement(threshold, title, subtitle));

            }
            else if (values[1].Equals("Spend"))
            {
                spendAchievements.Add(new Achievement(threshold, title, subtitle));

            } 
        }
        _achievements.Add(AchievementType.FireMage, fireMageAchievements);
        _achievements.Add(AchievementType.EarthMage, earthMageAchievements);
        _achievements.Add(AchievementType.WaterMage, waterMageAchievements);
        _achievements.Add(AchievementType.AirMage, airMageAchievements);
        _achievements.Add(AchievementType.Earn, earnAchievements);
        _achievements.Add(AchievementType.Spend, spendAchievements);
        _achievements.Add(AchievementType.Wave, waveAchievements);
        _achievements.Add(AchievementType.Reset, resetAchievements);



    }

    public void RegisterEvent(AchievementType type, BigIntWithUnit count)
    {
        if (!_achievements.ContainsKey(type))
            return;
        BigIntWithUnit oldValue = 0;

        if (_achievementKeeper.ContainsKey(type))
        {
            oldValue = _achievementKeeper[type];
        }

        _achievementKeeper[type] = AchievementTypeExtensions.updateAchievementCount(type,oldValue,count);
    
        ParseAchievements(type);
    }

    public void ParseAchievements(AchievementType type)
    {
        foreach (var kvp in _achievements.Where(a => a.Key == type))
        {
            foreach (var ach in kvp.Value.Where(a => a.getIsUnlocked() == false))
            {
                if (_achievementKeeper[type] >= ach.getCountToUnlock())
                {
                    ach.setIsUnlocked(true);
					UnlockAchievement (ach);
                    Camera.main.GetComponent<UIManager>().showAchievementPopup(ach);
                }
            }
        }
    }

    public void SetAchievementKeeper(Dictionary<AchievementType, BigIntWithUnit> achievementKeeper)
    {
        _achievementKeeper = achievementKeeper;
        if (_achievementKeeper == null)
        {
            _achievementKeeper = new Dictionary<AchievementType, BigIntWithUnit>();
            return;
        }
        foreach (AchievementType type in Enum.GetValues(typeof(AchievementType)))
        {
            foreach (var kvp in _achievements.Where(a => a.Key == type))
            {

                foreach (var ach in kvp.Value.Where(a => a.getIsUnlocked() == false))
                {

                    if (_achievementKeeper.ContainsKey(type) && _achievementKeeper[type] >= ach.getCountToUnlock())
                    {
                        ach.setIsUnlocked(true);
                    }
                }
            }
        }
        
    }

    public Dictionary<AchievementType, BigIntWithUnit> GetAchievementKeeper()
    {
        return _achievementKeeper;
    }

	public void SpawnAchievement(Achievement _achievement){
		var newAchievement = Instantiate (AchievementPrefab);
		newAchievement.transform.SetParent (AchievementsWindow, false);
		_achievement.setObject (newAchievement);
		var texts = newAchievement.GetComponentsInChildren<Text> ();
        texts[0].text = _achievement.getTitle();
        texts[1].text = _achievement.getSubTitle();
	}

	public void UnlockAchievement(Achievement _achievement){
		_achievement.getObject().transform.FindChild ("Closed Logo").gameObject.SetActive (false);
	}
}
