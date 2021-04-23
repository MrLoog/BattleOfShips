using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleStopDestroyParent : MonoBehaviour
{
    public float delay = 0f;
    private void OnParticleSystemStopped()
    {
        Destroy(transform.parent.gameObject, delay);
    }
}
