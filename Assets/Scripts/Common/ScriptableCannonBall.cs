using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableCannonBall", menuName = "BoS/Cannon Ball", order = 1)]
public class ScriptableCannonBall : ScriptableObjectPrefab
{
    public string cannonName;
    public float shipDamage;
    public float humanDamage;
    public Sprite sprite;
}
