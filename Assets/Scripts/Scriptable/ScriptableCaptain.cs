using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableCaptain", menuName = "BoS/Captain", order = 8)]
public class ScriptableCaptain : MScriptableObject
{
    public string captainName;
    public float healthUnit;
    public float attackUnit;
    public float healthPoint;
    public float attackPoint;

    public float health;
    public float attack;
    public int remainPoint;
    public int level;
    public long expLevel;
}
