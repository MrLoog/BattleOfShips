using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShipManageInfo : MonoBehaviour
{
    public bool isMainShip = false;
    public Text txtShipName;
    public GameObject statsContent;

    public ScriptableShipCustom data;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void SetShipData(ScriptableShipCustom s, GameObject prefab)
    {
        this.prefab = prefab;
        data = s;
        if (data.curShipData == null)
        {
            data.curShipData = data.PeakData;
        }
        ShowData();
    }

    public void ShowData()
    {
        ClearStats();

        ScriptableShip curState = data.curShipData;
        ScriptableShip originState = data.PeakData;

        txtShipName.text = (data.shipName?.Length > 0) ? data.shipName : "No Name";
        DisplayInfo(string.Format("Crew:{0}/{1}", curState.maxCrew, originState.maxCrew));
        DisplayInfo(string.Format("Hull Health:{0}/{1}", curState.hullHealth, originState.hullHealth));
        DisplayInfo(string.Format("Sail Health:{0}/{1}", curState.sailHealth, originState.sailHealth));
        DisplayInfo(string.Format("Oar Speed: {0}", curState.oarsSpeed));
        DisplayInfo(string.Format("Wheel: {0}°", curState.MaxDegreeRotate));
        DisplayInfo(string.Format("Wind Affect:{0}", curState.windConversionRate));
        DisplayInfo(string.Format("Capacity: {0}/{1}", curState.capacity, originState.capacity));
        DisplayInfo(string.Format("Goods Rate: {0}", curState.capacityWeightRate));
        DisplayInfo(string.Format("Deck: {0}", curState.numberDeck));
        DisplayInfo(string.Format("Cannons: {0}", curState.numberCannons.Sum()));
    }

    private void ClearStats()
    {
        MyGameObjectUtils.ClearAllChilds(statsContent);
    }

    private void DisplayInfo(string info)
    {
        GameObject newRow = Instantiate(prefab, statsContent.transform, false);
        Text text = newRow.GetComponent<Text>();
        text.text = info;
    }

    public void ShowMiniMenu()
    {
        ShipManageMenu.Instance.ShowMiniMenu(this);
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
