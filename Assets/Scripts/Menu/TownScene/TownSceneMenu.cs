using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownSceneMenu : MonoBehaviour
{
    public static TownSceneMenu Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PressShipManage()
    {
        ShipManageMenu.Instance.ShowManage();
    }
}
