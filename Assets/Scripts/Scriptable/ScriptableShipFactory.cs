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

    public int[] slotRate;

    public ScriptableShipSkill[] skillList;

    public ScriptableShipCustom[] GetRandomShip(int quantity = 1)
    {
        ScriptableShipCustom[] result = new ScriptableShipCustom[quantity];

        List<ScriptableShip> shipPool = shipList.Where(x => x != null).ToList();
        List<ScriptableShipSkill> skillPool = skillList.Where(x => x != null).ToList();

        for (int n = 0; n < quantity; n++)
        {
            List<ScriptableShipSkill> skillPool2 = skillPool.ToList();
            int choiceShip = (int)Random.Range(0, shipPool.Count - 1);
            ScriptableShipCustom resultShip = ScriptableShipCustom.CreateInstance<ScriptableShipCustom>();
            resultShip.baseShipData = shipPool[choiceShip].Clone<ScriptableShip>();
            resultShip.curShipData = shipPool[choiceShip].Clone<ScriptableShip>();

            int numberSlot = 1 + CommonUtils.RandomByRate(slotRate);
            ScriptableShipSkill[] choiceSkills = new ScriptableShipSkill[numberSlot];
            for (int i = 0; i < numberSlot; i++)
            {
                int choiceSkill = (int)Random.Range(0, skillPool2.Count - 1);
                choiceSkills[i] = skillPool2[choiceSkill];
                skillPool2.RemoveAt(choiceSkill);
            }
            resultShip.skills = choiceSkills;

            result[n] = resultShip;
        }


        return result;
    }
}
