using System.Collections;
using System.Collections.Generic;
using Resources.Scripts;
using RootMotion.Demos;
using UnityEngine;

public class CustomControllerLol : UserControlThirdPerson
{ 
	/// <summary>
	/// User input for an AI controlled character controller.
	/// </summary>
	// public MoveTarget moveTarget;
	// public Transform moveTarget;
	// public float stoppingDistance = 0.5f;
	// public float stoppingThreshold = 1.5f;
	// public float brakingDistance = 5f;
	public Navigator navigator;

	protected override void Start()
	{
		base.Start();
		navigator.Initiate(transform);
	}

    protected override void Update () {
	       //
        // float moveSpeed = 1f;
        // if (moveTarget == null) //todo fix it
	       //  return;
        // // If using Unity Navigation
        // if (navigator.activeTargetSeeking)
        // {
        //     navigator.Update(moveTarget.position);
        //     var speed = moveSpeed;
        //     if (navigator.lastCorner)
        //     {
        //         if (navigator.Distance < stoppingDistance + 1.5f)
	       //          speed = 0;
        //         else{
	       //          var brakingForStopping = brakingDistance + stoppingDistance;
	       //          var deltaDist = navigator.Distance - brakingForStopping;
	       //          // Торможение
	       //          if (deltaDist <= 0)
		      //           speed = Mathf.Lerp(moveSpeed, 0.0f, -deltaDist / brakingForStopping);
        //         }
        //     }
        //     
        //     state.move = navigator.normalizedDeltaPosition * speed;
        // }
        // // No navigation, just move straight to the target
        // else
        // {
        //     Vector3 direction = moveTarget.position - transform.position;
        //     float distance = direction.magnitude;
        //
        //     Vector3 normal = transform.up;
        //     Vector3.OrthoNormalize(ref normal, ref direction);
        //
        //     float sD = state.move != Vector3.zero ? stoppingDistance : stoppingDistance * stoppingThreshold;
        //
        //     state.move = distance > sD ? direction * moveSpeed : Vector3.zero;
        //     state.lookPos = moveTarget.position;
        // }
	}

    // Visualize the navigator
    void OnDrawGizmos()
    {
        if (navigator.activeTargetSeeking) navigator.Visualize();
    }
}
