using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableBattleFlow", menuName = "BoS/Battle Flow", order = 15)]
public class ScriptableBattleFlow : MScriptableObject
{
    public ConditionAction[] battlePoints;
    public ScriptableShipFactory[] shipFactorys;
    public ScriptableShipCustom[] ships;
    public ScriptableShipCustom[] shipRewards;
    public int[] goldRewards;

    public void ActiveFlow() => GEventManager.Instance.RegisterBattleFlow(this);

}

