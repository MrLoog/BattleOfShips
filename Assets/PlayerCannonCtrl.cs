using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class PlayerCannonCtrl : MonoBehaviour
{


    public CannonDirection Direction = CannonDirection.None;
    public static PlayerCannonCtrl Instance { get; private set; }

    private Quaternion StartRotation;
    private Vector2 PlayerShipDirection;


    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartRotation = gameObject.transform.localRotation;
        PlayerShipDirection = GameManager.instance.GetPlayerShipFrontDirection();
        UpdateByPlayer();
    }

    void Update()
    {
        if (GameManager.instance.GetPlayerShipFrontDirection() != PlayerShipDirection)
        {
            PlayerShipDirection = GameManager.instance.GetPlayerShipFrontDirection();
            UpdateByPlayer();
        }
    }

    public void UpdateByPlayer()
    {
        Vector2 realCtrlDirection = VectorUtils.Rotate(Vector2.up, -StartRotation.eulerAngles.z, true);
        float angel = Vector2.Angle(realCtrlDirection, PlayerShipDirection);
        Vector3 cross = Vector3.Cross(realCtrlDirection, PlayerShipDirection);
        int sign = cross.z > 0 ? 1 : -1;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, sign * angel);
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
        if (Direction != CannonDirection.None)
        {
            GameManager.instance.PlayerFireCannon(Direction);
        }
    }
}
