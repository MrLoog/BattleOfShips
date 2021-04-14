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
        LoadTemplate();
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
        LoadTemplate();
        statsContent.GetComponent<TextMeshProUGUI>().text = templateInfo
                .Replace("{cch}", curState.maxCrew.ToString())
                .Replace("{chh}", curState.hullHealth.ToString())
                .Replace("{csh}", curState.sailHealth.ToString())
                .Replace("{pch}", originState.maxCrew.ToString())
                .Replace("{phh}", originState.hullHealth.ToString())
                .Replace("{psh}", originState.sailHealth.ToString())
                ;
    }

    private void LoadTemplate()
    {
        if (templateInfo == null)
        {
            templateInfo = statsContent.GetComponent<TextMeshProUGUI>().text;
        }
    }

    private void ClearStats()
    {
    }

}
