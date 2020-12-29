using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWheelCtrl : MonoBehaviour
{
    public GameObject wheel;
    public float maxDegreeWheel;
    private bool isRotate = false;
    private Vector2 StartPos;
    private float startAngel;

    public Ship ship;
    private float spdReturn = 3f;
    private float accReturn = 0f;
    private bool isReturn = false;
    private Quaternion beginReturn;
    // Start is called before the first frame update
    void Start()
    {
        ship = GameManager.instance.playerShip.GetComponent<Ship>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotate)
        {
            /*
            Vector2 startV = StartPos - (Vector2)wheel.transform.position;
            Vector2 curV = (Vector2)Input.mousePosition - (Vector2)wheel.transform.position;
            float angel = Vector2.Angle(startV
                , curV

            );
            Vector3 cross = Vector3.Cross(startV, curV);
            int sign = cross.z > 0 ? 1 : -1;
            angel = sign * angel + startAngel;
            if (Mathf.Abs(angel) > maxDegreeWheel)
            {
                angel = maxDegreeWheel * (angel > 0 ? 1 : -1);
            }
            wheel.transform.localRotation = Quaternion.Euler(0, 0, angel);
            ship.CalculateRotateVector(ConvertAngel(angel));
            */
            Vector2 startV = StartPos - (Vector2)wheel.transform.position;
            Vector2 curV = (Vector2)Input.mousePosition - (Vector2)wheel.transform.position;
            float angel = Vector2.Angle(startV
                , curV

            );
            StartPos = (Vector2)Input.mousePosition;
            Vector3 cross = Vector3.Cross(startV, curV);
            int sign = cross.z > 0 ? 1 : -1;
            angel = sign * angel + VectorUtils.ConvertAngel(wheel.transform.localRotation.eulerAngles.z);
            if (Mathf.Abs(angel) > maxDegreeWheel)
            {
                angel = maxDegreeWheel * (angel > 0 ? 1 : -1);
            }
            wheel.transform.localRotation = Quaternion.Euler(0, 0, angel);
            ship.CalculateRotateVector(angel);
        }
        else
        {
            if (isReturn)
            {
                accReturn += Time.deltaTime;
                wheel.transform.localRotation = Quaternion.Lerp(beginReturn, Quaternion.Euler(0, 0, 0), accReturn / spdReturn);

                if (accReturn > spdReturn)
                {
                    accReturn = 0;
                    isReturn = false;
                    wheel.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                ship.CalculateRotateVector(ConvertAngel(wheel.transform.localRotation.eulerAngles.z));
            }
        }
    }

    public void StartRotate()
    {
        startAngel = wheel.transform.localRotation.eulerAngles.z;
        StartPos = Input.mousePosition;
        isRotate = true;
    }

    public void EndRotate()
    {
        isRotate = false;

        beginReturn = wheel.transform.localRotation;
        spdReturn = Mathf.Abs(ConvertAngel(wheel.transform.localRotation.eulerAngles.z)) / (180 / ship.rotateSpeed);
        Debug.Log(string.Format("return {0}/{1}/{2}", wheel.transform.localRotation.eulerAngles.z, ship.rotateSpeed, spdReturn));
        accReturn = 0;
        isReturn = true;
    }

    private float ConvertAngel(float angel)
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

    public void TestRotate()
    {
        Ship s = GameManager.instance.playerShip.GetComponent<Ship>();
        float angel = wheel.transform.localRotation.eulerAngles.z;
        if (angel > 180)
        {
            angel = -(360 - angel);
        }
        Vector2 rs = s.CalculateRotateVector(angel);
        // Debug.Log("rotate " + rs + "/" + angel);
        rs = rs.normalized * 3f;
        Debug.DrawLine(s.transform.position, s.transform.position + (Vector3)rs, Color.red, 3f);
    }
}
