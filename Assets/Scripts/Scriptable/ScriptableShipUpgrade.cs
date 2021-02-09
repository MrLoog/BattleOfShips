using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableShipUpgrade", menuName = "BoS/Ship Upgrade", order = 7)]
public class ScriptableShipUpgrade : ScriptableObjectPrefab
{
    [System.Flags]
    public enum UpradeType
    {
        HullHealth, SailHealth
        , CargoCapacity, CrewCapacity
        , CannonRange, CannonDamage
        , Wheel, SailWindAffect
    }

    public UpradeType type;
    public float value;
    public ScriptableShipUpgrade prevUpgrade;
    public ScriptableShipUpgrade nextUpgrade;
}
