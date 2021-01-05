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

    public static Vector2 Reverse(Vector2 v)
    {
        return new Vector2(-v.x, -v.y);
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

    public static Vector2 GetForceOnLine(Vector2 line, Vector2 force, bool onLine = true)
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

    public static float AreaTriangle(Vector2 a, Vector2 b, Vector2 c)
    {
        return GetForceOnLine(a - c, b - c, false).magnitude * (b - c).magnitude / 2;
    }
    public static bool IsPointInRectangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        float areaRect = ((b - a).magnitude + 0.1f) * ((c - b).magnitude + 0.1f);
        float areaFromPoint = AreaTriangle(a, b, p);
        areaFromPoint += AreaTriangle(b, c, p);
        areaFromPoint += AreaTriangle(c, d, p);
        areaFromPoint += AreaTriangle(d, a, p);
        if (areaFromPoint > areaRect)
        {
            Debug.Log("Area " + areaFromPoint + " / " + areaRect);
            Debug.DrawLine(a, b, Color.red, 1f);
            Debug.DrawLine(b, c, Color.green, 1f);
            Debug.DrawLine(c, d, Color.blue, 1f);
            Debug.DrawLine(d, a, Color.yellow, 1f);
            Debug.DrawLine(a, p, Color.gray, 1f);
            return false;
        }
        Debug.Log("Area " + areaFromPoint + " / " + areaRect);
        return true;
    }
}
