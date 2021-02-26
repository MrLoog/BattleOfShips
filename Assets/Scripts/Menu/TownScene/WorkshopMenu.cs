using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WorkshopMenu : MonoBehaviour
{
    public static WorkshopMenu Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

    }
    public GameObject panel;
    public GameObject scrollView;
    public GameObject shipListContent;
    public GameObject prefabShipInfo;
    public GameObject prefabInfoRow;

    private Workshop workshop;

    private ScriptableShipCustom[] shipCustoms;

    public ScriptableShipCustom[] ShipCustomList
    {
        set
        {
            shipCustoms = value;
        }
        get
        {
            return shipCustoms;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void ShowAllShip()
    {

        for (int i = shipListContent.transform.childCount; i > 0; i--)
        {
            Destroy(shipListContent.transform.GetChild(i - 1).gameObject);
        }
        if (workshop == null)
        {
            workshop = TownManager.Instance.GetWorkshopData();
            ShipCustomList = workshop.workshopShips;
        }
        for (int i = 0; i < ShipCustomList.Length; i++)
        {
            ShowShip(ShipCustomList[i]);
        }
        panel.SetActive(true);
    }

    private void ShowShip(ScriptableShipCustom data)
    {
        GameObject newShipInfo = Instantiate(prefabShipInfo, shipListContent.transform, false);
        ShipManageInfo scriptInfo = newShipInfo.GetComponent<ShipManageInfo>();
        scriptInfo.SetShipData(data, prefabInfoRow);
        newShipInfo.SetActive(true);
    }

    public void HideWorkshop()
    {
        panel.SetActive(false);
    }
    private ShipManageInfo focusShip;
}
