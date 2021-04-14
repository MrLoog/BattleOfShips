using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ShipInfoDetails : MonoBehaviour
{
    public static ShipInfoDetails Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

    }
    public GameObject panel;
    private string templateInfo;
    private ScriptableShipCustom data;
    public GameObject txtInfo;
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowData(ScriptableShipCustom data)
    {
        if (this.data != null && this.data.Equals(data))
        {
            HidePanel();
        }
        else
        {
            this.data = data;
            txtInfo.GetComponent<TextMeshProUGUI>().text = BuildInfoShow();
            panel.SetActive(true);
        }
    }

    public void HidePanel()
    {
        this.data = null;
        panel.SetActive(false);
    }

    private string BuildInfoShow()
    {
        if (templateInfo == null || templateInfo.Length == 0)
        {
            templateInfo = txtInfo.GetComponent<TextMeshProUGUI>().text;
            Debug.Log("Template LOAD  " + templateInfo);
        }

        ScriptableShip cData = data.curShipData;
        ScriptableShip pData = data.PeakData;

        string info = templateInfo
        .Replace("{model}", cData.typeName)
        .Replace("{cch}", cData.maxCrew.ToString())
        .Replace("{chh}", cData.hullHealth.ToString())
        .Replace("{csh}", cData.sailHealth.ToString())
        .Replace("{pch}", pData.maxCrew.ToString())
        .Replace("{phh}", pData.hullHealth.ToString())
        .Replace("{psh}", pData.sailHealth.ToString())
        .Replace("{oar}", cData.oarsSpeed.ToString())
        .Replace("{wheel}", cData.MaxDegreeRotate.ToString() + "°")
        .Replace("{wind}", cData.windConversionRate.ToString())
        .Replace("{c_cap}", cData.capacity.ToString())
        .Replace("{p_cap}", pData.capacity.ToString())
        .Replace("{goods}", cData.capacityWeightRate.ToString())
        .Replace("{deck}", cData.numberDeck.ToString())
        .Replace("{cannon}", cData.numberCannons.Sum().ToString())
        .Replace("{hull_mat}", data.hullMaterial?.itemName ?? "")
        .Replace("{sail_mat}", data.sailMaterial?.itemName ?? "")
        ;

        for (int i = 1; i <= 4; i++)
        {
            info = info.Replace("{skill" + i + "}",
            !CommonUtils.IsArrayNullEmpty(data.skills) && data.skills.Length > i ? data.skills[i].skillName : ""
            );
        }
        Debug.Log("Template Origin " + templateInfo);
        Debug.Log("Template Replace " + info);
        return info;
    }


}
