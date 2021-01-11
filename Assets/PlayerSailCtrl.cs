﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSailCtrl : MonoBehaviour
{

    public static PlayerSailCtrl Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

    }
    public GameObject sail;
    public float maxDegreeWheel;
    private bool isRotate = false;
    private Vector2 StartPos;
    private float startAngel;

    public Ship ship;
    public bool isSync = false;


    // Start is called before the first frame update
    void Start()
    {
        StartSync();
    }

    public void StartSync()
    {
        isSync = false;
        if (GameManager.instance.playerShip != null)
        {
            ship = GameManager.instance.playerShip.GetComponent<Ship>();
            ship.OnChangeSailDirection.AddListener(UpdateSailInfo);
            UpdateSailInfo();
            isSync = true;
        }
    }

    private void UpdateSailInfo()
    {
        if (!isRotate)
        {
            // Debug.Log("update sail " + (VectorUtils.IsRightSide(ship.ShipDirection, ship.SailDirecion) ? 1 : -1) * Vector2.Angle(ship.ShipDirection, ship.SailDirecion));
            float angel = 90 + (VectorUtils.IsRightSide(ship.ShipDirection, ship.SailDirecion) ? -1 : 1) * Vector2.Angle(ship.ShipDirection, ship.SailDirecion);

            sail.transform.localRotation = Quaternion.Euler(0, 0, angel);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.playerShip == null) isSync = false;
        if (isRotate && isSync)
        {
            Vector2 startV = StartPos - (Vector2)sail.transform.position;
            Vector2 curV = (Vector2)Input.mousePosition - (Vector2)sail.transform.position;
            float angel = Vector2.Angle(startV
                , curV

            );
            StartPos = (Vector2)Input.mousePosition;
            Vector3 cross = Vector3.Cross(startV, curV);
            int sign = cross.z > 0 ? 1 : -1;
            angel = sign * angel + VectorUtils.ConvertAngel(sail.transform.localRotation.eulerAngles.z);
            if (Mathf.Abs(angel) > maxDegreeWheel)
            {
                angel = maxDegreeWheel * (angel > 0 ? 1 : -1);
            }
            sail.transform.localRotation = Quaternion.Euler(0, 0, angel);
            ship.RotateSail(angel);
        }
    }

    public void StartRotate()
    {
        startAngel = VectorUtils.ConvertAngel(sail.transform.localRotation.eulerAngles.z);
        StartPos = Input.mousePosition;
        isRotate = true;
    }

    public void EndRotate()
    {
        isRotate = false;
    }

}