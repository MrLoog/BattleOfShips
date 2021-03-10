using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

public class CommonUtils
{
    public static int RandomByRate(int[] weights)
    {
        int maxRange = 0;
        int[,] level = new int[weights.Length, 2];
        for (int i = 0; i < weights.Length; i++)
        {
            level[i, 0] = maxRange;
            maxRange += weights[i];
            level[i, 1] = maxRange;
        }
        int selected = Random.Range(0, maxRange);

        for (int i = 0; i < weights.Length; i++)
        {
            if (level[i, 0] <= selected && level[i, 1] > selected)
            {
                return i;
            }
        }
        return -1;
    }

    public static bool IsArrayNullEmpty<T>(T[] arr)
    {
        if (arr == null || arr.Length == 0)
        {
            return true;
        }
        return false;
    }

    public static bool IsStrMatchPattern(string check, string pattern)
    {
        Regex rgx = new Regex(pattern);
        return rgx.IsMatch(check);
    }

    public static T[] AddElemToArray<T>(T[] arr, T elem)
    {
        if (arr == null)
        {
            return new T[] { elem };
        }
        else
        {
            return arr.Concat(new T[] { elem }).ToArray();
        }
    }
}
