using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ShipManageMiniMenu : MonoBehaviour
{
    // , IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
    public GameObject btnMakeMain;
    public GameObject btnSell;
    public GameObject btnRepair;
    public GameObject btnTransfer;
    public GameObject btnMarket;
    public GameObject btnUpgrade;

    public ShipManageInfo focusShip;

    public GameObject panelBounds;
    public GameObject actualPanelMenu;

    public UnityAction OnAutoClose;

    private Transform originParent;

    private void Awake()
    {
        originParent = transform.parent;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckInputInsideMenu();
        UpdatePosition();
    }

    private void FixedUpdate()
    {

    }

    private void CheckInputInsideMenu()
    {
        if (focusShip != null)
        {
            if (gameObject.activeSelf && !RectTransformUtility.RectangleContainsScreenPoint(
            panelBounds.GetComponent<RectTransform>(),
            actualPanelMenu.transform.position
        ))
            {
                OnAutoClose?.Invoke();
                CloseMiniMenu();
            }
        }
    }


    private bool mouseIsOver = false;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Close the Window on Deselect only if a click occurred outside this panel
        if (!mouseIsOver)
            CloseMiniMenu();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseIsOver = true;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseIsOver = false;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void ValidFeatureAvaiable()
    {
        // btnMakeMain.SetActive(false);
        // btnSell.SetActive(false);
        // btnRepair.SetActive(false);
        // btnTransfer.SetActive(false);
        // btnHireCrew.SetActive(false);
        // btnUpgrade.SetActive(false);
        if (focusShip == null) return;
        btnMakeMain.SetActive(!focusShip.IsMainShip);
        btnSell.SetActive(!focusShip.IsMainShip
        && ShipHelper.IsCanSell(focusShip.data)
        );
        ScriptableShipCustom data = focusShip.data;
        btnRepair.SetActive(
            ShipHelper.IsNeedRepair(focusShip.data.curShipData, focusShip.data.PeakData)
        );
        btnTransfer.SetActive(true);
        btnMarket.SetActive(true);
        // btnUpgrade.SetActive(true);
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
            UpdatePosition();
            // gameObject.transform.SetParent(focusShip.transform, false);
            // gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            // transform.position = focusShip.transform.position;
            ValidFeatureAvaiable();
            gameObject.SetActive(true);
        }
    }

    private void UpdatePosition()
    {
        if (focusShip != null)
        {
            Vector3 relativeTarget = focusShip.btnFunc.transform.InverseTransformPoint(transform.position);
            Vector2 prePos = transform.position;
            if (relativeTarget.x != 0 || relativeTarget.y != 0)
            {
                transform.position -= relativeTarget;
            }
            Debug.Log("Update Position " + relativeTarget + "/" + prePos + "/" + transform.position);
            // transform.position = focusShip.transform.position;
        }
    }

    public void CloseMiniMenu()
    {
        gameObject.SetActive(false);
        // transform.SetParent(originParent, false);
        focusShip = null;
    }
}
