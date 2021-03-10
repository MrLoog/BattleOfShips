using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Map bounds trigger" + other.tag);
        if (other.tag.Equals(GameSettings.TAG_SHIP))
        {
            Ship ship = other.GetComponent<Ship>();
            if (ship.IsPlayerShip())
            {
                GameManager.Instance.PauseGamePlay();
                GameManager.Instance.PopupCtrl.ShowDialog(
                    title: GameText.GetText(GameText.CONFIRM_RETURN_TOWN_RUN_TITLE),
                    content: GameText.GetText(GameText.CONFIRM_RETURN_TOWN_RUN_CONTENT),
                    okText: GameText.GetText(GameText.CONFIRM_COMMON_YES),
                    cancelText: GameText.GetText(GameText.CONFIRM_COMMON_NO),
                    onResult: (i) =>
                    {
                        GameManager.Instance.ResumeGamePlay();
                        if (i == ModalPopupCtrl.RESULT_POSITIVE)
                        {
                            SeaBattleManager.Instance.ReturnTown(true);
                        }
                        else
                        {
                            ChangeDirectionShip(ship);
                        }
                    }
                );
            }
            else
            {
                ChangeDirectionShip(ship);
            }
            // GameManager.instance.RandomTeleport(ship.gameObject);
        }
    }

    private void ChangeDirectionShip(Ship ship)
    {
        Vector2 pos = ship.gameObject.transform.position;
        ship.transform.rotation *= Quaternion.Euler(0, 0, 180f);
        ship.transform.position = pos;
        ship.RevalidMovement();
    }
}
