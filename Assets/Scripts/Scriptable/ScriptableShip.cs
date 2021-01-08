using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableShip", menuName = "BoS/Ship", order = 2)]
public class ScriptableShip : ScriptableObjectPrefab
{
    [System.Flags]
    public enum ShipType
    {
        Normal = 0, Small = 1, Medium = 2, Transport = 4, War = 8
    }

    public string typeName;
    public ShipType type;
    public int maxCrew;
    public float hullHealth;
    public float sailHealth;
    public float oarsSpeed;
    public float MaxDegreeRotate;
    public float windConversionRate;
    public float capacity;
    public float capacityWeightRate;
    public float sizeRateWidth;
    public float sizeRateLength;
    public int numberDeck;
    public int[] numberCannons;

    public float MaxHitToStun = 5f;
    public float TimeHitToStun = 1f;
    public float TimeRegainStun = 5f;
}
