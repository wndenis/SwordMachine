using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_BodyRotator : MonoBehaviour
{
	private Quaternion defaultRot;
	private Transform trfm;

	// Use this for initialization
	void Start ()
	{
		trfm = transform;
		defaultRot = trfm.rotation;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		trfm.rotation = defaultRot;
	}
}
