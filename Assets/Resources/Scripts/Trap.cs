using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public ParticleSystem awaitParticles;
    public ParticleSystem explosion;

    public float force;

    private float cooldown = 5f;
    private float lastExplosionTime = -5f;

    private SphereCollider collider;
    // Start is called before the first frame update
    void Start()
    {
        awaitParticles.Play();
        explosion.Stop();
        collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (Time.time - lastExplosionTime >= cooldown)
        {
            lastExplosionTime = Time.time;
            StartCoroutine(Explode());
        }
            
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.5f);
        explosion.Play();
        var colliders = Physics.OverlapSphere(transform.position, collider.radius * 2f);
        foreach (var elem in colliders)
        {
            var rb = elem.GetComponent<Rigidbody>();
            if (rb == null)
                continue;
            rb.AddExplosionForce(force, transform.position, collider.radius*2f,1f);
        }
    }
}
