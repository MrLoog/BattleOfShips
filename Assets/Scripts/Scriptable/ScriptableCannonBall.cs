using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableCannonBall", menuName = "BoS/Cannon Ball", order = 1)]
public class ScriptableCannonBall : ScriptableObjectPrefab
{
    public string cannonName;
    public string codeName;

    public float hullDamage;
    public float sailDamage;
    public float crewDamage;

    [Tooltip("damage when get blocked")]
    public float damage;
    public float speed;
    public float range;
    public Sprite sprite;

}
