using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public bool animate = true;
    [Space(10)]
    public Color color;
    [Range(-2, 10)]
    public float intensity = 3f;

    private MeshRenderer target;

    private Coroutine anim;
    private bool ready;

    // Start is called before the first frame update
    void Start()
    {
        target = GetComponent<MeshRenderer>();
        ready = true;
        OnValidate();
    }

    private void OnValidate()
    {
        if (!ready)
            return;
        if (animate)
        {
            if (anim == null)
                anim = StartCoroutine(LightAnimation());
        }
        else
        {
            StopAllCoroutines();
            anim = null;
			SetColor(color);
        }
    }
	
	private void SetColor(Color newColor){
		target.material.SetColor("_EmissionColor", newColor * intensity);
		DynamicGI.SetEmissive(target, newColor * intensity);
		//target.material.EnableKeyword("_EMISSION");
        target.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        //target.material.SetColor("_EmissionColor", newColor * intensity);
	}

    private IEnumerator LightAnimation()
    {
        print("Routine");
        var from = new Color(1f, 0f, 0.05f);
        var to = new Color(0.05f, 0f, 1f);
        var gradient = new Gradient 
        {
            alphaKeys = new [] {new GradientAlphaKey()}, 
            colorKeys = new []
            {
                new GradientColorKey(from, 0f), 
                new GradientColorKey(to, 0.5f), 
                new GradientColorKey(from, 1f)
            }
        };

        var t = 0f;
        var dur = 5f;
        while (true)
        {
            t += Time.deltaTime;
            t %= dur;
			SetColor(gradient.Evaluate(t / dur));
            yield return null;
        }
    }
}
