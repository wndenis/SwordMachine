using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHitSound : MonoBehaviour
{
    public float soundVelocity;
    public AudioClip[] AudioClips;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = AudioClips[0];
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.relativeVelocity.magnitude > 2)
        {
            audioSource.clip = AudioClips[Random.Range(0, AudioClips.Length - 1)];
            audioSource.Play();
        }
    }
}
