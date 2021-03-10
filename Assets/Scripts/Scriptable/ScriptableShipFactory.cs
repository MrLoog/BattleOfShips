using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableShipFactory", menuName = "BoS/Ship Factory", order = 11)]
public class ScriptableShipFactory : MScriptableObject
{
    public ScriptableShip[] shipList;
    public int[] shipRates;

    public int[] slotRate;

    public ScriptableShipSkill[] skillList;

    public ScriptableShipMaterial[] hullMaterials;
    public int[] hullMaterialRates;
    public ScriptableShipMaterial[] sailMaterials;
    public int[] sailMaterialRates;

    public ScriptableShipCustom[] GetRandomShip(int quantity = 1)
    {
        ScriptableShipCustom[] result = new ScriptableShipCustom[quantity];

        List<ScriptableShip> shipPool = shipList.Where(x => x != null).ToList();
        List<ScriptableShipSkill> skillPool = skillList?.Where(x => x != null).ToList();

        for (int n = 0; n < quantity; n++)
        {
            List<ScriptableShipSkill> skillPool2 = skillPool?.ToList();
            int choiceShip = -1;
            if (!CommonUtils.IsArrayNullEmpty(shipRates))
            {
                choiceShip = CommonUtils.RandomByRate(shipRates);
            }
            else
            {
                choiceShip = Random.Range(0, shipPool.Count);
            }
            ScriptableShipCustom resultShip = ScriptableShipCustom.CreateInstance<ScriptableShipCustom>();


            resultShip.baseShipData = shipPool[choiceShip].Clone<ScriptableShip>();

            //random skill
            if (slotRate != null && slotRate.Length > 0)
            {
                int numberSlot = 1 + CommonUtils.RandomByRate(slotRate);
                ScriptableShipSkill[] choiceSkills = new ScriptableShipSkill[numberSlot];
                for (int i = 0; i < numberSlot; i++)
                {
                    int choiceSkill = Random.Range(0, skillPool2.Count);
                    choiceSkills[i] = skillPool2[choiceSkill];
                    skillPool2.RemoveAt(choiceSkill);
                }
                resultShip.skills = choiceSkills;
            }
            //random material hull
            if (!CommonUtils.IsArrayNullEmpty(hullMaterials))
            {
                int choice = -1;
                if (!CommonUtils.IsArrayNullEmpty(hullMaterialRates) && hullMaterials.Length == hullMaterialRates.Length)
                {
                    choice = CommonUtils.RandomByRate(hullMaterialRates);
                }
                else
                {
                    choice = Random.Range(0, hullMaterials.Length);
                }
                resultShip.hullMaterial = hullMaterials[choice];
            }
            //random material sail
            if (!CommonUtils.IsArrayNullEmpty(sailMaterials))
            {
                int choice = -1;
                if (!CommonUtils.IsArrayNullEmpty(sailMaterialRates) && sailMaterials.Length == sailMaterialRates.Length)
                {
                    choice = CommonUtils.RandomByRate(sailMaterialRates);
                }
                else
                {
                    choice = Random.Range(0, sailMaterials.Length);
                }
                resultShip.sailMaterial = sailMaterials[choice];
            }


            resultShip.curShipData = resultShip.PeakData.Clone<ScriptableShip>();

            result[n] = resultShip;
        }


        return result;
    }
}
