using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SciFiSwordAnimation : MonoBehaviour
{ 
    public Animator swordToAnimate; 
    public Collider ScratchMaker;
    [Range(0f, 2f)]
    public float pitch = 1f;
    [Space] public AudioClip openCloseSound;
    public ParticleSystem particles;
    [Space(5)] public bool Activate;

    private bool _scratchMaker = false;
    private bool _opened;
    private AudioSource audioSource;
    private static readonly int Direction = Animator.StringToHash("direction");

    
    private void OnValidate()
    {
        if (Activate && Application.isPlaying)
        {
            Activate = false;
            PlayAnimation();
        }
    }
    
    
    private void Awake()
    {
        if (ScratchMaker != null)
            _scratchMaker = true;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = openCloseSound;
        swordToAnimate.SetFloat(Direction, 0);
    }

    public void Open()
    {
        if (!_opened)
            PlayAnimation();
    }

    public void Close()
    {
        if (_opened)
            PlayAnimation();
    }

    public void Switch()
    {
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        _opened = !_opened;
        var speed = _opened ? 1 : -1;
        var playTime = swordToAnimate.GetCurrentAnimatorStateInfo(0).normalizedTime;
        playTime = Mathf.Clamp01(playTime);
        swordToAnimate.SetFloat(Direction, speed);
        swordToAnimate.Play("OpenClose", -1, playTime);
        var localPitch = pitch + Random.Range(0.01f, 0.025f);
        var scratchSwitchDelay = 0f;
        
        //audio and FX
        if (_opened)
        {
            if (audioSource.timeSamples >= audioSource.clip.samples - 1)
                audioSource.timeSamples = 0;

            particles.Play();
            audioSource.pitch = localPitch;
            scratchSwitchDelay = 0.5f;
        }
        else
        {
            if (audioSource.timeSamples < 2)
                audioSource.timeSamples = audioSource.clip.samples - 1;
            audioSource.pitch = -localPitch + 0.075f;
            scratchSwitchDelay = 0f;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.volume = Random.Range(0.85f, 1f);
            audioSource.Play();
        }
        
        Invoke(nameof(SetScratches), scratchSwitchDelay);
    }

    private void SetScratches()
    {
        if (_scratchMaker)
            ScratchMaker.enabled = _opened;
    }
}
