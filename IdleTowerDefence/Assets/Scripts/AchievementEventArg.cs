using UnityEngine;
using System.Collections;
using System;


public class AchievementEventArg : EventArgs {

    public Achievement Data;
    public AchievementEventArg(Achievement e)
    {
        Data = e;
    }
}
