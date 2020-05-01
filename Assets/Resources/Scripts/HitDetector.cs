using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetector : MonoBehaviour
{
    public ParticleSystem particles;

    private void OnCollisionEnter(Collision other)
    {
        particles.Emit(30);
    }
}
