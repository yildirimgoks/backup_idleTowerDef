using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Manager;

public class MinionData : ICloneable {

    private BigIntWithUnit _life;
    private BigIntWithUnit _currencyGivenOnDeath;
    private float _speed;
    private bool _mageLoot;

    public MinionData()
    {
        _life = UpgradeManager.MinionLifeInitial;
        _currencyGivenOnDeath = UpgradeManager.MinionDeathRewardInitial;
        _speed = 10f;
        _mageLoot = false;
    }

    public MinionData(BigIntWithUnit life, BigIntWithUnit deathCurrency, float speed)
    {
        _life = life;
        _currencyGivenOnDeath = deathCurrency;
        _speed = speed;
        _mageLoot = false;
    }

    public bool HasMageLoot()
    {
        return _mageLoot;
    }

    public void SetMageLoot(bool mageLoot)
    {
        _mageLoot = mageLoot;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public bool IsAlive()
    {
        return _life > 0;
    }

    public BigIntWithUnit GetDeathLoot()
    {
        return _currencyGivenOnDeath;
    }

    public void Kill()
    {
        _life = 0;
    }


    /// <summary>
    /// If you want damage floating text, call minion.DecreaseLife instead
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public BigIntWithUnit DecreaseLife(BigIntWithUnit damage)
    {
        _life -= damage;
        return _life;
    }

    public BigIntWithUnit GetCurrentLife()
    {
        return _life;
    }

    public object Clone()
    {
        return new MinionData((BigIntWithUnit)_life.Clone(), (BigIntWithUnit)_currencyGivenOnDeath.Clone(), _speed);
    }
}
