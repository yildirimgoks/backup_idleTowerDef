using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Manager;

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

    public static int updateAchievementCount(this AchievementType type,int oldValue, int update)
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
    private Dictionary<AchievementType, int> _achievementKeeper;
    public int[] _thresholds;

    //should change earn and spend to BigInt

    private void Awake()
    {
        Init();


    }

    public void Init()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("GameInput - Achievement", typeof(TextAsset));
        var lines = textAsset.text.Replace("\r", "").Split('\n');

        _achievements = new Dictionary<AchievementType, List<Achievement>>();

        for (var i = 1; i < lines.Length; i++)
        {
            var achievements = new List<Achievement>();

            var values = lines[i].Split(',');
            _thresholds = values[2].Split(';').Select(elem => int.Parse(elem)).ToArray();

            foreach(var threshold in _thresholds)
            {
                achievements.Add(new Achievement(threshold));
            }

            if (values[1].Equals("FireMage"))
            {
                _achievements.Add(AchievementType.FireMage, achievements);
            }
            else if (values[1].Equals("EarthMage"))
            {
                _achievements.Add(AchievementType.EarthMage, achievements);
            }
            else if (values[1].Equals("AirMage"))
            {
                _achievements.Add(AchievementType.AirMage, achievements);
            }
            else if (values[1].Equals("WaterMage"))
            {
                _achievements.Add(AchievementType.WaterMage, achievements);
            }
            else if (values[1].Equals("Wave"))
            {
                _achievements.Add(AchievementType.Wave, achievements);
            }
            else if (values[1].Equals("Reset"))
            {
                _achievements.Add(AchievementType.Reset, achievements);
            }
            else if (values[1].Equals("Earn"))
            {
                _achievements.Add(AchievementType.Earn, achievements);
            }
            else if (values[1].Equals("Spend"))
            {
                _achievements.Add(AchievementType.Spend, achievements);
            } 
        }
    }

    public void RegisterEvent(AchievementType type,int count)
    {
        if (!_achievements.ContainsKey(type))
            return;
        int oldValue = 0;

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
                    Camera.main.GetComponent<UIManager>().showAchievementPopup(ach);
                }
            }
        }
    }

    public void SetAchievementKeeper(Dictionary<AchievementType, int> achievementKeeper)
    {
        _achievementKeeper = achievementKeeper;
        if (_achievementKeeper == null)
        {
            _achievementKeeper = new Dictionary<AchievementType, int>();
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

    public Dictionary<AchievementType, int> GetAchievementKeeper()
    {
        return _achievementKeeper;
    }
}
