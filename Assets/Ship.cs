using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static GameManager;

public class Ship : MonoBehaviour
{
    private EventDict _events;
    public EventDict Events
    {
        get
        {
            if (_events == null) _events = new EventDict();
            return _events;
        }
    }

    public const string EVENT_CANNON_FRONT_FIRE = "CANNON_FRONT_FIRE";
    public const string EVENT_CANNON_FRONT_READY = "CANNON_FRONT_READY";
    public const string EVENT_CANNON_RIGHT_FIRE = "CANNON_RIGHT_FIRE";
    public const string EVENT_CANNON_RIGHT_READY = "CANNON_RIGHT_READY";
    public const string EVENT_CANNON_LEFT_FIRE = "CANNON_LEFT_FIRE";
    public const string EVENT_CANNON_LEFT_READY = "CANNON_LEFT_READY";
    public const string EVENT_CANNON_BACK_FIRE = "CANNON_BACK_FIRE";
    public const string EVENT_CANNON_BACK_READY = "CANNON_BACK_READY";
    public bool ForceStop = false;
    public float maxHealth;
    public float curHealth;
    public float speed = 0.2f;
    public float windForceValue = 0f;
    public float actualSpeed = 0f;

    public float maxRotateDegree = 45f;
    public UnityEvent OnChangeSailDirection = new UnityEvent();

    public Vector2 wind;
    public Vector2 Wind
    {
        set
        {
            wind = value;
            windForceValue = value.magnitude;
        }
        get
        {
            return wind;
        }
    }
    public Vector2 sailDirection = Vector2.left;
    public Vector2 SailDirecion
    {
        get
        {
            return sailDirection;
        }
        set
        {
            sailDirection = value;
            if (OnChangeSailDirection != null)
            {
                OnChangeSailDirection.Invoke();
            }
            CalculateMove();
        }
    }

    private Vector2 shipDirection = Vector2.down;
    public Vector2 ShipDirection
    {
        get
        {
            return VectorUtils.Rotate(Vector2.down, transform.localRotation.eulerAngles.z, true);
        }
    }
    public Vector2 shipVelocity;
    public Vector2 ShipVelocity
    {
        set
        {
            shipVelocity = value;
            actualSpeed = shipVelocity.magnitude;
        }
        get
        {
            return shipVelocity;
        }
    }

    public float rotateSpeed;
    public Vector2 rotateDirection;


    public int[] numberCannon; //front,right,left,back
    public CannonDirection FireDirections; //front,right,left,back

    public float[] cooldownCannons;
    public float[] timeReloadCannons;

    public Sprite normalState;
    public Sprite damagedState;
    public Sprite dangerState;
    public Sprite deathState;
    public List<GameObject> Fires = new List<GameObject>();

    public GameObject Fire;
    public GameObject HumanFly;
    public SpriteRenderer spriteRenderer;

    public bool IsDeath => curHealth <= 0;
    private Rigidbody2D rigidBody2d;

    private void Awake()
    {
        curHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody2d = GetComponent<Rigidbody2D>();
        cooldownCannons = new float[4] { 0f, 0f, 0f, 0f };
        timeReloadCannons = new float[4] { 3f, 3f, 3f, 3f };
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MakeShipRotate();
        ReloadCannons();
        // Debug.DrawLine(transform.position, transform.position + (Vector3)(VectorUtils.Rotate(Wind, 180, true)) * 3f, Color.red, 3f);
        // Debug.DrawLine(transform.position, transform.position + (Vector3)(ShipDirection) * 3f, Color.green, 3f);
        // Debug.DrawLine(transform.position, transform.position + (Vector3)SailDirecion.normalized * 1.5f, Color.yellow, 3f);
        // Debug.DrawLine(transform.position, transform.position + (Vector3)(VectorUtils.Rotate(SailDirecion, 180, true)).normalized * 1.5f, Color.yellow, 3f);

        // Debug.DrawLine(transform.position, transform.position + (Vector3)(ShipVelocity - ShipDirection.normalized * speed) * 3f, Color.blue, 3f);

    }

    private void ReloadCannons()
    {
        for (int i = 0; i < timeReloadCannons.Length; i++)
        {
            if (cooldownCannons[i] < timeReloadCannons[i])
            {
                cooldownCannons[i] += Time.deltaTime;
                if (cooldownCannons[i] >= timeReloadCannons[i])
                {
                    switch (i)
                    {
                        case 0: Events.InvokeOnAction(EVENT_CANNON_FRONT_READY); break;
                        case 1: Events.InvokeOnAction(EVENT_CANNON_RIGHT_READY); break;
                        case 2: Events.InvokeOnAction(EVENT_CANNON_LEFT_READY); break;
                        case 3: Events.InvokeOnAction(EVENT_CANNON_BACK_READY); break;
                        default: break;
                    }
                }
            }
        }
    }

