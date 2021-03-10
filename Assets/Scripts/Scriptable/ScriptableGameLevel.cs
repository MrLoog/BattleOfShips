using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableGameLevel", menuName = "BoS/Game Level", order = 12)]
public class ScriptableGameLevel : MScriptableObject
{
    public ScriptableGameLevel[] nextLevel;

    public bool isMainLevel = true;
    public string codeName;
    public string shortName;
    public string levelName;
    [TextArea(4, 100)]
    public string description;
    public ScriptableBattleFlow battleFlow;
    public RewardBattle reward;
}
