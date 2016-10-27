using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Model;

public class MageUpgradeManager : MonoBehaviour {

    public string[] MagePrefabNames;
    public Mage[] MagePrefabs;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Init()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("Fire", typeof(TextAsset));
        MageData.ReadUpgradeInfo(Element.Fire, ParseTextAsset(textAsset));
        textAsset = (TextAsset)Resources.Load("Water", typeof(TextAsset));
        MageData.ReadUpgradeInfo(Element.Water, ParseTextAsset(textAsset));
        textAsset = (TextAsset)Resources.Load("Air", typeof(TextAsset));
        MageData.ReadUpgradeInfo(Element.Air, ParseTextAsset(textAsset));
        textAsset = (TextAsset)Resources.Load("Earth", typeof(TextAsset));
        MageData.ReadUpgradeInfo(Element.Earth, ParseTextAsset(textAsset));
    }

    private List<MageUpgradeInfo> ParseTextAsset(TextAsset textAsset)
    {
        var lines = textAsset.text.Split('\n');
        var mageInfo = new List<MageUpgradeInfo>();
        for (var i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');

            MageUpgradeInfo info;
            info.Type = values[1];
            info.SpellDamage = new BigIntWithUnit(values[2]);
            info.SpellRange = float.Parse(values[3]);
            info.Delay = float.Parse(values[4]);
            info.UpgradePrice = new BigIntWithUnit(values[5]);
            info.SkillDamage = new BigIntWithUnit(values[6]);

            mageInfo.Add(info);
        }
        return mageInfo;
    }
}
