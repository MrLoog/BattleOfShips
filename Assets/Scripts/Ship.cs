using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static SeaBattleManager;

public class Ship : MonoBehaviour
{

    public int shipId;
    private EventDict _events;
    public EventDict Events
    {
        get
        {
            if (_events == null) _events = new EventDict();
            return _events;
        }
    }

    public ShipInventory Inventory => customData?.inventory;

    public const string EVENT_CANNON_FRONT_FIRE = "CANNON_FRONT_FIRE";
    public const string EVENT_CANNON_FRONT_READY = "CANNON_FRONT_READY";
    public const string EVENT_CANNON_RIGHT_FIRE = "CANNON_RIGHT_FIRE";
    public const string EVENT_CANNON_RIGHT_READY = "CANNON_RIGHT_READY";
    public const string EVENT_CANNON_LEFT_FIRE = "CANNON_LEFT_FIRE";
    public const string EVENT_CANNON_LEFT_READY = "CANNON_LEFT_READY";
    public const string EVENT_CANNON_BACK_FIRE = "CANNON_BACK_FIRE";
    public const string EVENT_CANNON_BACK_READY = "CANNON_BACK_READY";
    public const string EVENT_TACKED_OTHER_SHIP = "TACKED_OTHER_SHIP";
    public bool ForceStop = false;
    public float windForceValue = 0f;
    public float fixedSpeed = 0f;  // 0 mean not apply
    public float actualSpeed = 0f;

    public CapsuleCollider2D ShipCollider => GetComponent<CapsuleCollider2D>();
    // public float ActualSizeX => ShipCollider.size.x * curShipData.sizeRateWidth;
    // public float ActualSizeY => ShipCollider.size.y * curShipData.sizeRateLength;

    public float ActualSizeX => ShipCollider.size.x;
    public float ActualSizeY => ShipCollider.size.y;


    public List<ShipSkill> shipSkills;

    private List<ScriptableShipSkill> skillDatas;

    public ScriptableShipCustom customData;

    public ScriptableShipCustom CustomData
    {
        get
        {
            return BuildCustomData();
            // return customData;
        }
    }
    public ScriptableShip shipData; //origin data
    public ScriptableShip startShipData; //permanent data

    public ScriptableShip StartShipData
    {
        get
        {
            return startShipData;
        }
        set
        {
            startShipData = value.Clone<ScriptableShip>();
            curShipData = value.Clone<ScriptableShip>();
            ShipCollider.size = new Vector2(curShipData.sizeRateWidth * ShipCollider.size.x / model.transform.localScale.x
            , curShipData.sizeRateLength * ShipCollider.size.y / model.transform.localScale.y);

            model.transform.localScale = new Vector3(curShipData.sizeRateWidth, curShipData.sizeRateLength, 0);

            numberCannon = curShipData.numberCannons;
            cooldownCannons = new float[numberCannon.Length];
            timeReloadCannons = new float[numberCannon.Length];
            for (int i = 0; i < timeReloadCannons.Length; i++)
            {
                cooldownCannons[i] = 0f;
                timeReloadCannons[i] = 3f;
            }
        }
    }

    public ScriptableShip curShipData; //data change on battle

    public ScriptableShip ShipData
    {
        get
        {
            return shipData;
        }
        set
        {
            shipData = value.Clone<ScriptableShip>();
        }
    }


    public bool IsSameShip(Ship ship)
    {
        return shipId == ship.shipId;
    }

    public bool IsSameGroup(Ship ship)
    {
        return Group == ship.Group;
    }


    internal bool IsPlayerShip()
    {
        return shipId == 0;
    }




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

    public SpriteRenderer minimap;
    private int group;
    public int Group
    {
        get
        {
            return group;
        }
        set
        {
            group = value;
            minimap.color = value == 0 ? //0 mean player group
            Color.green :
            Color.red;

            // SetSpriteMinimap(value == SeaBattleManager.Instance.playerShip?.GetComponent<Ship>().Group ?
            // SeaBattleManager.Instance.MinimapAllySprite :
            // SeaBattleManager.Instance.MinimapEnemySprite);
        }
    }

    [SerializeField]
    public bool AutoSail { get; set; } = true;
    public float sailSet = 1f;