    private bool CheckCannonReady(CannonDirection direction, bool IsFire = false)
    {
        int i = -1;
        switch (direction)
        {
            case CannonDirection.Front:
                i = 0;
                break;
            case CannonDirection.Right:
                i = 1;
                break;
            case CannonDirection.Left:
                i = 2;
                break;
            case CannonDirection.Back:
                i = 3;
                break;
            default: return false;
        }
        if (cooldownCannons[i] >= timeReloadCannons[i])
        {
            if (IsFire)
            {
                switch (i)
                {
                    case 0: Events.InvokeOnAction(EVENT_CANNON_FRONT_FIRE); break;
                    case 1: Events.InvokeOnAction(EVENT_CANNON_RIGHT_FIRE); break;
                    case 2: Events.InvokeOnAction(EVENT_CANNON_LEFT_FIRE); break;
                    case 3: Events.InvokeOnAction(EVENT_CANNON_BACK_FIRE); break;
                    default: break;
                }
                cooldownCannons[i] = 0f;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ApplyDamage(float damage, GameObject source)
    {

        SpawnHumanFly(source);
        float healthRateB = curHealth / maxHealth;
        float healthRateA = (curHealth - damage) / maxHealth;

        if (healthRateA <= 0f)
        {
            spriteRenderer.sprite = deathState;
            MakeDeathShip();
        }
        else if ((healthRateB > 0.2f) && (healthRateA <= 0.2f))
        {
            spriteRenderer.sprite = dangerState;
            SpawnRandomFire();
        }
        else if ((healthRateB > 0.5f) && (healthRateA <= 0.5f))
        {
            SpawnRandomFire();
        }
        else if ((healthRateB > 0.7f) && (healthRateA <= 0.7f))
        {
            spriteRenderer.sprite = damagedState;
            SpawnRandomFire();
        }
        else if (healthRateA > 0.7f)
        {
            RestoreState();
        }

        curHealth -= damage;
    }

    public void RestoreState()
    {
        spriteRenderer.sprite = normalState;
        ClearFire();
        ShipAI AI = GetComponent<ShipAI>();
        if (AI != null)
        {
            AI.enabled = true;
        }
        ApplyWindForce(GameManager.instance.windForce);
    }

    public void MakeDeathShip()
    {
        ClearFire();
        ShipAI AI = GetComponent<ShipAI>();
        ApplyWindForce(Vector2.zero);
        rigidBody2d.velocity = Vector2.zero;
        if (AI != null)
        {
            AI.enabled = false;
        }
    }

    public void SpawnRandomFire()
    {
        GameObject fire = Instantiate(Fire, transform, false);
        CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
        fire.transform.localPosition = new Vector2(
            Random.Range(-col.size.x / 2, col.size.x / 2),
            Random.Range(-col.size.y / 2, col.size.y / 2)
        );
        Fires.Add(fire);
    }



    public void SpawnHumanFly(GameObject source)
    {
        if (source == null) return;
        GameObject humanfly = Instantiate(HumanFly, null, true);
        Vector2 direction = transform.position - source.transform.position;
        Debug.Log("before direction " + direction);
        direction = Rotate(direction, Random.Range(-45f, 45f));
        Debug.Log("after direction " + direction);
        humanfly.transform.position = new Vector2(
            transform.position.x + direction.x,
            transform.position.y + direction.y
        );
        humanfly.GetComponentInChildren<HumanFly>().StartAnimate(direction);
    }


    internal Vector2 GetFrontDirection()
    {
        return Rotate(Vector2.down, gameObject.transform.rotation.eulerAngles.z, true);
    }

    public static Vector2 Rotate(Vector2 v, float degrees, bool isNew = false)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        if (isNew)
        {
            return new Vector2((cos * tx) - (sin * ty)
            , (sin * tx) + (cos * ty)
            );
        }
        else
        {
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

    }

    public static List<Vector2> RotateByDirection(Vector2 frontV, CannonDirection direction)
    {
        List<Vector2> results = new List<Vector2>();
        Vector2 sampleV = new Vector2(frontV.x, frontV.y);
        if (((direction & CannonDirection.Front) != 0))
        {
            sampleV = new Vector2(frontV.x, frontV.y);
            results.Add(Rotate(sampleV, 0));
        }
        if (((direction & CannonDirection.Left) != 0))
        {
            sampleV = new Vector2(frontV.x, frontV.y);
            results.Add(Rotate(sampleV, 90));
        }
        if (((direction & CannonDirection.Right) != 0))
        {
            sampleV = new Vector2(frontV.x, frontV.y);
            results.Add(Rotate(sampleV, -90));
        }
        if (((direction & CannonDirection.Back) != 0))
        {
            sampleV = new Vector2(frontV.x, frontV.y);
            results.Add(Rotate(sampleV, 180));
        }
        return results;
    }

    internal void ApplyWindForce(Vector2 windForce)
    {
        if (IsDeath) return;
        Wind = windForce;
        // this.windForce = Vector2.zero;
        // this.windForce = ShipDirection.normalized * 2f;
        CalculateSailPos();
        CalculateMove();
    }

    public void RevalidMovement()
    {
        CalculateSailPos();
        CalculateMove();
    }

    public void CalculateRotateVector(float degreeRotate)
    {
        if (degreeRotate != 0f)
        {
            float absDegree = Mathf.Abs(degreeRotate);
            Vector2 vShip = ShipVelocity;
            float forceRotate = Mathf.Sin(absDegree) * Mathf.Cos(absDegree) * vShip.magnitude;
            forceRotate = Mathf.Abs(forceRotate);
            // Debug.Log(string.Format("calculate1 {0}/{1}/{2}/{3}", Mathf.Sin(absDegree), Mathf.Cos(absDegree), vShip.magnitude, forceRotate));
            // Debug.Log(string.Format("calculate {0}/{1}/{2}/{3}", degreeRotate, absDegree, shipVelocity, forceRotate));
            Vector2 vRotate = forceRotate * VectorUtils.Rotate(ShipVelocity, (degreeRotate > 0 ? 1 : -1) * 90f, true).normalized;
            rotateDirection = vRotate;
            CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
            float c = Mathf.PI * col.size.y / 2; // 1/2 perimeter
            if (forceRotate > 0)
            {
                rotateSpeed = c / vRotate.magnitude;
            }
            else
            {
                rotateSpeed = 0;
            }
        }
        else
        {
            rotateSpeed = 0;
        }
    }

    public void CalculateSailPos()
    {
        float angelShipWind = Vector2.Angle(Wind, ShipDirection);
        angelShipWind = 180 - Mathf.Abs(angelShipWind); //small degree
        float angelSail = angelShipWind / 2;

        if (VectorUtils.IsRightSide(ShipDirection, VectorUtils.Rotate(Wind, 180, true)))
        {
            angelSail = -angelSail;
        }
        else
        {
            angelSail = -(180 - angelSail);

        }
        // Debug.Log("angelSail " + angelSail);
        // angelSail *= VectorUtils.IsRightSide(ShipDirection, VectorUtils.Rotate(Wind, 180, true)) ? 1 : -1;
        SailDirecion = VectorUtils.Rotate(ShipDirection, angelSail, true).normalized;
    }

    public void MakeShipRotate()
    {
        if (rotateSpeed > 0)
        {
            float deltaAngel = 180 * Time.deltaTime / rotateSpeed;
            deltaAngel = deltaAngel * (VectorUtils.IsRightSide(ShipVelocity, rotateDirection) ? -1 : 1);
            transform.localRotation *= Quaternion.Euler(0, 0, deltaAngel);
            ShipVelocity = VectorUtils.Rotate(ShipVelocity, deltaAngel);
            rigidBody2d.velocity = ShipVelocity;
            CalculateSailPos();
            // Debug.Log(deltaAngel);
            // Debug.DrawLine(transform.position, transform.position + (Vector3)rigidBody2d.velocity, Color.blue, 1f);
        }
    }

    public void RotateSail(float degreeRotate)
    {
        SailDirecion = VectorUtils.Rotate(Vector2.left, degreeRotate);
    }

    private void CalculateMove()
    {
        if (ForceStop) return;
        Vector2 vSail = VectorUtils.GetForceOnLine(SailDirecion, Wind, false);
        Vector2 vShip = VectorUtils.GetForceOnLine(ShipDirection, vSail);
        // Debug.Log(string.Format("calculate move {0}/{1}/{2}", Wind, vSail, vShip));
        vShip = vShip + ShipDirection.normalized * speed;
        if (!VectorUtils.IsSameDirection(ShipDirection, vShip))
        {
            vShip = Vector2.zero;
        }
        rigidBody2d.velocity = vShip;
        ShipVelocity = vShip;
    }

    public void ClearFire()
    {
        foreach (GameObject go in Fires)
        {
            Destroy(go);
        }
        Fires.Clear();
    }

    public void FireTarget(Vector2 target)
    {
        Vector2 toTarget = target - (Vector2)transform.position;
        float range = 7f;
        Vector2 flag = VectorUtils.Rotate(ShipDirection, 45, true);
        float angel = Vector2.Angle(toTarget, flag);

        /*
        Vector2 crossDir = VectorUtils.Rotate(ShipDirection, -90, true).normalized;
        CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
        Vector2 pA = (Vector2)transform.position + ShipDirection.normalized * col.size.y / 2
        + crossDir * col.size.x / 2;
        Vector2 pB = (Vector2)transform.position + ShipDirection.normalized * col.size.y / 2
        + new Vector2(-crossDir.x, -crossDir.y) * col.size.x / 2;
        Vector2 pC = (Vector2)transform.position + new Vector2(-ShipDirection.x, -ShipDirection.y).normalized * col.size.y / 2
        + new Vector2(-crossDir.x, -crossDir.y) * col.size.x / 2;
        Vector2 pD = (Vector2)transform.position + new Vector2(-ShipDirection.x, -ShipDirection.y).normalized * col.size.y / 2
         + crossDir * col.size.x / 2;

        if (!CheckCannonReady(CannonDirection.Front))
        {
            Debug.DrawLine(pA, pA + ShipDirection.normalized * range, Color.green, 1f);
            Debug.DrawLine(pB, pB + ShipDirection.normalized * range, Color.green, 1f);
            if (VectorUtils.IsPointInRectangle(target,
            pA, pB, pB + ShipDirection.normalized * range, pA + ShipDirection.normalized * range
            ))
            {
                Debug.Log("fire 1");
                FireCannonOneDirection(CannonDirection.Front);
            }
        }
        if (CheckCannonReady(CannonDirection.Left))
        {
            Debug.DrawLine(transform.position, transform.position + (Vector3)VectorUtils.Reverse(crossDir).normalized, Color.red, 1f);
            Debug.DrawLine(transform.position, target, Color.red, 1f);
            float area = VectorUtils.AreaTriangle(transform.position, transform.position + (Vector3)VectorUtils.Reverse(crossDir).normalized, target);
            Debug.Log("test area " + area);

            if (area == 0)
            {
                FireCannonOneDirection(CannonDirection.Right);
            }
            // Debug.DrawLine(pB, pB + VectorUtils.Reverse(crossDir).normalized * range, Color.green, 1f);
            // Debug.DrawLine(pC, pC + VectorUtils.Reverse(crossDir).normalized * range, Color.green, 1f);
            // if (VectorUtils.IsPointInRectangle(target,
            // pB, pC, pC + VectorUtils.Reverse(crossDir).normalized * range, pB + VectorUtils.Reverse(crossDir).normalized * range
            // ))
            // {
            //     Debug.Log("fire 2");
            //     FireCannonOneDirection(CannonDirection.Left);
            // }
        }
        RaycastHit hit;
        if (CheckCannonReady(CannonDirection.Right))
        {
            Debug.DrawRay(transform.position, (Vector3)crossDir.normalized * range, Color.green);
            if (Physics.Raycast(transform.position, (Vector3)crossDir.normalized * range, out hit))
            {
                Debug.Log("hit " + hit);
                FireCannonOneDirection(CannonDirection.Right);
            }

            // Debug.DrawLine(pA, pA + crossDir.normalized * range, Color.green, 1f);
            // Debug.DrawLine(pD, pD + crossDir.normalized * range, Color.green, 1f);

            // if (VectorUtils.IsPointInRectangle(target,
            // pA, pD, pD + crossDir.normalized * range, pA + crossDir.normalized * range
            // ))
            // {
            //     Debug.Log("fire 3");
            //     FireCannonOneDirection(CannonDirection.Right);
            // }
        }

        if (!CheckCannonReady(CannonDirection.Back))
        {
            Debug.DrawLine(pC, pC + VectorUtils.Reverse(ShipDirection).normalized * range, Color.green, 1f);
            Debug.DrawLine(pD, pD + VectorUtils.Reverse(ShipDirection).normalized * range, Color.green, 1f);
            if (VectorUtils.IsPointInRectangle(target,
            pC, pD, pD + VectorUtils.Reverse(ShipDirection).normalized * range, pC + VectorUtils.Reverse(ShipDirection).normalized * range
            ))
            {
                Debug.Log("fire 4");
                FireCannonOneDirection(CannonDirection.Back);
            }
        }
        */

        Debug.Log(angel);
        if (VectorUtils.IsRightSide(flag, toTarget))
        {
            if (angel >= 40 && angel <= 50)
            {
                Debug.Log("fire1 " + angel);
                FireCannonOneDirection(CannonDirection.Front);
            }
            else if (angel >= 130 && angel <= 140)
            {
                FireCannonOneDirection(CannonDirection.Right);
            }
        }
        else
        {
            Debug.Log("fire2 " + angel);
            if (angel >= 40 && angel <= 50)
            {
                FireCannonOneDirection(CannonDirection.Left);
            }
            else if (angel >= 130 && angel <= 140)
            {
                FireCannonOneDirection(CannonDirection.Back);
            }
        }
    }

    public void FireCannon(CannonDirection direction)
    {
        FireDirections = direction;

        if (((direction & CannonDirection.Front) != 0))
        {
            FireCannonOneDirection(CannonDirection.Front);
        }
        if (((direction & CannonDirection.Left) != 0))
        {
            FireCannonOneDirection(CannonDirection.Left);
        }
        if (((direction & CannonDirection.Right) != 0))
        {
            FireCannonOneDirection(CannonDirection.Right);
        }
        if (((direction & CannonDirection.Back) != 0))
        {
            FireCannonOneDirection(CannonDirection.Back);
        }


    }

    public int NumberCannonDirection(CannonDirection direction)
    {
        switch (direction)
        {
            case CannonDirection.Front: return numberCannon.Length > 0 ? numberCannon[0] : 0;
            case CannonDirection.Right: return numberCannon.Length > 1 ? numberCannon[1] : 0;
            case CannonDirection.Left: return numberCannon.Length > 2 ? numberCannon[2] : 0;
            case CannonDirection.Back: return numberCannon.Length > 3 ? numberCannon[3] : 0;
            default: return 0;
        }
    }
    private void FireCannonOneDirection(CannonDirection direction)
    {
        if (!CheckCannonReady(direction, true)) return;
        Vector2 to = Rotate(Vector2.down, transform.rotation.eulerAngles.z); //current front
        List<Vector2> fireDirections = RotateByDirection(to, direction);
        foreach (Vector2 fire in fireDirections)
        {
            int numberFires = NumberCannonDirection(direction);
            CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
            List<Vector2> fromList = new List<Vector2>();

            float areaFireFace = 0;
            float areaOtherFace = 0;
            // Debug.Log(string.Format("{0}/{1}/{2}/{3}", transform.position, fire.normalized, col.size, direction));
            switch (direction)
            {
                case CannonDirection.Front:
                case CannonDirection.Back:
                    areaFireFace = col.size.x;
                    areaOtherFace = col.size.y;
                    break;
                case CannonDirection.Left:
                case CannonDirection.Right:
                    areaFireFace = col.size.y;
                    areaOtherFace = col.size.x;
                    break;
                default:
                    break;
            }
            for (int i = 0; i < numberFires; i++)
            {
                float area2 = areaFireFace / (2 * numberFires);
                var pos = -((areaFireFace / 2) - area2) + i * area2 * 2;
                float angel = Vector2.Angle(new Vector2(-areaOtherFace / 2, 0), new Vector2(-areaOtherFace / 2, -pos));
                angel = ((i + 1) > (0.5 + (numberFires / 2)) ? 1 : -1) * angel;
                // Debug.DrawLine(transform.position, (Vector2)transform.position + Rotate(fire, angel, true), Color.blue, 3f);
                fromList.Add((Vector2)transform.position + Rotate(fire, angel, true).normalized * areaOtherFace / 2);
            }
            foreach (Vector2 from in fromList)
            {
                WrapPool wrapPool = GameManager.instance.poolCannon.GetPooledObject();
                if (wrapPool != null)
                {
                    GameObject actualCannon = wrapPool.cannonBall;
                    CannonShot shot = actualCannon.GetComponent<CannonShot>();
                    shot.ResetTravel();
                    actualCannon.transform.position = from;
                    shot.owner = gameObject;
                    shot.fireDirection = fire.normalized;
                    // shot.Target = from + fire.normalized * 3f;
                    // shot.speed = speed;
                    shot.OnImpactTarget = delegate ()
                    {
                        GameManager.instance.poolCannon.RePooledObject(wrapPool);
                    };
                    shot.gameObject.SetActive(true);
                    shot.StartTravel();

                    // Debug.DrawLine(from, shot.Target, Color.red, 3f);
                }
            }


        }
    }


}
