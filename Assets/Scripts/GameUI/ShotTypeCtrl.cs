using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SeaBattleManager;
using static UnityEngine.UI.Dropdown;

public class ShotTypeCtrl : MonoBehaviour
{
    public Ship ship;
    public bool isSync = false;
    public Dropdown CannonDirectionSelect;
    public Dropdown CannonTypeSelect;

    private bool skipReflectChange = false;
    // Start is called before the first frame update
    void Start()
    {
    }


    public void StartSync()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        isSync = false;
        Debug.Log("Shot Type Ctrl Enable");
        if (SeaBattleManager.Instance.playerShip != null)
        {
            ship = SeaBattleManager.Instance.playerShip.GetComponent<Ship>();

            //sync logic
            CannonTypeSelect.ClearOptions();
            List<OptionData> options = new List<OptionData>();
            if (!CommonUtils.IsArrayNullEmpty(ship.avaiableShotType))
            {
                for (int i = 0; i < ship.avaiableShotType.Length; i++)
                {
                    Debug.Log("Shot Type Ctrl Add " + ship.avaiableShotType[i]);
                    options.Add(new OptionData(ship.avaiableShotType[i]));
                }
            }
            CannonTypeSelect.AddOptions(options);

            ShowCurrent();

            ship.Events.RegisterListener(Ship.EVENT_SHOT_TYPE_CHANGED).AddListener(ShowCurrent);
            isSync = true;
        }
    }

    private void OnDisable()
    {
        if (ship != null)
        {
            ship.Events.RegisterListener(Ship.EVENT_SHOT_TYPE_CHANGED).RemoveListener(ShowCurrent);
        }
    }

    public void ShowCurrent()
    {
        if (CommonUtils.IsArrayNullEmpty(ship.avaiableShotType)) return;
        int indexDirection = CannonDirectionSelect.value;
        string typeShot = ship.shotTypeCode[indexDirection];
        for (int i = 0; i < CannonTypeSelect.options.Count; i++)
        {
            if (CannonTypeSelect.options[i].text == typeShot)
            {
                Debug.Log("Shot Type Ctrl ShowCurrent");
                if (CannonTypeSelect.value != i)
                {
                    skipReflectChange = true;
                    CannonTypeSelect.value = i;
                }
                break;
            }
        }
    }

    public void ChangeShotType()
    {
        Debug.Log("Shot Type Ctrl ChangeShotType");
        if (skipReflectChange)
        {
            Debug.Log("Shot Type Ctrl skipReflectChange");
            //avoid set by ShowCurrent
            skipReflectChange = false;
            return;
        }
        int indexDirection = CannonDirectionSelect.value;
        string typeShot = CannonTypeSelect.options[CannonTypeSelect.value].text;
        bool result = false;
        Debug.Log("Shot Type Ctrl " + indexDirection + "/" + typeShot);
        switch (indexDirection)
        {
            case 0:
                result = ship.ChangeShotType(CannonDirection.Front, typeShot);
                break;
            case 1:
                result = ship.ChangeShotType(CannonDirection.Right, typeShot);
                break;
            case 2:
                result = ship.ChangeShotType(CannonDirection.Left, typeShot);
                break;
            case 3:
                result = ship.ChangeShotType(CannonDirection.Back, typeShot);
                break;
            default: break;
        }
    }
}
