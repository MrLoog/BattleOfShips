using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableShipGoods", menuName = "BoS/Ship Goods", order = 5)]

public class ScriptableShipGoods : ScriptableObjectPrefab
{
    public string itemName;
    public string description;
    public float weight;
    public Sprite image;

    public string codeName;

    public int basePrice;
    public int baseQuantiy;

}
