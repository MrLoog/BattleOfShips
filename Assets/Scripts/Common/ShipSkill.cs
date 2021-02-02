using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSkill : MonoBehaviour
{
    [System.Flags]
    public enum SkillType
    {
        Active = 0, Passive = 1
    }

    public bool isActiveSkill = false;

    public SkillType type = SkillType.Active;
    public Ship ship;
    public Sprite avatar;
    public virtual bool RegisterShip(Ship ship)
    {
        this.ship = ship;
        if ((type & SkillType.Passive) > 0) ActivePassive();
        return true;
    }

    public virtual void ActivePassive()
    {

    }

    public virtual void ActiveSkill()
    {
        isActiveSkill = true;
    }

    public virtual void DeActiveSkill()
    {
        isActiveSkill = false;
    }

    public virtual void ToggleSkill()
    {
        isActiveSkill = !isActiveSkill;
    }

}
