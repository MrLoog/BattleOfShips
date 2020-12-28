using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Ship : MonoBehaviour
{

    public float maxHealth;
    public float curHealth;
    public float speed;

    public Vector2 windForce;
    public Vector2 shipVelocity;

    public float rotateSpeed;
    public Vector2 rotateDirection;

    public int[] numberCannon; //front,right,left,back
    public CannonDirection FireDirections; //front,right,left,back
    public Sprite normalState;
    public Sprite damagedState;
    public Sprite dangerState;
    public Sprite deathState;
    public List<GameObject> Fires = new List<GameObject>();

    public GameObject Fire;
    public GameObject HumanFly;
    public SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody2d;

    private void Awake()
    {
        curHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody2d = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MakeShipRotate();
    }

    public void ApplyDamage(float damage, GameObject source)
    {

        SpawnHumanFly(source);
        float healthRateB = curHealth / maxHealth;
        float healthRateA = (curHealth - damage) / maxHealth;

        if (healthRateA <= 0f)
        {
            spriteRenderer.sprite = deathState;
            ClearFire();
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
        else if (healthRateA >= 1f)
        {
            spriteRenderer.sprite = normalState;
            ClearFire();
        }

        curHealth -= damage;
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
        // this.windForce = windForce;
        this.windForce = GetFrontDirection().normalized * 0.5f;
        CalculateMove();
    }

    public Vector2 CalculateRotateVector(float degreeRotate)
    {
        float absDegree = Mathf.Abs(degreeRotate);
        Vector2 vShip = shipVelocity;
        float forceRotate = Mathf.Sin(absDegree) * Mathf.Cos(absDegree) * vShip.magnitude;
        forceRotate = Mathf.Abs(forceRotate);
        // Debug.Log(string.Format("calculate1 {0}/{1}/{2}/{3}", Mathf.Sin(absDegree), Mathf.Cos(absDegree), vShip.magnitude, forceRotate));
        // Debug.Log(string.Format("calculate {0}/{1}/{2}/{3}", degreeRotate, absDegree, shipVelocity, forceRotate));
        Vector2 vRotate = forceRotate * VectorUtils.Rotate(shipVelocity, (degreeRotate > 0 ? 1 : -1) * 90f, true).normalized;
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
        // Debug.Log("force" + forceRotate);
        return vRotate;
    }

    public void MakeShipRotate()
    {
        if (rotateSpeed > 0)
        {
            float deltaAngel = 180 * Time.deltaTime / rotateSpeed;
            deltaAngel = deltaAngel * (VectorUtils.IsRightSide(shipVelocity, rotateDirection) ? -1 : 1);
            transform.localRotation *= Quaternion.Euler(0, 0, deltaAngel);
            shipVelocity = VectorUtils.Rotate(shipVelocity, deltaAngel);
            rigidBody2d.velocity = shipVelocity;
            // Debug.Log(deltaAngel);
            // Debug.DrawLine(transform.position, transform.position + (Vector3)rigidBody2d.velocity, Color.blue, 1f);
        }
    }

    private void CalculateMove()
    {
        rigidBody2d.velocity = windForce;
        shipVelocity = windForce;
    }

    public void ClearFire()
    {
        foreach (GameObject go in Fires)
        {
            Destroy(go);
        }
        Fires.Clear();
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
