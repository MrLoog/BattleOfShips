using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonUtils
{
    public static int RandomByRate(int[] weights)
    {
        float maxRange = 0f;
        float[,] level = new float[weights.Length, 2];
        for (int i = 0; i < weights.Length; i++)
        {
            level[i, 0] = maxRange;
            maxRange += weights[i];
            level[i, 1] = maxRange;
        }
        float selected = Random.Range(0f, maxRange - 1);

        for (int i = 0; i < weights.Length; i++)
        {
            if (level[i, 0] <= selected && level[i, 1] > selected)
            {
                return i;
            }
        }
        return -1;
    }
}
