using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandAnimator : MonoBehaviour
{
    [Tooltip("Meters")]    public float HorizontalAmp;
    [Tooltip("Meters")]    public float VerticalAmp;
    [Tooltip("Seconds")]   public float Duration;
    
    private SpringJoint springJoint;
    private Vector3 defaultAnchor;
    
    private Vector3 horizontalVector;
    private Vector3 verticalVector;
    
    private Vector3 randomizedHorizontalVector;
    private Vector3 randomizedVerticalVector;
    
    private float randomizedDuration;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        randomizedDuration = Duration;
        springJoint = GetComponent<SpringJoint>();
        defaultAnchor = springJoint.connectedAnchor;
        horizontalVector = new Vector3(HorizontalAmp, 0, 0);
        verticalVector = new Vector3(0, VerticalAmp, 0);
    }

    //Deviation in range 0..1
    float RandomizedValue(float origin, float deviation)
    {
        return origin + Random.Range(-1, 1) * origin * deviation;
    }

    Vector3 RandomizedValue(Vector3 origin, float deviation)
    {
        return origin + new Vector3(RandomizedValue(origin.x, deviation),
                                    RandomizedValue(origin.y, deviation),
                                    RandomizedValue(origin.z, deviation));
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer > randomizedDuration * 2)
        {
            timer -= randomizedDuration * 2;
            randomizedDuration = RandomizedValue(Duration, Duration / 4);
            randomizedVerticalVector = RandomizedValue(verticalVector, 1);
            randomizedHorizontalVector = RandomizedValue(horizontalVector, 1);
        }

        var t = Mathf.PingPong(timer, randomizedDuration);
        springJoint.connectedAnchor = defaultAnchor + randomizedVerticalVector * t + randomizedHorizontalVector * t;
        timer += Time.fixedDeltaTime;
    }
}
