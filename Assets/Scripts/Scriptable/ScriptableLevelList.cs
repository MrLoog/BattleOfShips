using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableLevelList", menuName = "BoS/Level List", order = 8)]
public class ScriptableLevelList : ScriptableObjectPrefab
{
    public long[] expLevel;
}
