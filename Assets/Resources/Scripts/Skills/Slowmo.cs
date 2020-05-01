using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SocialPlatforms;

public class Slowmo : MonoBehaviour
{
    [Range(0.05f, 10f)]
    public float newTimeScale = 0.25f;
    public float duration = 0.2f;
    
    public AudioClip Begin;
    public AudioClip Hold;
    public AudioClip End;

    private AudioSource _audioSource;
    private float lastUseTime;
    private const float CooldownTotal = 8f;
    private bool _slowing;
    private PostProcessVolume postProcessing;

    private float defaultTimeScale;
    private float defaultFixedTimeScale;
    private AudioReverbZone reverb;

    // Use this for initialization
    private void Start () {
        postProcessing = GetComponent<PostProcessVolume>();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.bypassEffects = true;

        reverb = GetComponent<AudioReverbZone>();
        reverb.enabled = false;

        defaultTimeScale = 1f;
        defaultFixedTimeScale = Time.fixedDeltaTime;
    }
	
	// Update is called once per frame
    private void Update () {
        if (Input.GetKeyDown(KeyCode.E) && Time.time - lastUseTime >= CooldownTotal){
            if (!_slowing)
            {
                StartCoroutine(SlowmoProcess());
            }
        }
    }

    private IEnumerator SlowmoProcess()
    {
        _slowing = true;
        _audioSource.Stop();
        _audioSource.clip = Begin;
        _audioSource.volume = 1;
        _audioSource.pitch = 1f;
        _audioSource.Play();
        yield return StartCoroutine(SlowIn());
        reverb.enabled = true;
        yield return new WaitForSecondsRealtime(5f);
        reverb.enabled = false;
        _audioSource.Stop();
        _audioSource.clip = End;
        _audioSource.volume = 0.55f;
        _audioSource.pitch = 1.45f;
        _audioSource.Play();
        yield return StartCoroutine(SlowOut());
        _slowing = false;
    }

    private IEnumerator SlowIn()
    {
        var t = 0f;
        // var sounds = new HashSet<AudioSource>(FindObjectsOfType<AudioSource>());
        while (t <= duration)
        {
            t += Time.unscaledDeltaTime;
            var time = t / duration;

            // foreach (var ac in FindObjectsOfType<AudioSource>())
            // {
            //     if (!ac.Equals(_audioSource))
            //         ac.pitch = Mathf.Lerp(1f, 0.35f, time);
            // }
            postProcessing.weight = Mathf.Lerp(0f, 1f, time);
            Time.timeScale = Mathf.Lerp(defaultTimeScale, newTimeScale, time);
            Time.fixedDeltaTime = defaultFixedTimeScale * Time.timeScale;
            yield return null;
        }
        postProcessing.weight = 1f;
        Time.fixedDeltaTime = defaultFixedTimeScale * newTimeScale;
        Time.timeScale = newTimeScale;
        
        _audioSource.clip = Hold;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private IEnumerator SlowOut()
    {
        _audioSource.loop = false;
        var t = 0f;

        while (t <= duration)
        {
            t += Time.unscaledDeltaTime;
            var time = t / duration;

            // foreach (var ac in FindObjectsOfType<AudioSource>())
            // {
            //     if(!ac.Equals(_audioSource))
            //         ac.pitch = Mathf.Lerp(0.35f, 1f, time);
            // }
            postProcessing.weight = Mathf.Lerp(1f, 0f, time);
            Time.timeScale = Mathf.Lerp(newTimeScale, defaultTimeScale, time);
            Time.fixedDeltaTime = defaultFixedTimeScale * Time.timeScale;
            yield return null;
        }
        // foreach (var ac in FindObjectsOfType<AudioSource>())
        // {
        //     ac.pitch = 1f;
        // }
        postProcessing.weight = 0f;
        Time.fixedDeltaTime = defaultFixedTimeScale;
        Time.timeScale = defaultTimeScale;
        lastUseTime = Time.time;
    }
}
