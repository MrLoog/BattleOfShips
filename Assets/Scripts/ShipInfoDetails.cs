using System.Collections;
using System.Collections.Generic;
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
        }
        return templateInfo.Replace("{model}", data.curShipData.typeName);
    }
}
