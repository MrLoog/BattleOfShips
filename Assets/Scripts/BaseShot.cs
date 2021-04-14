using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseShot : MonoBehaviour
{
    protected ScriptableCannonBall data;
    public ScriptableCannonBall Data
    {
        set
        {
            data = value;
        }
        get
        {
            return data;
        }
    }

    public GameObject ShotImg;
    protected Vector2 startPos;
    public Ship owner;
    protected Vector2 fireDirection;
    public UnityEvent OnEndProjectile = new UnityEvent();
    public UnityEvent OnStartProjectile = new UnityEvent();

    public virtual void PrepareFire(Ship owner, Vector2 startPos, Vector2 fireDirection)
    {
        this.startPos = startPos;
        this.owner = owner;
        this.fireDirection = fireDirection;
        OnStartProjectile?.RemoveAllListeners();
        OnEndProjectile?.RemoveAllListeners();
    }
    public virtual void Fire()
    {
        OnStartProjectile?.Invoke();
    }
    public virtual void EndProjectile()
    {
        OnEndProjectile?.Invoke();
    }

    internal virtual void MakeBlocked()
    {

    }

    internal virtual float GetDamageBlocked()
    {
        return data.damage;
    }

    internal virtual void EndProjectileByBlocked()
    {

    }

    public virtual DamageDealShip GetShipDamage()
    {
        return new DamageDealShip()
        {
            hullDamage = data.hullDamage,
            sailDamage = data.sailDamage,
            crewDamage = data.crewDamage
        };
    }
}
