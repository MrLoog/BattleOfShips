﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipSkillCtrl : MonoBehaviour
{
    public static ShipSkillCtrl Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

    }
    public Ship ship;
    public bool isSync = false;

    public Sprite[] avatarSkills = new Sprite[ShipHelper.MAX_SHIP_SKILL];
    public GameObject[] buttonSkill;
    public Image[] imgSKill;
    // Start is called before the first frame update
    void Start()
    {
        StartSync();
    }

    public void StartSync()
    {
        isSync = false;
        gameObject.SetActive(false);
        if (SeaBattleManager.Instance.playerShip != null)
        {
            ship = SeaBattleManager.Instance.playerShip.GetComponent<Ship>();
            isSync = true;
            if (ship.shipSkills == null || ship.shipSkills.Count == 0)
            {
                return;
            }
            gameObject.SetActive(true);
            for (int i = 0; i < ship.shipSkills.Count; i++)
            {
                avatarSkills[i] = ship.shipSkills[i].avatar;
                imgSKill[i].sprite = avatarSkills[i];
                buttonSkill[i].SetActive(true);
            }
            for (int i = ship.shipSkills.Count; i < ShipHelper.MAX_SHIP_SKILL; i++)
            {
                //if not full 4 skill, clear slot remain
                avatarSkills[i] = null;
                imgSKill[i].sprite = null;
                buttonSkill[i].SetActive(false);
            }
            gameObject.SetActive(true);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            ActiveSkill(0);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            ActiveSkill(1);
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            ActiveSkill(2);
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            ActiveSkill(3);
        }
    }

    public void ActiveSkill(int i)
    {
        ship.shipSkills[i].ToggleSkill();
    }
}