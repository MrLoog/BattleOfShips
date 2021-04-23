using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Slider sailSet;
    public Toggle toggleSail;

    public float sailChangeStep = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        StartSync();
    }

    public void StartSync()
    {
        isSync = false;
        if (SeaBattleManager.Instance.playerShip != null)
        {
            ship = SeaBattleManager.Instance.playerShip.GetComponent<Ship>();
            ship.OnChangeSailDirection.AddListener(UpdateSailInfo);
            sailSet.value = ship.sailSet;
            toggleSail.isOn = ship.AutoSail;
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
        if (SeaBattleManager.Instance.playerShip == null) isSync = false;
        if (!isSync) return;
        UpdateSailUpDown();
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

    public void ChangeSailSet()
    {
        ship?.SetSail(sailSet.value);
    }

    public void ToggleAutoSail()
    {
        if (ship != null)
        {
            ship.AutoSail = toggleSail.isOn;
        }
    }

    public void DownSail(float value = -1)
    {
        if (value != -1)
        {
            sailSet.value = value;
        }
        else
        {
            sailSet.value -= (sailSet.value > sailChangeStep ? sailChangeStep : sailSet.value);
        }
    }

    public void UpSail(float value = -1)
    {
        if (value != -1)
        {
            sailSet.value = value;
        }
        else
        {
            sailSet.value += ((sailSet.maxValue - sailSet.value) > sailChangeStep ? sailChangeStep : (sailSet.maxValue - sailSet.value));
        }
    }

    public float timeHold = 0.5f;
    public float accumTimeHold = 0f;
    public int holdBtnSailCtrl = 0;
    public bool isHold = false;

    private void UpdateSailUpDown()
    {
        if (isHold || accumTimeHold > 0)
        {
            if (!isHold)
            {
                accumTimeHold = 0f;
                Debug.Log("DownUpSail Change Sail Release");
                if (holdBtnSailCtrl == -1) DownSail();
                else UpSail();
            }
            else
            {
                accumTimeHold += Time.deltaTime;
                if (accumTimeHold >= timeHold)
                {
                    Debug.Log("DownUpSail Change Sail Hold");
                    accumTimeHold = 0.01f;
                    if (holdBtnSailCtrl == -1) DownSail();
                    else UpSail();
                }
            }
        }
    }
    public void DownSailPress()
    {
        Debug.Log("DownUpSail DownSailPress");
        holdBtnSailCtrl = -1;
        isHold = true;
    }

    public void DownUpSailRelease()
    {
        Debug.Log("DownUpSail DownUpSailRelease");
        isHold = false;
    }
    public void UpSailPress()
    {
        Debug.Log("DownUpSail UpSailPress");
        holdBtnSailCtrl = 1;
        isHold = true;
    }

    public void DownSailZeroPress()
    {
        Debug.Log("DownUpSail DownSailZeroPress");
        DownSail(0);
    }

    public void UpSailFullPress()
    {
        Debug.Log("DownUpSail UpSailFullPress");
        UpSail(1f);
    }
}
