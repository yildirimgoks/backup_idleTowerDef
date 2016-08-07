using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using Assets.Scripts;

[DataContract]
public class WaveData
{
    public static readonly BigIntWithUnit BaseCurrencyGivenOnDeath = 100;
    public static readonly BigIntWithUnit BaseLife = 100;

    [DataMember]
    private int _currentWave;

    [DataMember]
    private int _maxWave;

    public WaveData()
    {
        _currentWave = 0;
        _maxWave = 0;
    }
    
    public bool IsBossWave
    {
        get { return (_currentWave + 1) % 5 == 0; }
    }

    public bool IsNextWaveBossWave
    {
        get { return (_currentWave + 1) % 5 == 4; }
    }

    public int CurrentWave
    {
        get { return _currentWave; }
    }

    public void IncreaseCurrentWaveAndMaxWave()
    {
        if (_maxWave == _currentWave)
        {
            _maxWave += 1;
        }
        _currentWave += 1;
    }

    public bool IncreaseCurrentWaveIfLessThanMax()
    {
        if (_maxWave <= _currentWave) return false;
        _currentWave += 1;
        return true;
    }

    public void DecreaseCurrentWave()
    {
        _currentWave -= 1;
    }

    //ToDo: fix when waves are customized
    public int GetCurrentWaveLength()
    {
        return 10;
    }

    public int GetMaxReachedWave()
    {
        return _maxWave;
    }

    public MinionData GetMinionDataForCurrentWave()
    {
        var multiplierLife = System.Math.Pow(1.1, CurrentWave);

        if (IsBossWave)
        {
            var bossLife = (BigIntWithUnit) (System.Math.Ceiling(multiplierLife) * 1000);
            return new MinionData(bossLife, bossLife, 10f);
        }

        var life = BigIntWithUnit.MultiplyPercent(BaseLife, multiplierLife * 100);

        var multiplierMoney = System.Math.Pow(1.03, CurrentWave);
        var currencyGivenOnDeath = BigIntWithUnit.MultiplyPercent(BaseCurrencyGivenOnDeath, multiplierMoney * 100);

        return new MinionData(life, currencyGivenOnDeath, 10f);
    }
}
