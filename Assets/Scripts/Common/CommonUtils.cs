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
        if (elem == null) return arr;
        if (arr == null)
        {
            return new T[] { elem };
        }
        else
        {
            return arr.Concat(new T[] { elem }).ToArray();
        }
    }

    public static T[] AddElemToArray<T>(T[] arr, T[] elems)
    {
        if (CommonUtils.IsArrayNullEmpty(elems)) return arr;
        if (arr == null)
        {
            return elems;
        }
        else
        {
            return arr.Concat(elems).ToArray();
        }
    }

    internal static T[] RandomElemFromList<T>(List<T> lst, int number = 1, bool removeAfterTake = false)
    {
        T[] result = new T[number];
        int index = 0;

        while (number > 0 && lst.Count > 0)
        {
            int take = Random.Range(0, lst.Count);
            result[index++] = lst[take];
            if (removeAfterTake) lst.RemoveAt(take);
            number--;
        }

        return result;
    }

    internal static T[] RemoveFromArray<T>(T[] array, int indexRemove)
    {
        if (array == null) return array;
        List<T> toList = array.ToList();
        toList.RemoveAt(indexRemove);
        return toList.ToArray();
    }

    public static T CastToEnum<T>(object value)
    {
        return (T)System.Enum.ToObject(typeof(T), value);
    }
}
