using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public float minOnTime;
    public float maxOnTime;
    public float minOffTime;
    public float maxOffTime;

    private Light light;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        Play();
    }

    private IEnumerator Animation()
    {
        while (true)
        {
            light.enabled = true;
            yield return new WaitForSeconds(Random.Range(minOnTime, maxOnTime));
            light.enabled = false;
            yield return new WaitForSeconds(Random.Range(minOffTime, maxOffTime));
        }
    }

    public void Stop()
    {
        StopAllCoroutines();
    }
    
    public void Play()
    {
        StartCoroutine(Animation());
    }
}
