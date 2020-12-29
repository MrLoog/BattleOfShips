using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorUtils
{
    public static Vector2 Rotate(Vector2 v, float degrees, bool isNew = false)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        if (isNew)
        {
            return new Vector2((cos * tx) - (sin * ty)
            , (sin * tx) + (cos * ty)
            );
        }
        else
        {
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

    }

    public static bool IsRightSide(Vector2 mainV, Vector2 checkV)
    {
        Vector3 cross = Vector3.Cross(mainV, checkV);
        // int sign = cross.z > 0 ? 1 : -1;
        return cross.z < 0;
    }

    public static float ConvertAngel(float angel)
    {
        if (angel > 180)
        {
            return -(360 - angel);
        }
        else
        {
            return angel;
        }
    }

    public static Vector2 GetForceOnLine(Vector2 line, Vector2 force, bool onLine = false)
    {
        Vector2 vProj = Vector2.Dot(line, force) * line / Mathf.Pow(line.magnitude, 2);
        if (onLine)
        {
            return vProj;
        }
        else
        {
            return force - vProj;
        }
    }

    public static bool IsSameDirection(Vector2 v1, Vector2 v2)
    {
        // Debug.Log(string.Format("same direction {0}/{1}/{2}", v1, v2, Vector2.Dot(v1, v2)));
        //-1 opposite
        //0 perpendicular
        //1 same direction
        return Vector2.Dot(v1, v2) > 0;
    }
}
