using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableShipMaterial", menuName = "BoS/Ship Material", order = 13)]

public class ScriptableShipMaterial : ScriptableShipGoods
{
    public enum MaterialType
    {
        Hull, Sail
    }

    public MaterialType materialType;

    public float effectRate = 1f;

    public float speedAffect = 1f;

    public float defense = 0f;
}
