using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManageMiniMenu : MonoBehaviour
{
    public GameObject btnMakeMain;
    public GameObject btnSell;
    public GameObject btnRepair;
    public GameObject btnTransfer;
    public GameObject btnMarket;
    public GameObject btnUpgrade;

    public ShipManageInfo focusShip;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (focusShip != null)
        {
            transform.position = focusShip.transform.position;
        }
    }

    public void ValidFeatureAvaiable()
    {
        // btnMakeMain.SetActive(false);
        // btnSell.SetActive(false);
        // btnRepair.SetActive(false);
        // btnTransfer.SetActive(false);
        // btnHireCrew.SetActive(false);
        // btnUpgrade.SetActive(false);

        btnMakeMain.SetActive(!focusShip.isMainShip);
        btnSell.SetActive(!focusShip.isMainShip
        && ShipHelper.IsCanSell(focusShip.data)
        );
        ScriptableShipCustom data = focusShip.data;
        btnRepair.SetActive(
            ShipHelper.IsNeedRepair(focusShip.data.curShipData, focusShip.data.PeakData)
        );
        btnTransfer.SetActive(true);
        btnMarket.SetActive(true);
        btnUpgrade.SetActive(true);
    }

    public void ShowMiniMenu(ShipManageInfo shipManageInfo)
    {

        if (shipManageInfo == null || shipManageInfo.Equals(focusShip))
        {
            CloseMiniMenu();
        }
        else
        {
            focusShip = shipManageInfo;
            transform.position = focusShip.transform.position;
            ValidFeatureAvaiable();
            gameObject.SetActive(true);
        }
    }


    public void CloseMiniMenu()
    {
        gameObject.SetActive(false);
        focusShip = null;
    }
}
