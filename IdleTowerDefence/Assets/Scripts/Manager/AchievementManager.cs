using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

public class AchievementManager : MonoBehaviour {
    private Dictionary<AchievementType, List<Achievement>> _achievements;
    private Dictionary<AchievementType, int> _achievementKeeper;

    public event EventHandler AchievementUnlocked;
    protected virtual void RaiseAchievementUnlocked(Achievement ach)
    {
        // unlock the event
        ach.setIsUnlocked(true);

        var del = AchievementUnlocked as EventHandler;
        if (del != null)
        {
            del(this, new AchievementEventArg(ach));
        }
    }

    public AchievementManager()
    {
        _achievementKeeper = new Dictionary<AchievementType, int>();
        // ideally, load here previous, saved values.
        // tap = 0
        // die = 1
        // start = 12
        // score = 1231

        //add achivements here
        _achievements = new Dictionary<AchievementType, List<Achievement>>();

        var earthMageAchievements = new List<Achievement>();
        earthMageAchievements.Add(new Achievement(10));
        earthMageAchievements.Add(new Achievement(20));
        earthMageAchievements.Add(new Achievement(30));
        earthMageAchievements.Add(new Achievement(40));
        earthMageAchievements.Add(new Achievement(50));
        
        _achievements.Add(AchievementType.EarthMage, earthMageAchievements);

        var fireMageAchievements = new List<Achievement>();
        fireMageAchievements.Add(new Achievement(10));
        fireMageAchievements.Add(new Achievement(20));
        fireMageAchievements.Add(new Achievement(30));
        fireMageAchievements.Add(new Achievement(40));
        fireMageAchievements.Add(new Achievement(50));

        _achievements.Add(AchievementType.FireMage, fireMageAchievements);

        var airMageAchievements = new List<Achievement>();
        airMageAchievements.Add(new Achievement(10));
        airMageAchievements.Add(new Achievement(20));
        airMageAchievements.Add(new Achievement(30));
        airMageAchievements.Add(new Achievement(40));
        airMageAchievements.Add(new Achievement(50));

        _achievements.Add(AchievementType.AirMage, airMageAchievements);

        var waterMageAchievements = new List<Achievement>();
        waterMageAchievements.Add(new Achievement(10));
        waterMageAchievements.Add(new Achievement(20));
        waterMageAchievements.Add(new Achievement(30));
        waterMageAchievements.Add(new Achievement(40));
        waterMageAchievements.Add(new Achievement(50));

        _achievements.Add(AchievementType.WaterMage, waterMageAchievements);

        var waveAchievements = new List<Achievement>();
        waveAchievements.Add(new Achievement(50));
        waveAchievements.Add(new Achievement(100));
        waveAchievements.Add(new Achievement(150));
        waveAchievements.Add(new Achievement(200));
        waveAchievements.Add(new Achievement(250));
        waveAchievements.Add(new Achievement(300));

        _achievements.Add(AchievementType.Wave, waveAchievements);

        var resetAchievements = new List<Achievement>();
        resetAchievements.Add(new Achievement(1));

        _achievements.Add(AchievementType.Reset, resetAchievements);

        var earnAchievements = new List<Achievement>();
        earnAchievements.Add(new Achievement(100000));
        earnAchievements.Add(new Achievement(500000));

        _achievements.Add(AchievementType.Earn, earnAchievements);

        var spendAchievements = new List<Achievement>();
        spendAchievements.Add(new Achievement(100000));
        spendAchievements.Add(new Achievement(500000));

        _achievements.Add(AchievementType.Spend, spendAchievements);

    }

    public void RegisterEvent(AchievementType type,bool add,int count)
    {
        if (!_achievementKeeper.ContainsKey(type))
            return;

        switch (type)
        {
            case AchievementType.WaterMage:
                if (add)
                {
                    _achievementKeeper[type] += count;
                } else
                {
                    _achievementKeeper[type] = count;
                }
                break;
            case AchievementType.FireMage:
                if (add)
                {
                    _achievementKeeper[type] += count;
                }
                else
                {
                    _achievementKeeper[type] = count;
                }
                break;
            case AchievementType.AirMage:
                if (add)
                {
                    _achievementKeeper[type] += count;
                }
                else
                {
                    _achievementKeeper[type] = count;
                }
                break;
            case AchievementType.EarthMage:
                if (add)
                {
                    _achievementKeeper[type] += count;
                }
                else
                {
                    _achievementKeeper[type] = count;
                }
                break;
            case AchievementType.Earn:
                if (add)
                {
                    _achievementKeeper[type] += count;
                }
                else
                {
                    _achievementKeeper[type] = count;
                }
                break;
            case AchievementType.Spend:
                if (add)
                {
                    _achievementKeeper[type] += count;
                }
                else
                {
                    _achievementKeeper[type] = count;
                }
                break;
            case AchievementType.Wave:
                if (add)
                {
                    _achievementKeeper[type] += count;
                }
                else
                {
                    _achievementKeeper[type] = count;
                }
                break;
            case AchievementType.Reset:
                if (add)
                {
                    _achievementKeeper[type] += count;
                }
                else
                {
                    _achievementKeeper[type] = count;
                }
                break;
        }

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
                    RaiseAchievementUnlocked(ach);
                }
            }
        }
    }
}