    internal void SetSail(float value)
    {
        sailSet = value;
        RevalidMovement();
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

    public bool isStuned = false;
    public float accumStunTime = 0f;

    public float lastTimeHit = 0f;
    public int totalHitConsecutive = 0;


    public ScriptableStateShip imgStateShip;

    public ScriptableStateShip ImgStateShip
    {
        get
        {
            return imgStateShip;
        }
        set
        {
            imgStateShip = value;
            switch (stateShip)
            {
                case StateShip.Normal: spriteRenderer.sprite = imgStateShip.normalState; break;
                case StateShip.Damaged: spriteRenderer.sprite = imgStateShip.damagedState; break;
                case StateShip.Danger: spriteRenderer.sprite = imgStateShip.dangerState; break;
                case StateShip.Death: spriteRenderer.sprite = imgStateShip.deathState; break;
                default: spriteRenderer.sprite = imgStateShip.normalState; break;
            }
        }
    }

    public enum StateShip
    {
        Normal, Damaged, Danger, Death
    }

    public StateShip stateShip = StateShip.Normal;
    public List<GameObject> Fires = new List<GameObject>();

    public GameObject Fire;
    public GameObject HumanFly;

    public GameObject model;
    public GameObject prefabCannonSight;
    public GameObject cannonSight;
    public SpriteRenderer spriteRenderer;

    public bool IsDeath => curShipData.hullHealth <= 0;


    private bool enableCannonSight;
    public bool EnableCannonSight
    {
        get
        {
            return enableCannonSight;
        }
        set
        {
            if (value)
            {
                if (cannonSight == null)
                {
                    cannonSight = Instantiate(prefabCannonSight, transform, false);
                    CannonSight[] sights = cannonSight.GetComponentsInChildren<CannonSight>();
                    foreach (CannonSight s in sights)
                    {
                        s.shipOwner = this;
                    }
                }
                else
                {
                    cannonSight.SetActive(true);
                }
            }
            else
            {
                cannonSight?.SetActive(false);
            }
            enableCannonSight = value;
        }
    }

    private Rigidbody2D rigidBody2d;

    public GameObject stunEffect;

    private void Awake()
    {
        spriteRenderer = model.GetComponent<SpriteRenderer>();
        rigidBody2d = GetComponent<Rigidbody2D>();
        if (customData != null)
        {
            InitFromCustomData(customData);
        }
        else if (ShipData != null)
        {
            ShipData = ShipData; //clone start data
        }
        BuildCustomData(); //init custom data if empty


        ShipHealthBar healthBar = GetComponentInChildren<ShipHealthBar>();
        if (healthBar != null)
            healthBar.shipOwner = this;
        // cooldownCannons = new float[4] { 0f, 0f, 0f, 0f };
        // timeReloadCannons = new float[4] { 3f, 3f, 3f, 3f };
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lastTimeHit += Time.deltaTime;
        if (isStuned)
        {
            accumStunTime += Time.deltaTime;
            if (accumStunTime >= curShipData.TimeRegainStun)
            {
                // Debug.Log("stun exit stun");
                stunEffect?.SetActive(false);
                isStuned = false;
                accumStunTime = 0;
            }
        }
        MakeShipRotate();
        ReloadCannons();
        // Debug.DrawLine(transform.position, transform.position + (Vector3)(VectorUtils.Rotate(Wind, 180, true)) * 3f, Color.red, 3f);
        // Debug.DrawLine(transform.position, transform.position + (Vector3)(ShipDirection) * 3f, Color.green, 3f);
        // Debug.DrawLine(transform.position, transform.position + (Vector3)SailDirecion.normalized * 1.5f, Color.yellow, 3f);
        // Debug.DrawLine(transform.position, transform.position + (Vector3)(VectorUtils.Rotate(SailDirecion, 180, true)).normalized * 1.5f, Color.yellow, 3f);

        // Debug.DrawLine(transform.position, transform.position + (Vector3)(ShipVelocity - ShipDirection.normalized * speed) * 3f, Color.blue, 3f);

    }

    void FixedUpdate()
    {
    }

    public int GetCannonBallInShip()
    {
        if (Inventory == null || Inventory.goodsCode == null) return 0;
        for (int i = 0; i < Inventory.goodsCode.Length; i++)
        {
            if (Inventory.goodsCode[i] == SeaBattleManager.Instance.GoodsCannonBallCode)
            {
                return Inventory.quantity[i];
            }
        }
        return 0;
    }

    public void DeductCannonBall(int quantity = 1)
    {
        if (Inventory == null || Inventory.goodsCode == null) return;
        for (int i = 0; i < Inventory.goodsCode.Length; i++)
        {
            if (Inventory.goodsCode[i] == SeaBattleManager.Instance.GoodsCannonBallCode)
            {
                Inventory.quantity[i] -= quantity;
                return;
            }
        }
    }

    public bool isReloadingCannon = true;
    private void ValidReloadingCannon()
    {
        // if (inventory != null && GetCannonBallInShip() <= 0)
        // {
        //     isReloadingCannon = false;
        // }
        isReloadingCannon = false;
        for (int i = 0; i < timeReloadCannons.Length; i++)
        {
            if (cooldownCannons[i] > 0 && cooldownCannons[i] < timeReloadCannons[i])
            {
                // Debug.Log("Reload Cannon Done reloading " + i + ":" + cooldownCannons[i] + "/" + timeReloadCannons[i]);
                isReloadingCannon = true;
                return;
            }
        }
    }

    private void ReloadCannons()
    {
        if (!isReloadingCannon) return;
        // if (inventory != null && GetCannonBallInShip() <= 0)
        // {
        //     return;
        // }
        for (int i = 0; i < 4; i++)
        {
            for (int c = 0; c < curShipData.numberDeck; c++)
            {
                int indexCannon = i * curShipData.numberDeck + c;
                if (numberCannon[indexCannon] == 0) continue;
                if (cooldownCannons[indexCannon] < timeReloadCannons[indexCannon])
                {
                    if (cooldownCannons[indexCannon] == 0)
                    {
                        if (Inventory != null)
                        {
                            int cannonBallRequired = numberCannon[indexCannon];
                            if (GetCannonBallInShip() < cannonBallRequired)
                            {
                                ValidReloadingCannon();
                                return;
                            }
                            DeductCannonBall(cannonBallRequired);
                        }
                    }
                    cooldownCannons[indexCannon] += Time.deltaTime;
                }
                // if (c < (curShipData.numberDeck - 1)) continue;
                if (c > 0) continue; //only first deck raise event
                if (cooldownCannons[indexCannon] >= timeReloadCannons[indexCannon])
                {
                    // Debug.Log("Reload Cannon Done " + indexCannon + " / " + c);
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
        ValidReloadingCannon();
    }

    internal void SetSpriteMinimap(Sprite minimapSprite)
    {
        foreach (Transform eachChild in transform)
        {
            if (eachChild.name == "MinimapInfo")
            {
                eachChild.GetComponent<SpriteRenderer>().sprite = minimapSprite;
                return;
            }
        }
    }

    private bool CheckCannonReady(CannonDirection direction, bool IsFire = false)
    {
        if (isStuned)
        {
            Debug.Log("stun cant fire");
            return false;
        }
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
        if (cooldownCannons[i * curShipData.numberDeck] >= timeReloadCannons[i * curShipData.numberDeck])
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
                for (int c = 0; c < curShipData.numberDeck; c++)
                {
                    cooldownCannons[i * curShipData.numberDeck + c] = 0f;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    internal void InitFromCustomData(ScriptableShipCustom playerStartShipCustom)
    {
        customData = playerStartShipCustom;
        ShipData = playerStartShipCustom.baseShipData;
        startShipData = playerStartShipCustom.PeakData;
        if (customData.curShipData != null)
        {
            curShipData = customData.curShipData.Clone<ScriptableShip>(); ;
        }

        if (!(customData.unions != null && customData.unions.Length > 0))
        {
            customData.unions = new ScriptableShipCustom.Union[] { (ScriptableShipCustom.Union)((int)Random.Range(2, 6)) };
        }
        ImgStateShip = SeaBattleManager.Instance.GetImgStateShip(customData.unions[0]);

        if (customData.group > 0)
        {
            Group = customData.group;
        }
        if (customData.skills != null)
        {
            foreach (ScriptableShipSkill skill in customData.skills)
            {
                RegisterShipSkill(skill);
            }
        }
    }


    internal ScriptableShipCustom BuildCustomData()
    {
        ScriptableShipCustom result = null;
        if (customData != null)
        {
            result = customData;
        }
        else
        {
            result = ScriptableObject.CreateInstance<ScriptableShipCustom>();
            // result.captain
            result.baseShipData = shipData;
            if (skillDatas != null)
            {
                result.skills = skillDatas.ToArray();
            }
            customData = result;
        }
        result.curShipData = curShipData;
        result.group = Group;
        return result;
    }

    public void EnterNoCrewState()
    {
        AutoSail = false;
        GetComponent<ShipAI>().enabled = false;
        curShipData.oarsSpeed = 0;
        Debug.Log("Test Damage Inflict Crew Damage Enter No Crew");
    }


    public void TakeDamage(DamageDealShip damage, GameObject source)
    {
        if (damage.crewDamage > 0)
        {
            SpawnHumanFly(source);
            curShipData.maxCrew -= (int)damage.crewDamage;
            Debug.Log("Test Damage Inflict Crew Damage " + (int)damage.crewDamage + "=>" + curShipData.maxCrew);
            if (curShipData.maxCrew <= 0)
            {
                curShipData.maxCrew = 0;
                EnterNoCrewState();
            }
        }
        if (damage.sailDamage > 0)
        {
            curShipData.sailHealth -= (int)damage.sailDamage;
            Debug.Log("Test Damage Inflict sail Damage " + (int)damage.sailDamage + "=>" + curShipData.sailHealth);

            if (curShipData.sailHealth <= 0) curShipData.sailHealth = 0;
            RevalidMovement();
        }
        if (damage.hullDamage > 0)
        {
            float curHealth = curShipData.hullHealth;
            float maxHealth = startShipData.hullHealth;
            float healthRateB = curHealth / maxHealth;
            float healthRateA = (curHealth - damage.hullDamage) / maxHealth;

            if (healthRateA <= 0f)
            {
                spriteRenderer.sprite = imgStateShip.deathState;
                MakeDeathShip();
            }
            else if ((healthRateB > 0.2f) && (healthRateA <= 0.2f))
            {
                spriteRenderer.sprite = imgStateShip.dangerState;
                SpawnRandomFire();
            }
            else if ((healthRateB > 0.5f) && (healthRateA <= 0.5f))
            {
                SpawnRandomFire();
            }
            else if ((healthRateB > 0.7f) && (healthRateA <= 0.7f))
            {
                spriteRenderer.sprite = imgStateShip.damagedState;
                SpawnRandomFire();
            }
            else if (healthRateA > 0.7f)
            {
                // RestoreState();
            }

            curShipData.hullHealth -= damage.hullDamage;
            if (curShipData.hullHealth <= 0) curShipData.hullHealth = 0;
            if (lastTimeHit >= curShipData.TimeHitToStun)
            {
                lastTimeHit = 0;
                totalHitConsecutive = 1;
                Debug.Log("Stun first hit");
            }
            else
            {
                totalHitConsecutive++;
                Debug.Log("Stun accum hit " + totalHitConsecutive);
                if (totalHitConsecutive >= curShipData.MaxHitToStun)
                {
                    Debug.Log("Stun get stunned ");
                    stunEffect?.SetActive(true);
                    isStuned = true;
                    lastTimeHit = 0;
                    totalHitConsecutive = 0;
                    accumStunTime = 0;
                }
            }
        }
    }

    public void RestoreState()
    {
        spriteRenderer.sprite = imgStateShip.normalState;
        ClearFire();
        ShipAI AI = GetComponent<ShipAI>();
        if (AI != null)
        {
            AI.enabled = true;
        }
        ApplyWindForce(SeaBattleManager.Instance.windForce);
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
        CapsuleCollider2D col = ShipCollider;
        fire.transform.localPosition = new Vector2(
            Random.Range(-col.size.x / 2, col.size.x / 2),
            Random.Range(-col.size.y / 2, col.size.y / 2)
        );
        Fires.Add(fire);
    }



    public void SpawnHumanFly(GameObject source)
    {
        if (source == null) return;

        WrapPool wrapPool = SeaBattleManager.Instance.poolHumanFly.GetPooledObject();
        if (wrapPool != null)
        {
            GameObject humanfly = wrapPool.poolObj;

            Vector2 direction = transform.position - source.transform.position;
            direction = Rotate(direction, Random.Range(-45f, 45f));
            humanfly.transform.position = new Vector2(
                transform.position.x + direction.x,
                transform.position.y + direction.y
            );
            humanfly.SetActive(true);
            humanfly.GetComponentInChildren<HumanFly>().OnAnimateDone = delegate ()
            {
                SeaBattleManager.Instance.poolHumanFly.RePooledObject(wrapPool);
            };

            humanfly.GetComponentInChildren<HumanFly>().StartAnimate(direction);
        }

        // GameObject humanfly = Instantiate(HumanFly, null, true);
        // Vector2 direction = transform.position - source.transform.position;
        // direction = Rotate(direction, Random.Range(-45f, 45f));
        // humanfly.transform.position = new Vector2(
        //     transform.position.x + direction.x,
        //     transform.position.y + direction.y
        // );
        // humanfly.GetComponentInChildren<HumanFly>().StartAnimate(direction);
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
        if (((direction & CannonDirection.Front) != 0))
        {
            results.Add(RotateByOneDirection(frontV, CannonDirection.Front));
        }
        if (((direction & CannonDirection.Left) != 0))
        {
            results.Add(RotateByOneDirection(frontV, CannonDirection.Left));
        }
        if (((direction & CannonDirection.Right) != 0))
        {
            results.Add(RotateByOneDirection(frontV, CannonDirection.Right));
        }
        if (((direction & CannonDirection.Back) != 0))
        {
            results.Add(RotateByOneDirection(frontV, CannonDirection.Back));
        }
        return results;
    }

    public static Vector2 RotateByOneDirection(Vector2 frontV, CannonDirection direction)
    {
        switch (direction)
        {
            case CannonDirection.Front: return Rotate(frontV, 0, true);
            case CannonDirection.Left: return Rotate(frontV, 90, true);
            case CannonDirection.Right: return Rotate(frontV, -90, true);
            case CannonDirection.Back: return Rotate(frontV, 180, true);
            default: return Rotate(frontV, 0, true);
        }
    }

    internal void ApplyWindForce(Vector2 windForce)
    {
        if (IsDeath) return;
        Wind = windForce * curShipData.windConversionRate;
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
            CapsuleCollider2D col = ShipCollider;
            float c = Mathf.PI * ActualSizeY / 2; // 1/2 perimeter
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
        if (!AutoSail) return;
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
        Vector2 vShip = Vector2.zero;
        if (fixedSpeed == 0)
        {
            Vector2 actualWindForce = Wind * sailSet; //reduce by sail user control
            actualWindForce = actualWindForce * curShipData.sailHealth / startShipData.sailHealth; //reduce by sail damage 

            Debug.Log("Test Damage Inflict Sail Damage " + (curShipData.sailHealth / startShipData.sailHealth) + " => " + actualWindForce);
            Vector2 vSail = VectorUtils.GetForceOnLine(actualWindForce, SailDirecion, false);
            vShip = VectorUtils.GetForceOnLine(vSail, ShipDirection);
            // Debug.Log(string.Format("calculate move {0}/{1}/{2}", Wind, vSail, vShip));
            vShip = vShip + ShipDirection.normalized * curShipData.oarsSpeed;
            if (!VectorUtils.IsSameDirection(ShipDirection, vShip))
            {
                vShip = Vector2.zero;
            }
        }
        else
        {
            vShip = ShipDirection.normalized * fixedSpeed;
        }
        // rigidBody2d.AddForce(vShip);
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
        Vector2 flag = VectorUtils.Rotate(ShipDirection, 45, true);
        float angel = Vector2.Angle(toTarget, flag);

        Vector2 crossDir = VectorUtils.Rotate(ShipDirection, -90, true).normalized;
        CapsuleCollider2D col = ShipCollider;
        Vector2 pA = (Vector2)transform.position + ShipDirection.normalized * ActualSizeY / 2
        + crossDir * ActualSizeX / 2;
        Vector2 pB = (Vector2)transform.position + ShipDirection.normalized * ActualSizeY / 2
        + new Vector2(-crossDir.x, -crossDir.y) * ActualSizeX / 2;
        Vector2 pC = (Vector2)transform.position + new Vector2(-ShipDirection.x, -ShipDirection.y).normalized * ActualSizeY / 2
        + new Vector2(-crossDir.x, -crossDir.y) * ActualSizeX / 2;
        Vector2 pD = (Vector2)transform.position + new Vector2(-ShipDirection.x, -ShipDirection.y).normalized * ActualSizeY / 2
         + crossDir * ActualSizeX / 2;

        float range = 5f;
        if (CheckCannonReady(CannonDirection.Front))
        {
            if (VectorUtils.IsPointInRectangle(target,
            pA, pB, pB + ShipDirection.normalized * range, pA + ShipDirection.normalized * range
            ))
            {
                FireCannonOneDirection(CannonDirection.Front);
            }
        }
        if (CheckCannonReady(CannonDirection.Left))
        {
            if (VectorUtils.IsPointInRectangle(target,
            pB, pC, pC + VectorUtils.Reverse(crossDir).normalized * range, pB + VectorUtils.Reverse(crossDir).normalized * range
            ))
            {
                FireCannonOneDirection(CannonDirection.Left);
            }
        }
        if (CheckCannonReady(CannonDirection.Right))
        {
            if (VectorUtils.IsPointInRectangle(target,
            pA, pD, pD + crossDir.normalized * range, pA + crossDir.normalized * range
            ))
            {
                FireCannonOneDirection(CannonDirection.Right);
            }
        }

        if (CheckCannonReady(CannonDirection.Back))
        {
            if (VectorUtils.IsPointInRectangle(target,
            pC, pD, pD + VectorUtils.Reverse(ShipDirection).normalized * range, pC + VectorUtils.Reverse(ShipDirection).normalized * range
            ))
            {
                FireCannonOneDirection(CannonDirection.Back);
            }
        }

        /*
        //check by angel
        if (VectorUtils.IsRightSide(flag, toTarget))
        {
            if (angel >= 40 && angel <= 50)
            {
                FireCannonOneDirection(CannonDirection.Front);
            }
            else if (angel >= 130 && angel <= 140)
            {
                FireCannonOneDirection(CannonDirection.Right);
            }
        }
        else
        {
            if (angel >= 40 && angel <= 50)
            {
                FireCannonOneDirection(CannonDirection.Left);
            }
            else if (angel >= 130 && angel <= 140)
            {
                FireCannonOneDirection(CannonDirection.Back);
            }
        }
                */
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

    public int NumberCannonDirectionFire(CannonDirection direction, int deck = 0, bool reload = true)
    {
        int indexCheck = -1;
        switch (direction)
        {
            case CannonDirection.Front:
                indexCheck = 0 * curShipData.numberDeck + deck; break;
            // return numberCannon.Length > 0 ? numberCannon[0 * curShipData.numberDeck + deck] : 0;
            case CannonDirection.Right:
                indexCheck = 1 * curShipData.numberDeck + deck; break;
            // return numberCannon.Length > 1 ? numberCannon[1 * curShipData.numberDeck + deck] : 0;
            case CannonDirection.Left:
                indexCheck = 2 * curShipData.numberDeck + deck; break;
            // return numberCannon.Length > 2 ? numberCannon[2 * curShipData.numberDeck + deck] : 0;
            case CannonDirection.Back:
                indexCheck = 3 * curShipData.numberDeck + deck; break;
            // return numberCannon.Length > 3 ? numberCannon[3 * curShipData.numberDeck + deck] : 0;
            default: return 0;
        }
        if (numberCannon.Length > indexCheck)
        {
            if (cooldownCannons[indexCheck] >= timeReloadCannons[indexCheck])
            {
                if (reload)
                {
                    cooldownCannons[indexCheck] = 0;
                    if (deck == 0)
                    {
                        switch (direction)
                        {
                            case CannonDirection.Front: Events.InvokeOnAction(EVENT_CANNON_FRONT_FIRE); break;
                            case CannonDirection.Right: Events.InvokeOnAction(EVENT_CANNON_RIGHT_FIRE); break;
                            case CannonDirection.Left: Events.InvokeOnAction(EVENT_CANNON_LEFT_FIRE); break;
                            case CannonDirection.Back: Events.InvokeOnAction(EVENT_CANNON_BACK_FIRE); break;
                            default: break;
                        }
                    }
                }
                return numberCannon[indexCheck];
            }
            return 0;
        }
        return 0;
    }

    public int countShot = 0;

    private void FireCannonOneDirection(CannonDirection direction)
    {
        if (!CheckCannonReady(direction)) return;
        // Vector2 to = Rotate(Vector2.down, transform.rotation.eulerAngles.z); //current front
        countShot++;
        StartCoroutine(DelayFire(direction, 0.1f));
    }

    public IEnumerator DelayFire(CannonDirection direction, float delay)
    {
        for (int i = 0; i < curShipData.numberDeck; i++)
        {
            FireCannonOneDirectionOnDeck(direction, i);
            yield return new WaitForSeconds(delay);
        }
    }

    private void FireCannonOneDirectionOnDeck(CannonDirection direction, int deck)
    {
        Vector2 fire = RotateByOneDirection(GetFrontDirection(), direction);
        int numberFires = NumberCannonDirectionFire(direction, deck);
        CapsuleCollider2D col = ShipCollider;
        List<Vector2> fromList = new List<Vector2>();

        float areaFireFace = 0;
        float areaOtherFace = 0;
        Vector2 vPillar = ShipDirection;
        // Debug.Log(string.Format("{0}/{1}/{2}/{3}", transform.position, fire.normalized, col.size, direction));
        if (direction == CannonDirection.Front || direction == CannonDirection.Back)
        {
            areaFireFace = ActualSizeX;
            areaOtherFace = ActualSizeY;
        }
        else
        {
            areaFireFace = ActualSizeY;
            areaOtherFace = ActualSizeX;
        }
        switch (direction)
        {
            case CannonDirection.Back:
                vPillar = new Vector2(-vPillar.x, -vPillar.y);
                break;
            case CannonDirection.Front:
                break;
            case CannonDirection.Left:
                vPillar = VectorUtils.GetPerpendicular(vPillar, false);
                break;
            case CannonDirection.Right:
                vPillar = VectorUtils.GetPerpendicular(vPillar);
                break;
            default:
                break;
        }
        for (int i = 0; i < numberFires; i++)
        {
            float area2 = areaFireFace / (2 * numberFires);
            var pos = -((areaFireFace / 2) - area2) + i * area2 * 2;

            Vector2 shotFrom = (Vector2)transform.position +
            vPillar.normalized * areaOtherFace / 2
            + VectorUtils.GetPerpendicular(vPillar, false).normalized * pos;
            // + (((i + 1) > (0.5 + (numberFires / 2)) ? VectorUtils.GetPerpendicular(vPillar, false) : VectorUtils.GetPerpendicular(vPillar)).normalized * pos);
            fromList.Add(shotFrom);
            if (deck == 0) Debug.DrawLine(transform.position, shotFrom, Color.blue, 3f);
            // float angel = Vector2.Angle(new Vector2(-areaOtherFace / 2, 0), new Vector2(-areaOtherFace / 2, -pos));
            // Debug.Log("pos fire " + i + " " + pos + " " + area2 + " " + areaOtherFace + " " + angel);
            // angel = ((i + 1) > (0.5 + (numberFires / 2)) ? 1 : -1) * angel;
            // if (deck == 0) Debug.DrawLine(transform.position, (Vector2)transform.position + Rotate(fire, angel, true), Color.blue, 3f);
            // fromList.Add((Vector2)transform.position + Rotate(fire, angel, true).normalized * areaOtherFace / 2);

        }
        foreach (Vector2 from in fromList)
        {
            WrapPool wrapPool = SeaBattleManager.Instance.poolCannon.GetPooledObject();
            Debug.Assert(wrapPool != null, "cannon should avaiable");
            if (wrapPool != null)
            {
                GameObject actualCannon = wrapPool.poolObj;
                CannonShot shot = actualCannon.GetComponent<CannonShot>();
                shot.ResetTravel();
                actualCannon.transform.position = from;
                shot.owner = this;
                shot.fireDirection = fire.normalized;
                // shot.Target = from + fire.normalized * 3f;
                // shot.speed = speed;
                shot.OnImpactTarget = delegate ()
                {
                    SeaBattleManager.Instance.poolCannon.RePooledObject(wrapPool);
                };
                shot.gameObject.SetActive(true);
                shot.StartTravel();

                if (deck == 0) Debug.DrawLine(from, shot.Target, Color.red, 3f);
            }
        }
        isReloadingCannon = true;
    }

    public void RegisterShipSkill(ScriptableShipSkill skillData)
    {
        if (shipSkills == null) shipSkills = new List<ShipSkill>();
        if (skillDatas == null) skillDatas = new List<ScriptableShipSkill>();
        skillDatas.Add(skillData);

        Object instanceSkill = Instantiate(skillData.prefab, model.transform, false);
        ShipSkill shipSkill = ((GameObject)instanceSkill).GetComponent<ShipSkill>();
        shipSkill.RegisterShip(this);
        shipSkills.Add(shipSkill);
        // shipSkill.ActiveSkill();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("ship trigger " + other.tag);
    }

    public Collision2D LastCollision2D { get; set; }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals(GameSettings.TAG_SHIP))
        {
            LastCollision2D = other;
            Events.InvokeOnAction(EVENT_TACKED_OTHER_SHIP);
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
    }
}
