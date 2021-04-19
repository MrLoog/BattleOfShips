using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SeaBattleManager;

public class ShipAI : MonoBehaviour
{
    public enum MoveBehavior
    {
        Travel, KeepDistance
    }

    public enum AttackBehavior
    {
        Cannon
    }
    public const string EVENT_CHANGE_ATTACK_TARGET = "CHANGE_ATTACK_TARGET";
    public const string EVENT_CHANGE_ATTACK_TARGET_GROUP = "CHANGE_ATTACK_TARGET_GROUP";
    public const string MEMORY_KEY_TARGET_ATTACK = "TARGET_ATTACK";
    public const string MEMORY_KEY_TARGET_ATTACK_GROUP = "TARGET_ATTACK_GROUP";
    public const string MEMORY_KEY_TARGET_MOVE = "TARGET_MOVE";
    public const string MEMORY_KEY_FIRE_BEHAVIOR = "FIRE_BEHAVIOR";
    public const string MEMORY_KEY_MOVE_BEHAVIOR = "MOVE_BEHAVIOR";
    public const string MEMORY_KEY_SIZE_X = "SIZE_X";
    public const string MEMORY_KEY_SIZE_Y = "SIZE_Y";
    public ShipMoveBehavior moveBehavior;
    public ShipFireBehavior fireBehavior;
    public ShipSkillBehavior[] skillBehavior;
    public Ship target;
    private Ship ship;

