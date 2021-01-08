using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableStateShip", menuName = "BoS/State Ship", order = 3)]
public class ScriptableStateShip : ScriptableObjectPrefab
{
    public string nameGroupState;
    public Sprite normalState;
    public Sprite damagedState;
    public Sprite dangerState;
    public Sprite deathState;
}
