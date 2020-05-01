using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource leftFoot;
    public AudioSource rightFoot;
    public AudioClip[] footstepSounds;

    private void Start()
    {
        leftFoot.loop = false;
        rightFoot.loop = false;
    }

    private float RandPitch()
    {
        return Random.Range(0.8f, 1.15f);
    }

    private void PlayFootstep(AudioSource foot)
    {
        if (foot.isPlaying)
            return;
        foot.pitch = RandPitch();
        foot.clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        foot.Play();
    }
    
    public void FootstepLeft()
    {
        PlayFootstep(leftFoot);
    }

    public void FootstepRight()
    {
        PlayFootstep(rightFoot);
    }
}
