using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;
    public float speed;
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

    public void ApplyDamage(float damage)
    {

        SpawnHumanFly();
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

    private Vector2 LastCannonImpact;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == GameSettings.TAG_CANNON)
        {
            LastCannonImpact = other.gameObject.transform.position;
        }
    }

    public void SpawnHumanFly()
    {
        GameObject humanfly = Instantiate(HumanFly, null, false);
        Vector2 direction = (Vector2)transform.position - LastCannonImpact;
        humanfly.GetComponentInChildren<HumanFly>().StartAnimate(direction);
    }

    public void ClearFire()
    {
        foreach (GameObject go in Fires)
        {
            Destroy(go);
        }
        Fires.Clear();
    }
}
