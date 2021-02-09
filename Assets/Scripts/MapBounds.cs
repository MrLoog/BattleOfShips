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
            Vector2 pos = other.gameObject.transform.position;
            ship.transform.rotation *= Quaternion.Euler(0, 0, 180f);
            ship.transform.position = pos;
            ship.RevalidMovement();
            // GameManager.instance.RandomTeleport(ship.gameObject);
        }
    }
}
