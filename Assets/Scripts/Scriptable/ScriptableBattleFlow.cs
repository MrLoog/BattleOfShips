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

    public RewardBattle reward;
    public void ActiveFlow() => GEventManager.Instance.RegisterBattleFlow(this);

}

[Serializable]
public class RewardBattle
{
    public ScriptableShipCustom[] ships;
    public int[] gold;
    public long[] exp;
    public int[] gem;
    internal string[] goodsCode;
    internal int[] goodsQty;

    public void Clear()
    {
        ships = null;
        gold = null;
        exp = null;
        gem = null;
        goodsCode = null;
        goodsQty = null;
    }
}