using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CloseCombatDetech : MonoBehaviour
{
    public GameObject attackSign;
    public GameObject transferSign;
    public Text closeCombatInfo;
    private CapsuleCollider2D capCol;
    public Ship owner;

    List<Ship> ships = new List<Ship>();
    public float range = 2f;
    public float maxPercentSuccess = 100f;
    public float minPercentSuccess = 5f;
    public float maxRateHealthRate = 0.3f;

    public bool AttackShow = false;

    public float cooldown = 10f;
    public float resetCooldown = 10f;

    // Start is called before the first frame update
    void Start()
    {
        if (owner == null || owner.IsPlayerShip())
        {
            gameObject.SetActive(false);
            return;
        }

        capCol = GetComponent<CapsuleCollider2D>();
        range = SeaBattleManager.Instance.rangeCloseConduct;
        capCol.size = new Vector2(capCol.size.x + range * 2, capCol.size.y + range * 2);

        owner.Events.RegisterListener(Ship.EVENT_CREW_DAMAGED).AddListener(ShowCombatInfo);
        owner.Events.RegisterListener(Ship.EVENT_TACKED_OTHER_SHIP).AddListener(delegate
        {
            Ship target = owner.LastCollision2D.gameObject.GetComponent<Ship>();
            if (AttackShow && target.IsPlayerShip())
            {
                CommandCloseCombat();
            }
        });

        // owner.Events.RegisterListener(Ship.EVENT_SHIP_DEFEATED).AddListener(delegate
        // {
        //     GameManager.Instance.ToastService.ShowMessage(
        //         GameText.GetText(GameText.TOAST_CLOSE_COMBAT_AVAIABLE),
        //         1f
        //     );
        //     attackSign.SetActive(true);
        // });
    }

    // Update is called once per frame
    void Update()
    {
        bool updateAttackInfo = resetCooldown < cooldown;
        if (updateAttackInfo) resetCooldown += Time.deltaTime;
        if (AttackShow && !IsCanCloseCombat())
        {
            attackSign.SetActive(false);
            AttackShow = false;
        }
        else if (!AttackShow && IsCanCloseCombat())
        {
            attackSign.SetActive(true);
            AttackShow = true;
            ShowCombatInfo();
        }
        if (AttackShow && updateAttackInfo)
        {
            ShowCombatInfo();
        }
    }

    private void ShowCombatInfo()
    {
        if (AttackShow)
        {
            if (resetCooldown < cooldown)
            {
                closeCombatInfo.text = ((int)resetCooldown).ToString();
                return;
            }
            else
            {
                closeCombatInfo.text = ((int)CalculateSuccessRate()).ToString() + "%";
                return;
            }
        }
    }
    private bool IsCanCloseCombat()
    {
        if (ships.Count > 0) //only player ship
        {
            Ship player = ships.FirstOrDefault();
            if (!owner.IsDefeated && player.curShipData.maxCrew < owner.curShipData.maxCrew)
            {
                //sure lose not show
                Debug.Log("Close combat cant combat sure lost");
                return false;
            }
            Debug.Log("Close combat can combat");
            return true;
        }
        Debug.Log("Close combat cant combat");
        return false;
    }

    public void CommandCloseCombat()
    {
        Debug.Log("Conduct Close Combat Press");
        if (resetCooldown < cooldown) return;
        if (ships.FirstOrDefault(x => x.IsPlayerShip()) != null)
        {
            if (IsSuccessConduct(CalculateSuccessRate()))
            {
                SeaBattleManager.Instance.CommandTackedOtherShip(owner);
            }
            else
            {
                GameManager.Instance.ToastService.ShowMessage(
                    GameText.GetText(GameText.TOAST_CLOSE_COMBAT_FAILED),
                    1f
                );
            }
            resetCooldown = 0f;
        }
        else
        {
            GameManager.Instance.ToastService.ShowMessage(
                GameText.GetText(GameText.TOAST_CLOSE_COMBAT_OUT_RANGE),
                1f
            );
        }
    }

    private float CalculateSuccessRate()
    {
        if (owner.IsDefeated) return 100f;
        float curRate = (float)owner.curShipData.maxCrew / (float)owner.StartShipData.maxCrew;
        float process = (1 - curRate) / (1 - maxRateHealthRate);
        if (process > 1) process = 1;
        float successRate = minPercentSuccess + ((maxPercentSuccess - minPercentSuccess) * process);
        Debug.Log("Close combat success rate " + successRate);
        return successRate;
    }

    private bool IsSuccessConduct(float rate)
    {
        Debug.Log("Close combat perform conduct " + rate);
        return rate >= Random.Range(0f, 100f);
    }

    public void CommandCargoTrasnfer()
    {
        Debug.Log("Conduct Trasnfer Cargo Press");
        SeaBattleManager.Instance.CommandTackedOtherShip(owner);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals(GameSettings.TAG_SHIP))
        {
            Ship s = other.GetComponent<Ship>();
            if (s.IsPlayerShip())
            {
                ships.Add(s);
                Debug.Log("Close combat trigger enter");
                if (s.IsSameGroup(owner))
                {
                    transferSign.SetActive(true);
                }

            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals(GameSettings.TAG_SHIP))
        {
            Ship s = other.GetComponent<Ship>();
            if (s.IsPlayerShip())
            {
                ships.Remove(s);
                Debug.Log("Close combat trigger exit");
                if (ships.Count == 0 || ships.Where(x => x.IsSameGroup(owner)).FirstOrDefault() == null)
                    transferSign.SetActive(false);
            }
        }
    }
}
