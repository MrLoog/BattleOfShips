using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;

public class GActionManager : Singleton<GActionManager>
{
    protected GActionManager() { }

    internal bool Check(ActionApi aCheck)
    {
        return (bool)this.GetType().GetMethod(aCheck.actionName)?.Invoke(this, new object[] { aCheck.actionParams });
    }

    internal void Perform(ActionApi aAction)
    {
        this.GetType().GetMethod(aAction.actionName)?.Invoke(this, new object[] { aAction.actionParams });
    }

    #region check
    public bool IsClearLevel(params string[] args)
    {
        string levelNamePattern = args[0];
        return GameManager.Instance.GameData?.IsLevelClearedPattern(levelNamePattern) ?? false;
    }
    #endregion

    #region action
    public void UnlockShopShip(params string[] args)
    {
        string typeNameShip = args[0];
        ScriptableShipFactory curShipShop = GameManager.Instance.GameData?.shipShopFactory;
        Debug.Assert(curShipShop != null, "Should have data");
        if (curShipShop == null) return;

        object find = curShipShop.shipList?.FirstOrDefault(x => x.typeName == typeNameShip);
        if (find == null)
        {
            ScriptableShip shipUnlock = MyResourceUtils.ResourcesLoadAll<ScriptableShip>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS).FirstOrDefault(x => x.typeName == typeNameShip);

            Debug.Assert(shipUnlock != null, "Should have data");
            if (shipUnlock == null) return;
            curShipShop.shipList = CommonUtils.AddElemToArray<ScriptableShip>(curShipShop.shipList, shipUnlock);
        }
    }

    public void UnlockShopMaterial(params string[] args)
    {
        try
        {
            string level = args[0];
            int index = Int16.Parse(level);
            ScriptableShipFactory curShipShop = GameManager.Instance.GameData?.shipShopFactory;
            Debug.Assert(curShipShop != null, "Should have data");
            if (curShipShop == null) return;

            ScriptableShipFactory final = GameManager.Instance.FinalShopFactory;

            ScriptableShipMaterial hull = final?.hullMaterials?[index];

            if (hull != null)
            {
                if (curShipShop.hullMaterials?.FirstOrDefault(x => x.name == hull.name) == null)
                {
                    curShipShop.hullMaterials = CommonUtils.AddElemToArray(curShipShop.hullMaterials, hull);
                    int? hullRate = final?.hullMaterialRates?[index];
                    if (hullRate != null)
                    {
                        curShipShop.hullMaterialRates = CommonUtils.AddElemToArray(curShipShop.hullMaterialRates, (int)hullRate);
                    }
                }
            }

            ScriptableShipMaterial sail = final?.sailMaterials?[index];
            if (sail != null)
            {
                if (curShipShop.sailMaterials?.FirstOrDefault(x => x.name == sail.name) == null)
                {
                    curShipShop.sailMaterials = CommonUtils.AddElemToArray(curShipShop.sailMaterials, sail);
                    int? sailRate = final?.sailMaterialRates?[index];
                    if (sailRate != null)
                    {
                        curShipShop.sailMaterialRates = CommonUtils.AddElemToArray(curShipShop.sailMaterialRates, (int)sailRate);
                    }
                }
            }



        }
        catch (Exception)
        {
            Debug.Log("Error UnlockShopMaterial");
        }
    }
    #endregion


    #region test
    public bool CheckTestTrue(params string[] args)
    {
        foreach (string val in args)
            Debug.Log(string.Format("CheckTestTrue {0}", val));
        return true;
    }

    public bool CheckTestFalse(params string[] args)
    {
        foreach (string val in args)
            Debug.Log(string.Format("CheckTestFalse {0}", val));
        return false;
    }

    public void ActionTest(params string[] args)
    {
        foreach (string val in args)
            Debug.Log(string.Format("ActionTest {0}", val));
    }

    #endregion
}
