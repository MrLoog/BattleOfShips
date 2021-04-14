using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableShipSkill", menuName = "BoS/Ship Skill", order = 4)]
public class ScriptableShipSkill : ScriptableObjectPrefab
{
    public string codeName;
    public string skillName;
    [TextArea(3, 10)]
    public string description;
}
