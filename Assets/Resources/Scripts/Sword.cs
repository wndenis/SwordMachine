using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
[DisallowMultipleComponent]
#endif

public class Sword : MonoBehaviour
{
	public PlayableUnit Owner;
	public Transform TipTransform;
	public Transform MiddleTransform;
	public Transform HandleTransform;
	public Rigidbody Rigidbody;
	private float cooldownTotal = 0.1165f;
	private float lastAttackTime;
	public TrajectoryApproximator trajectoryApproximator;

	private void Start()
	{
		if (Rigidbody == null)
			Rigidbody = GetComponent<Rigidbody>();
		Rigidbody.maxAngularVelocity = 55f;
		//0.15f
		//120
		//120 / 0.15f
	}

	public void Delay()
	{
		lastAttackTime = Time.time;
	}

	public bool IsReady()
	{
		return !(Time.time - lastAttackTime < cooldownTotal);
	}

	public float Damage = -1; //require negative for correct damage todo: fix?
	
	public delegate void HitCallback(Collision other);
	public HitCallback HitCallbackMethod;

	private void OnCollisionEnter(Collision other)
	{
		return;
		if (IsReady())
			HitCallbackMethod?.Invoke(other); // if method != null => Invoke
	}
}
