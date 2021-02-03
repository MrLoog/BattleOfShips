using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHealthBar : MonoBehaviour
{
    public Ship shipOwner;
    public GameObject hullBar;
    public GameObject sailBar;
    public GameObject crewBar;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (shipOwner != null)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, -shipOwner.gameObject.transform.localRotation.eulerAngles.z);
            hullBar.transform.localScale = new Vector3(
                shipOwner.curShipData.hullHealth / (float)shipOwner.startShipData.hullHealth,
                hullBar.transform.localScale.y
            );
            sailBar.transform.localScale = new Vector3(
                shipOwner.curShipData.sailHealth / (float)shipOwner.startShipData.sailHealth,
                sailBar.transform.localScale.y
            );
            Debug.Log("Health Bar " + shipOwner.curShipData.maxCrew + " / " + shipOwner.startShipData.maxCrew);
            crewBar.transform.localScale = new Vector3(
                shipOwner.curShipData.maxCrew / (float)shipOwner.startShipData.maxCrew,
                crewBar.transform.localScale.y
            );
        }
    }
}
