using UnityEngine;
using System.Collections;

public class Achievement {

    private int _countToUnlock;
    private bool _isUnlocked;
	private string _title;
	private string _subTitle;
	private GameObject _achievementObject;

    public Achievement(int countToUnlock)
    {
        _countToUnlock = countToUnlock;
        _isUnlocked = false;

    }

	public Achievement(int countToUnlock, string title, string subTitle)
	{
		_countToUnlock = countToUnlock;
		_isUnlocked = false;
		_title = title;
		_subTitle = subTitle;

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

	public void setTitle(string title)
	{
		_title = title;
	}

	public string getTitle()
	{
		return _title;
	}

	public void setSubTitle(string subtitle)
	{
		_subTitle = subtitle;
	}

	public string getSubTitle()
	{
		return _subTitle;
	}

	public void setObject(GameObject _object)
	{
		_achievementObject=_object;
	}

	public GameObject getObject()
	{
		return _achievementObject;
	}
}
