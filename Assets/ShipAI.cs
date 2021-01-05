using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAI : MonoBehaviour
{
    [System.Flags]
    public enum ShipIntent
    {
        None = 0, Travel = 1, Flee = 2, Shot = 4, Loot = 8
    }

    public ShipIntent CurIntent = ShipIntent.None;
    public ActionShipBehavior CurActionBehavior;
    public Ship target;
    private Ship ship;
    // Start is called before the first frame update
    void Start()
    {
        ship = GetComponent<Ship>();
        CurActionBehavior = new ActionShipBehavior();
        DetermineIntent();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(GameManager.instance.playerShip.transform.position, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        CurActionBehavior.Update(this);
    }

    public void DetermineIntent()
    {
        CurIntent = ShipIntent.Shot;
        ShotBehavior behavior = new ShotBehavior();
        target = GameManager.instance.playerShip.GetComponent<Ship>();
        behavior.target = target;
        CurActionBehavior = behavior;
    }



    public class ActionShipBehavior
    {
        public virtual void Update(ShipAI shipAI)
        {

        }

    }

    public class TravelBehavior : ActionShipBehavior
    {

    }

    public class FleeBehavior : ActionShipBehavior
    {

    }

    public class ShotBehavior : ActionShipBehavior
    {
        public Ship target;

        public ShotBehavior()
        {
        }

        public override void Update(ShipAI shipAI)
        {
            //calculate good position to fire
            Vector2 pos = FindPosToShot(shipAI);
            //command ship move to position
            shipAI.MoveTo(pos);
            //if target in range => command ship fire
            shipAI.FireIfInRange();
        }

        public Vector2 FindPosToShot(ShipAI shipAI)
        {
            Vector2 targetPos = target.transform.position;
            Vector2 shipPos = shipAI.ship.transform.position;
            Vector2 line = shipPos - targetPos;
            float rangeAttack = 5f;
            /*scenario 1
            check if distance between ship and target > range attack 
            then position on line between 2 ship and distance to target equal range attack
            */
            if (line.SqrMagnitude() > Mathf.Pow(rangeAttack, 2))
            {
                Debug.DrawLine(shipPos, targetPos + line.normalized * rangeAttack, Color.cyan, 3f);
                return targetPos + line.normalized * rangeAttack;
            }
            else
            {
                float rad2 = rangeAttack - line.magnitude + rangeAttack / 5f;
                return FindCircleCircleInsideIntersections(targetPos, rangeAttack, shipPos, rad2, shipAI.ship.ShipDirection);
            }
            /*scenario 2
            check if distance between ship and target <= range attack 
            then position is cross of circle target and range and circle ship and radius is speed of ship 
            */


        }

        public Vector2 FindCircleCircleInsideIntersections(Vector2 center1, float radius1, Vector2 center2, float radius2, Vector2 shipDirection)
        {
            Vector2 line = center2 - center1;
            float d = line.magnitude;
            float b = (Mathf.Pow(radius1, 2) - Mathf.Pow(radius2, 2) - Mathf.Pow(d, 2)) / (2 * d);
            float a = d + b;
            float h = Mathf.Sqrt(Mathf.Pow(radius1, 2) - Mathf.Pow(a, 2));

            Vector2 m = center1 + line.normalized * a;
            Vector2 p1 = m + (new Vector2(line.y, -line.x)).normalized * h;
            Vector2 p2 = m + (new Vector2(-line.y, line.x)).normalized * h;

            float angel = Vector2.Angle(line, shipDirection);
            if (angel == 0 || angel == 180)
            {
                Debug.Log("random point");
                Debug.DrawLine(center2, p1, Color.cyan, 3f);
                return p1;
            }
            else
            {
                Debug.Log(VectorUtils.IsRightSide(line, shipDirection) + " == " + VectorUtils.IsRightSide(line, p1 - center1));
                if (VectorUtils.IsRightSide(line, shipDirection) == VectorUtils.IsRightSide(line, p1 - center1))
                {
                    Debug.Log("point 1");
                    Debug.DrawLine(center2, p1, Color.cyan, 3f);
                    return p1;
                }
                else
                {
                    Debug.Log("point 2");
                    Debug.DrawLine(center2, p2, Color.cyan, 3f);
                    return p2;
                }
            }

            // Debug.DrawLine(center1, m, Color.cyan, 3f);
            // Debug.DrawLine(center2, p1, Color.cyan, 3f);
            // Debug.DrawLine(center2, p2, Color.cyan, 3f);
            // Debug.Log("move " + p1 + "/" + p2);


        }

    }

    private void FireIfInRange()
    {
        if (target != null)
        {
            Vector2 vTarget = ship.transform.position - target.transform.position;
            float distance = vTarget.sqrMagnitude;
            // Debug.Log("distance" + distance);
            if (distance <= 25f)
            {
                ship.FireTarget(target.transform.position);
            }
        }
    }

    private void MoveTo(Vector2 pos)
    {
        Vector2 move = pos - (Vector2)ship.transform.position;
        float angel = Vector2.Angle(ship.ShipDirection, move);
        // Debug.Log(angel);
        // Debug.DrawLine(ship.transform.position, ship.transform.position + (Vector3)move, Color.red, 3f);
        // Debug.DrawLine(ship.transform.position, ship.transform.position + (Vector3)ship.ShipDirection, Color.blue, 3f);
        if (angel > 1f || angel < -1f)
        {
            ship.CalculateRotateVector((VectorUtils.IsRightSide(ship.ShipDirection, move) ? -1 : 1) * ship.maxRotateDegree);
        }
        else
        {
            ship.CalculateRotateVector(0f);
        }
    }

    public class LootBehavior : ActionShipBehavior
    {

    }
}
