using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CloseCombatDetech : MonoBehaviour
{
    public GameObject attackSign;
    public GameObject transferSign;
    private CapsuleCollider2D capCol;
    public Ship owner;

    List<Ship> ships = new List<Ship>();
    public float range = 1f;
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
        owner.Events.RegisterListener(Ship.EVENT_CREW_50).AddListener(delegate
        {
            GameManager.Instance.ToastService.ShowMessage(
                GameText.GetText(GameText.TOAST_CLOSE_COMBAT_AVAIABLE),
                1f
            );
            attackSign.SetActive(true);
        });
        owner.Events.RegisterListener(Ship.EVENT_SHIP_DEFEATED).AddListener(delegate
        {
            GameManager.Instance.ToastService.ShowMessage(
                GameText.GetText(GameText.TOAST_CLOSE_COMBAT_AVAIABLE),
                1f
            );
            attackSign.SetActive(true);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CommandCloseCombat()
    {
        Debug.Log("Conduct Close Combat Press");
        if (ships.FirstOrDefault(x => x.IsPlayerShip()) != null)
        {
            SeaBattleManager.Instance.CommandTackedOtherShip(owner);
        }
        else
        {
            GameManager.Instance.ToastService.ShowMessage(
                GameText.GetText(GameText.TOAST_CLOSE_COMBAT_OUT_RANGE),
                1f
            );
        }
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
                if (ships.Count == 0 || ships.Where(x => x.IsSameGroup(owner)).FirstOrDefault() == null)
                    transferSign.SetActive(false);
            }
        }
    }
}