    private Dictionary<string, object> memoryAI;
    public Dictionary<string, object> MemoryAI
    {
        get
        {
            return memoryAI;
        }
    }
    private EventDict _events;
    public EventDict Events
    {
        get
        {
            if (_events == null) _events = new EventDict();
            return _events;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        Debug.Log("Ship AI onenable");
        ship = GetComponent<Ship>();
        if (memoryAI == null)
        {
            Debug.Log("Ship AI memory null");
            if (ship.customData?.AIMemory != null)
            {
                Debug.Log("Ship AI memory restore");
                memoryAI = ship.customData?.AIMemory;
                RestoreFromMemory();
            }
            else
            {
                Debug.Log("Ship AI memory new");
                memoryAI = new Dictionary<string, object>();
                DetermineBehavior();
            }
        }
    }

    private void RestoreFromMemory()
    {
        Debug.Log("Shoudl Restore memory");
        if (fireBehavior == null && memoryAI.ContainsKey(MEMORY_KEY_FIRE_BEHAVIOR))
        {
            ChangeFireBehavior(CommonUtils.CastToEnum<AttackBehavior>(memoryAI[MEMORY_KEY_FIRE_BEHAVIOR]));
        }
        if (moveBehavior == null && memoryAI.ContainsKey(MEMORY_KEY_MOVE_BEHAVIOR))
        {
            ChangeMoveBehavior(CommonUtils.CastToEnum<MoveBehavior>(memoryAI[MEMORY_KEY_MOVE_BEHAVIOR]));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(SeaBattleManager.Instance.playerShip.transform.position, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        moveBehavior?.Update();
        fireBehavior?.Update();
        if (!CommonUtils.IsArrayNullEmpty(skillBehavior))
        {
            for (int i = 0; i < skillBehavior.Length; i++)
            {
                skillBehavior[i].Update();
            }
        }
    }

    void FixedUpdate()
    {
    }

    public bool Memoriez(string key, object value)
    {
        if (memoryAI == null) return false;
        if (memoryAI.ContainsKey(key))
        {
            memoryAI[key] = value;
        }
        else
        {
            memoryAI.Add(key, value);
        }
        return true;
    }


    private void DetermineBehavior()
    {
        if (fireBehavior == null)
        {
            ChangeFireBehavior(AttackBehavior.Cannon);
        }
        if (moveBehavior == null)
        {
            ChangeMoveBehavior(MoveBehavior.KeepDistance);
        }
    }

    private void ChangeFireBehavior(AttackBehavior behavior)
    {
        if (!behavior.Equals(fireBehavior))
        {
            Debug.Log("Shoudl Restore memory ChangeFireBehavior");
            fireBehavior?.Disable();
            Memoriez(MEMORY_KEY_FIRE_BEHAVIOR, (int)behavior);
            fireBehavior = new ShotCannonBehavior(this);
            fireBehavior.Enable();
        }
    }

    private void ChangeMoveBehavior(MoveBehavior behavior)
    {
        if (!behavior.Equals(moveBehavior))
        {
            Debug.Log("Shoudl Restore memory ChangeMoveBehavior");
            moveBehavior?.Disable();
            Memoriez(MEMORY_KEY_MOVE_BEHAVIOR, (int)behavior);
            switch (behavior)
            {
                case MoveBehavior.KeepDistance:
                    moveBehavior = new KeepDistanceBehavior(this);
                    break;
                default: break;
            }
            moveBehavior?.Enable();
        }
    }

    public void CommandAttack(string shipBattleId)
    {
        Memoriez(MEMORY_KEY_TARGET_ATTACK, shipBattleId);
        Events.InvokeOnAction(EVENT_CHANGE_ATTACK_TARGET);
    }
    public void CommandAttackGroup(string group)
    {
        Memoriez(MEMORY_KEY_TARGET_ATTACK_GROUP, group);
        Events.InvokeOnAction(EVENT_CHANGE_ATTACK_TARGET_GROUP);
    }

    public void CommandMove(Vector2 pos)
    {
        Memoriez(MEMORY_KEY_TARGET_MOVE, pos.x + "," + pos.y);
        Vector2 move = pos - (Vector2)ship.transform.position;
        float angel = Vector2.Angle(ship.ShipDirection, move);
        // Debug.Log(angel);
        // Debug.DrawLine(ship.transform.position, ship.transform.position + (Vector3)move, Color.red, 3f);
        // Debug.DrawLine(ship.transform.position, ship.transform.position + (Vector3)ship.ShipDirection, Color.blue, 3f);
        if (angel > 1f || angel < -1f)
        {
            ship.CalculateRotateVector((VectorUtils.IsRightSide(ship.ShipDirection, move) ? -1 : 1) * ship.curShipData.MaxDegreeRotate);
        }
        else
        {
            ship.CalculateRotateVector(0f);
        }
    }
    public void KeepPosition()
    {
        if (ship.sailSet != 0f)
        {
            ship.SetSail(0f);
        }
    }

    public void FullMove()
    {
        if (ship.sailSet != 1f)
            ship.SetSail(1f);
    }

    public void Fire(CannonDirection direction)
    {
        ship.FireCannon(direction);
    }

    private float FireIfInRange()
    {
        if (target != null)
        {
            Vector2 vTarget = ship.transform.position - target.transform.position;
            float distance = vTarget.sqrMagnitude;
            // Debug.Log("distance" + distance);
            if (distance <= 25f)
            {
                return ship.FireTarget(target.transform.position);
            }
            return 0f;
        }
        return 0f;
    }

    public abstract class ShipActionBehavior
    {
        public ShipAI AIControl;
        public ShipActionBehavior(ShipAI ai)
        {
            this.AIControl = ai;
        }
        public abstract void Update();
        public abstract void Enable();
        public abstract void Disable();
    }

    public abstract class ShipMoveBehavior : ShipActionBehavior
    {
        public ShipMoveBehavior(ShipAI ai) : base(ai)
        {

        }
        public override abstract void Update();
        public override abstract void Enable();
        public override abstract void Disable();
    }

    public abstract class ShipFireBehavior : ShipActionBehavior
    {
        public ShipFireBehavior(ShipAI ai) : base(ai)
        {

        }
        public override abstract void Update();
        public override abstract void Enable();
        public override abstract void Disable();
    }

    public abstract class ShipSkillBehavior : ShipActionBehavior
    {
        public ShipSkillBehavior(ShipAI ai) : base(ai)
        {

        }
        public override abstract void Update();
        public override abstract void Enable();
        public override abstract void Disable();
    }

    public class KeepDistanceBehavior : ShipMoveBehavior
    {
        private float delayFindPos = 1f;
        private float accumFindPos = 0f;

        private Ship target;

        public KeepDistanceBehavior(ShipAI ai) : base(ai)
        {

        }
        public override void Disable()
        {
            AIControl.Events.RegisterListener(ShipAI.EVENT_CHANGE_ATTACK_TARGET).RemoveListener(FocusTarget);
        }

        public override void Enable()
        {
            AIControl.Events.RegisterListener(ShipAI.EVENT_CHANGE_ATTACK_TARGET).AddListener(FocusTarget);
            FocusTarget();
            Debug.Log("KeepDistanceBehavior enable " + target?.BattleId);
        }

        private void FocusTarget()
        {
            if (AIControl.MemoryAI.ContainsKey(ShipAI.MEMORY_KEY_TARGET_ATTACK))
            {
                target = SeaBattleManager.Instance.FindShipByBattleId(AIControl.MemoryAI[ShipAI.MEMORY_KEY_TARGET_ATTACK].ToString());
                Debug.Log("KeepDistanceBehavior focustarger " + target?.BattleId ?? "empty");
            }
        }

        public override void Update()
        {
            if (target == null) return;
            accumFindPos += Time.deltaTime;
            Debug.Log("KeepDistanceBehavior update ");
            if (accumFindPos > delayFindPos)
            {
                //calculate good position to fire
                Vector2 pos = FindPosToShot();
                Debug.Log("KeepDistanceBehavior update " + pos);
                //command ship move to position
                AIControl.CommandMove(pos);
                accumFindPos = 0f;
            }
        }

        public Vector2 FindPosToShot()
        {
            Vector2 targetPos = target.transform.position;
            Vector2 shipPos = AIControl.ship.transform.position;
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
                return FindCircleCircleInsideIntersections(targetPos, rangeAttack, shipPos, rad2, AIControl.ship.ShipDirection);
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
                // Debug.Log("random point");
                Debug.DrawLine(center2, p1, Color.cyan, 3f);
                return p1;
            }
            else
            {
                // Debug.Log(VectorUtils.IsRightSide(line, shipDirection) + " == " + VectorUtils.IsRightSide(line, p1 - center1));
                if (VectorUtils.IsRightSide(line, shipDirection) == VectorUtils.IsRightSide(line, p1 - center1))
                {
                    // Debug.Log("point 1");
                    Debug.DrawLine(center2, p1, Color.cyan, 3f);
                    return p1;
                }
                else
                {
                    // Debug.Log("point 2");
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

    public class ShotCannonBehavior : ShipFireBehavior
    {
        private float accumWaitTimeShot = 0f;
        private float waitTime = 0f;

        private float[] cannonRanges;
        private float maxRange = -1f;
        private List<Ship> ships = new List<Ship>();

        private float ShipSizeX = 0f;
        private float ShipSizeY = 0f;

        public ShotCannonBehavior(ShipAI ai) : base(ai)
        {
        }

        public override void Update()
        {
            if (ships == null || ships.Count == 0) return;
            CannonDirection fire = CannonDirection.None;
            for (int i = 0; i < ships.Count; i++)
            {
                fire = fire | CanShotTarget(ships[i]);
            }
            if (!CannonDirection.None.Equals(fire))
            {
                AIControl.Fire(fire);
            }
        }

        private CannonDirection CanShotTarget(Ship shipTarget)
        {
            Vector2 myDirection = AIControl.ship.ShipDirection;
            Vector2 target = shipTarget.transform.position;
            Vector2 shipPos = AIControl.ship.transform.position;

            Vector2 toTarget = (Vector2)shipTarget.transform.position - (Vector2)AIControl.ship.transform.position;

            if (CommonUtils.IsArrayNullEmpty(cannonRanges)) return CannonDirection.None;
            if (Mathf.Pow(maxRange, 2) < toTarget.sqrMagnitude) return CannonDirection.None;

            Vector2 flag = VectorUtils.Rotate(myDirection, 45, true);
            float angel = Vector2.Angle(toTarget, flag);

            Vector2 crossDir = VectorUtils.Rotate(myDirection, -90, true).normalized;

            Vector2 pA = (Vector2)shipPos + myDirection.normalized * ShipSizeY / 2
            + crossDir * ShipSizeY / 2;
            Vector2 pB = (Vector2)shipPos + myDirection.normalized * ShipSizeY / 2
            + new Vector2(-crossDir.x, -crossDir.y) * ShipSizeY / 2;
            Vector2 pC = (Vector2)shipPos + new Vector2(-myDirection.x, -myDirection.y).normalized * ShipSizeY / 2
            + new Vector2(-crossDir.x, -crossDir.y) * ShipSizeY / 2;
            Vector2 pD = (Vector2)shipPos + new Vector2(-myDirection.x, -myDirection.y).normalized * ShipSizeY / 2
             + crossDir * ShipSizeY / 2;

            float range = cannonRanges[0];
            if (range > 0 && VectorUtils.IsPointInRectangle(target,
                pA, pB, pB + myDirection.normalized * range, pA + myDirection.normalized * range
                ))
            {
                return CannonDirection.Front;
            }


            range = cannonRanges[1];
            if (range > 0 && VectorUtils.IsPointInRectangle(target,
                pA, pD, pD + crossDir.normalized * range, pA + crossDir.normalized * range
                ))
            {
                return CannonDirection.Right;
            }

            range = cannonRanges[2];
            if (range > 0 && VectorUtils.IsPointInRectangle(target,
                pB, pC, pC + VectorUtils.Reverse(crossDir).normalized * range, pB + VectorUtils.Reverse(crossDir).normalized * range
                ))
            {
                return CannonDirection.Left;
            }

            range = cannonRanges[3];
            if (range > 0 && VectorUtils.IsPointInRectangle(target,
                pC, pD, pD + VectorUtils.Reverse(myDirection).normalized * range, pC + VectorUtils.Reverse(myDirection).normalized * range
                ))
            {
                return CannonDirection.Back;
            }

            return CannonDirection.None;
        }

        public void FixedUpdate()
        {

        }

        public override void Enable()
        {
            if (ShipSizeX == 0)
            {
                if (AIControl.MemoryAI.ContainsKey(ShipAI.MEMORY_KEY_SIZE_X))
                {
                    ShipSizeX = Convert.ToSingle(AIControl.MemoryAI[ShipAI.MEMORY_KEY_SIZE_X]);
                }
                else
                {
                    ShipSizeX = AIControl.ship.ActualSizeX;
                    AIControl.Memoriez(ShipAI.MEMORY_KEY_SIZE_X, ShipSizeX);
                }
            }
            if (ShipSizeY == 0)
            {
                if (AIControl.MemoryAI.ContainsKey(ShipAI.MEMORY_KEY_SIZE_Y))
                {
                    ShipSizeY = Convert.ToSingle(AIControl.MemoryAI[ShipAI.MEMORY_KEY_SIZE_Y]);
                }
                else
                {
                    ShipSizeY = AIControl.ship.ActualSizeY;
                    AIControl.Memoriez(ShipAI.MEMORY_KEY_SIZE_Y, ShipSizeY);
                }
            }
            LoadRangeCannon();
            AIControl.ship.Events.RegisterListener(Ship.EVENT_SHOT_TYPE_CHANGED).AddListener(LoadRangeCannon);


            AIControl.Events.RegisterListener(EVENT_CHANGE_ATTACK_TARGET).AddListener(FocusTarget);
            FocusTarget();

        }

        private void FocusTarget()
        {
            if (AIControl.MemoryAI.ContainsKey(ShipAI.MEMORY_KEY_TARGET_ATTACK))
            {
                ships.Clear();
                Ship target = SeaBattleManager.Instance.FindShipByBattleId(AIControl.MemoryAI[ShipAI.MEMORY_KEY_TARGET_ATTACK].ToString());
                if (target != null && ships.Where(x => x.BattleId == target.BattleId).FirstOrDefault() == null)
                {
                    ships.Add(target);
                }
            }
        }
        public void LoadRangeCannon()
        {
            cannonRanges = new float[4];
            for (int i = 0; i < AIControl.ship.shotTypeCode.Length; i++)
            {
                cannonRanges[i] = ShipHelper.GetRangeCannonType(AIControl.ship.shotTypeCode[i]);
            }
            maxRange = cannonRanges.Max();
        }

        public override void Disable()
        {
            AIControl.ship.Events.RegisterListener(Ship.EVENT_SHOT_TYPE_CHANGED).RemoveListener(LoadRangeCannon);
            AIControl.Events.RegisterListener(EVENT_CHANGE_ATTACK_TARGET).RemoveListener(FocusTarget);
        }
    }
}
