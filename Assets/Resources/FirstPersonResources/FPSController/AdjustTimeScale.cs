using UnityEngine;
using System.Collections;
using TMPro;


public class AdjustTimeScale : MonoBehaviour
{
    private float oldFixed;

    private void Start()
    {
        oldFixed = Time.fixedDeltaTime;
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (Time.timeScale < 1.0F)
            {
                Time.timeScale += 0.05f;
            }
               
            Time.fixedDeltaTime = oldFixed * Time.timeScale;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (Time.timeScale >= 0.05F)
            {
                Time.timeScale -= 0.05f;
            }
                
            Time.fixedDeltaTime = oldFixed * Time.timeScale;
        }
    }

    void OnApplicationQuit()
    {
        Time.timeScale = 1.0F;
        Time.fixedDeltaTime = oldFixed;
    }
}