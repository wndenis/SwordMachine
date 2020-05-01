using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

public class FXCollisionSound : MonoBehaviour
{
    //todo: надо было наследоваться, но я не хотел
    public BehaviourPuppet puppet;
    public float minCollisionImpulse = 300f;
    public AudioClip[] sounds;
	
	private float lastSoundTime = 0f;

    private AudioSource audioSource;

    void Start () {
        audioSource = GetComponentInChildren<AudioSource>();
        puppet.OnCollisionImpulse += OnCollisionImpulse;
    }

    void OnCollisionImpulse(MuscleCollision m, float impulse) {
        if (m.collision.contacts.Length == 0) return;
        if (impulse < minCollisionImpulse) return;
		if (Time.time - lastSoundTime < 0.125f) return;
		
		lastSoundTime = Time.time;
        transform.position = m.collision.contacts[0].point + m.collision.contacts[0].normal.normalized * 0.01f;

        var clip = sounds[Random.Range(0, sounds.Length)];
		if (!audioSource.isPlaying)
			audioSource.pitch = Random.Range(0.925f, 1.075f);
        audioSource.PlayOneShot(clip, Random.Range(0.9f, 1f));
    }

    void OnDestroy() {
        if (puppet != null) puppet.OnCollisionImpulse -= OnCollisionImpulse;
    }
}
