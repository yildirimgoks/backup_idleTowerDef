using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class MinionData {

    private BigIntWithUnit _life;
    private BigIntWithUnit _currencyGivenOnDeath;
    private float _speed;

    public MinionData()
    {
        _life = 100;
        _currencyGivenOnDeath = 100;
        _speed = 10f;
    }

    public MinionData(BigIntWithUnit life, BigIntWithUnit deathCurrency, float speed)
    {
        _life = life;
        _currencyGivenOnDeath = deathCurrency;
        _speed = speed;
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

    public BigIntWithUnit DecreaseLife(BigIntWithUnit damage)
    {
        _life -= damage;
        return _life;
    }

    public BigIntWithUnit GetCurrentLife()
    {
        return _life;
    }
       
}
