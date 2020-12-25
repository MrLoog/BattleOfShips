using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Ship : MonoBehaviour
{

    public float maxHealth;
    public float curHealth;
    public float speed;

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
    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

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
            Debug.Log(string.Format("{0}/{1}/{2}/{3}", transform.position, fire.normalized, col.size, direction));
            switch (direction)
            {
                case CannonDirection.Front:
                    // Debug.Log(string.Format("{0}/{1}/{2}", transform.position.x, to.normalized.x, col.size.x));
                    // Debug.Log(string.Format("{0}/{1}/{2}", transform.position.y, to.normalized.y, col.size.y));

                    fromList.Add(new Vector2(
                        transform.position.x + fire.normalized.x * col.size.x
                    , transform.position.y + fire.normalized.y * col.size.y / 2
                    ));
                    // Debug.Log(string.Format("{0}/{1},{2}", transform.position, to.normalized, col.size));
                    break;
                case CannonDirection.Back:
                    for (int i = 0; i < numberFires; i++)
                    {
                        float area2 = col.size.x / (2 * numberFires);
                        var pos = -((col.size.x / 2) - area2) + i * area2 * 2;
                        float angel = 90 - Vector2.Angle(new Vector2(pos, 0), new Vector2(pos, -col.size.y / 2));
                        angel = ((i + 1) > (0.5 + (numberFires / 2)) ? 1 : -1) * angel;
                        Debug.Log(angel);
                        Debug.Log(string.Format("{0}/{1}/{2}", fire.normalized, pos, transform.position.x + fire.normalized.x * pos));
                        // fromList.Add(new Vector2(
                        // transform.position.x + fire.normalized.x * pos
                        // , transform.position.y + fire.normalized.y * col.size.y / 2
                        // ));
                        Debug.DrawLine(transform.position, (Vector2)transform.position + Rotate(fire, angel, true), Color.blue, 3f);
                        fromList.Add((Vector2)transform.position + Rotate(fire, angel, true).normalized * col.size.y / 2);
                        // fromList.Add((Vector2)transform.position + fire.normalized + new Vector2(pos, col.size.y / 2));
                    }
                    break;
                default: break;

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

                    shot.Target = from + fire.normalized * 3f;
                    shot.speed = speed;
                    shot.OnImpactTarget = delegate ()
                    {
                        GameManager.instance.poolCannon.RePooledObject(wrapPool);
                    };
                    shot.gameObject.SetActive(true);
                    shot.StartTravel();

                    Debug.DrawLine(from, shot.Target, Color.red, 3f);
                }
            }


        }
    }


}
