using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

public class FXCollision : MonoBehaviour
{
    public BehaviourPuppet puppet;
    public float minCollisionImpulse = 300f;
    public int emission = 10;
    public float emissionImpulseAdd = 0.01f;
    public int maxEmission = 50;
    private ParticleSystem particles;

    void Start () {
        particles = GetComponentInChildren<ParticleSystem>();
        puppet.OnCollisionImpulse += OnCollisionImpulse;
    }

    void OnCollisionImpulse(MuscleCollision m, float impulse) {
        if (m.collision.contacts.Length == 0) return;
        if (impulse < minCollisionImpulse) return;
        //todo: check for our sword collision to ignore damage
        //todo: if impulse is higher than some threshold - pin zero, state stun.
        //todo: stun may include pin weight management
        

        transform.position = m.collision.contacts[0].point + m.collision.contacts[0].normal.normalized * 0.01f;
        transform.rotation = Quaternion.LookRotation(m.collision.contacts[0].normal);
        particles.Emit(Mathf.Min(emission + (int)(emissionImpulseAdd * impulse), maxEmission));
    }

    void OnDestroy() {
        if (puppet != null) puppet.OnCollisionImpulse -= OnCollisionImpulse;
    }
}

