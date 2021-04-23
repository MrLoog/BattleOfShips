using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem
{
    public long expStep = 100;
    public long GetExpNextLevel(int curLevel)
    {
        if (curLevel <= 1) return 0;
        return expStep * (curLevel - 1);
    }

    public long GetTotalExpToLevel(int level)
    {
        long exp = 0;
        for (int i = 0; i < level; i++)
        {
            exp += GetExpNextLevel(i);
        }
        return exp;
    }
}
