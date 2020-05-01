using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
	public bool RigidbodyMode = false;
    public Transform Target;
    public float Sharpness = 0.1f;

    private Rigidbody Rigidbody;
	// Use this for initialization
	void Start ()
	{
		Rigidbody = GetComponent<Rigidbody>();
	}
	
	void LateUpdate () {

        // This constructs a rotation looking in the direction of our target,
        Quaternion targetRotation = Quaternion.LookRotation(Target.position - transform.position);
        var rotation = Quaternion.Slerp(transform.rotation, targetRotation, Sharpness);
        // This blends the target rotation in gradually.
        // Keep sharpness between 0 and 1 - lower values are slower/softer.

        if (RigidbodyMode)
	        Rigidbody.MoveRotation(rotation);
        else
	        transform.rotation = rotation;
	}
}
