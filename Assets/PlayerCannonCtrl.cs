using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCannonCtrl : MonoBehaviour
{

    [System.Flags]
    public enum CannonDirection
    {
        None = 0, Front = 1, Right = 2, Left = 4, Back = 8
    }

    public CannonDirection Direction = CannonDirection.None;
    public static PlayerCannonCtrl _instance = new PlayerCannonCtrl();
    private PlayerCannonCtrl()
    {

    }

    public static PlayerCannonCtrl Instance()
    {
        return _instance;
    }

    public Sprite CannonDirectionImg;
    public Sprite CannonDirectionSelectedImg;
    public Button Front;
    public Button Right;
    public Button Left;
    public Button Back;

    public void ToggleFront()
    {
        ToggleDirection(CannonDirection.Front);
    }
    public void ToggleRight()
    {
        ToggleDirection(CannonDirection.Right);
    }

    public void ToggleLeft()
    {
        ToggleDirection(CannonDirection.Left);
    }

    public void ToggleBack()
    {
        ToggleDirection(CannonDirection.Back);
    }

    private void ToggleDirection(CannonDirection direction)
    {
        Direction ^= direction;
        Debug.Log(Direction);
        Button b = GetButtonByDirection(direction);
        if (b == null) return;
        b.GetComponent<Image>().sprite = ((Direction & direction) != 0) ? CannonDirectionSelectedImg : CannonDirectionImg;
    }

    private Button GetButtonByDirection(CannonDirection direction)
    {
        switch (direction)
        {
            case CannonDirection.Front: return Front;
            case CannonDirection.Right: return Right;
            case CannonDirection.Left: return Left;
            case CannonDirection.Back: return Back;
            default: return null;
        }
    }

    public void PressFire()
    {

    }
}
