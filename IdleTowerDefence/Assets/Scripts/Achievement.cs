using UnityEngine;
using System.Collections;

public class Achievement {

    private int _countToUnlock;
    private bool _isUnlocked;

    public Achievement(int countToUnlock)
    {
        _countToUnlock = countToUnlock;
        _isUnlocked = false;
    }

    public void setCountToUnlock(int countToUnlock) 
    {
        _countToUnlock = countToUnlock;
    }

    public int getCountToUnlock()
    {
        return _countToUnlock;
    }

    public void setIsUnlocked(bool isUnlocked)
    {
        _isUnlocked = isUnlocked;
    }

    public bool getIsUnlocked()
    {
        return _isUnlocked;
    }
}
