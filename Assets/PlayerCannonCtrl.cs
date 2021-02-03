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

    public Ship playerShip;
    public bool isSync = false;

    public Toggle toggleCannonSight;


    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
        StartRotation = gameObject.transform.localRotation;
    }

    void Start()
    {
        StartSync();
    }
    public void StartSync()
    {
        isSync = false;
        if (GameManager.instance.playerShip != null)
        {
            playerShip = GameManager.instance.playerShip.GetComponent<Ship>();
            PlayerShipDirection = GameManager.instance.GetPlayerShipFrontDirection();
            UpdateByPlayer();
            playerShip.Events.RegisterListener(Ship.EVENT_CANNON_FRONT_FIRE).AddListener(delegate ()
            {
                ShowCooldownDirection(CannonDirection.Front);
            });
            playerShip.Events.RegisterListener(Ship.EVENT_CANNON_FRONT_READY).AddListener(delegate ()
            {
                ShowReadyDirection(CannonDirection.Front);
            });
            playerShip.Events.RegisterListener(Ship.EVENT_CANNON_RIGHT_FIRE).AddListener(delegate ()
            {
                ShowCooldownDirection(CannonDirection.Right);
            });
            playerShip.Events.RegisterListener(Ship.EVENT_CANNON_RIGHT_READY).AddListener(delegate ()
            {
                ShowReadyDirection(CannonDirection.Right);
            });
            playerShip.Events.RegisterListener(Ship.EVENT_CANNON_LEFT_FIRE).AddListener(delegate ()
            {
                ShowCooldownDirection(CannonDirection.Left);
            });
            playerShip.Events.RegisterListener(Ship.EVENT_CANNON_LEFT_READY).AddListener(delegate ()
            {
                ShowReadyDirection(CannonDirection.Left);
            });
            playerShip.Events.RegisterListener(Ship.EVENT_CANNON_BACK_FIRE).AddListener(delegate ()
            {
                ShowCooldownDirection(CannonDirection.Back);
            });
            playerShip.Events.RegisterListener(Ship.EVENT_CANNON_BACK_READY).AddListener(delegate ()
            {
                ShowReadyDirection(CannonDirection.Back);
            });
            isSync = true;
        }
    }

    void Update()
    {
        if (!isSync) return;
        if (GameManager.instance.playerShip == null) isSync = false;
        if (GameManager.instance.GetPlayerShipFrontDirection() != PlayerShipDirection)
        {
            PlayerShipDirection = GameManager.instance.GetPlayerShipFrontDirection();
            UpdateByPlayer();
        }

        if (Input.GetKeyDown(KeyCode.A)) ToggleLeft();
        if (Input.GetKeyDown(KeyCode.W)) ToggleFront();
        if (Input.GetKeyDown(KeyCode.S)) ToggleBack();
        if (Input.GetKeyDown(KeyCode.D)) ToggleRight();
    }

    public void UpdateByPlayer()
    {
        // Vector2 realCtrlDirection = VectorUtils.Rotate(Vector2.up, -StartRotation.eulerAngles.z, true);
        Vector2 realCtrlDirection = Vector2.up;
        float angel = Vector2.Angle(realCtrlDirection, PlayerShipDirection);
        Vector3 cross = Vector3.Cross(realCtrlDirection, PlayerShipDirection);
        int sign = cross.z > 0 ? 1 : -1;
        gameObject.transform.localRotation = Quaternion.Euler(0, 0, sign * angel + StartRotation.eulerAngles.z);
    }

    public Sprite CannonDirectionImg;
    public Sprite CannonDirectionSelectedImg;
    public Button Front;
    public Button Right;
    public Button Left;
    public Button Back;

    public void ToggleFront()
    {
        playerShip.FireCannon(CannonDirection.Front);
        // ToggleDirection(CannonDirection.Front);
    }
    public void ToggleRight()
    {
        playerShip.FireCannon(CannonDirection.Right);
        // ToggleDirection(CannonDirection.Right);
    }

    public void ToggleLeft()
    {
        playerShip.FireCannon(CannonDirection.Left);
        // ToggleDirection(CannonDirection.Left);
    }

    public void ToggleBack()
    {
        playerShip.FireCannon(CannonDirection.Back);
        // ToggleDirection(CannonDirection.Back);
    }

    private void ToggleDirection(CannonDirection direction)
    {
        Direction ^= direction;
        Button b = GetButtonByDirection(direction);
        if (b == null) return;
        b.GetComponent<Image>().sprite = ((Direction & direction) != 0) ? CannonDirectionSelectedImg : CannonDirectionImg;
    }

    private void ShowReadyDirection(CannonDirection direction)
    {
        Button b = GetButtonByDirection(direction);
        if (b == null) return;
        b.GetComponent<Image>().sprite = CannonDirectionSelectedImg;
    }

    private void ShowCooldownDirection(CannonDirection direction)
    {
        Button b = GetButtonByDirection(direction);
        if (b == null) return;
        b.GetComponent<Image>().sprite = CannonDirectionImg;
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
        CannonDirection direction = CannonDirection.Front | CannonDirection.Right | CannonDirection.Left | CannonDirection.Back;

        playerShip.FireCannon(direction);
    }


    public void ToggleCannonSight()
    {
        playerShip.EnableCannonSight = toggleCannonSight.isOn;
    }
}
