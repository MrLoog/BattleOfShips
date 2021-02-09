using System.Collections;
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

    public Sprite[] avatarSkills = new Sprite[3];
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
        if (SeaBattleManager.Instance.playerShip != null)
        {
            ship = SeaBattleManager.Instance.playerShip.GetComponent<Ship>();
            for (int i = 0; i < ship.shipSkills.Count; i++)
            {
                avatarSkills[i] = ship.shipSkills[i].avatar;
                imgSKill[i].sprite = avatarSkills[i];
                buttonSkill[i].SetActive(true);
            }
            for (int i = ship.shipSkills.Count; i < 3; i++)
            {
                avatarSkills[i] = null;
                imgSKill[i].sprite = null;
                buttonSkill[i].SetActive(false);
            }
            isSync = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActiveSkill(int i)
    {
        ship.shipSkills[i].ToggleSkill();
    }
}
