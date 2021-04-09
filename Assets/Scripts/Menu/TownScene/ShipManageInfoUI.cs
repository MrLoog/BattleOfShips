using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShipManageInfoUI : ShipManageInfo
{
    public Text txtShipModel;

    private string templateInfo;

    private void Awake()
    {
        templateInfo = statsContent.GetComponent<TextMeshProUGUI>().text;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public override void ShowData()
    {
        // ClearStats();

        ScriptableShip curState = data.curShipData;
        ScriptableShip originState = data.PeakData;
        if (!WorkshopMode)
        {
            txtShipName.text = (data.shipName?.Length > 0) ? data.shipName : "No Name";
        }
        txtShipModel.text = curState.typeName;

    }

    private void ClearStats()
    {
    }

    private void DisplayInfo(string info)
    {

    }

}
