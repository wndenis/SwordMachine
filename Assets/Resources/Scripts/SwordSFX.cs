using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Sword))]
public class SwordSFX : MonoBehaviour
{
	public Interactable steamVrObject;
	public AudioClip[] smallClangs;
	public AudioClip[] greaterClangs;
	public AudioClip[] lesserSwishes;
	public AudioClip[] greaterSwishes;
	public GameObject Sparks;

    public float SwashThresh = 0.1f;
	public float MidSwashSpeed = 0;
	public float HighSwashSpeed = 95;
    private float swashThreshCounter;

    private float movementSpeed;
	private float rotationSpeed;

	private SteamVR_Action_Vibration haptics = SteamVR_Actions.default_Haptic;
	private bool isSwashing;
	
	private enum OwnerType
	{
		AI, Player
	}
	private OwnerType ownerType;
	
	private AudioSource tipAudioSource;
	private Sword sword;


    // Use this for initialization
    private void Start ()
    {
	    sword = GetComponent<Sword>();
		ownerType = sword.Owner is UnitPlayer ? OwnerType.Player : OwnerType.AI;
		tipAudioSource = sword.TipTransform.GetComponent<AudioSource>();
	}
	
    private void SpawnParticles(Vector3 point)
    {
        Instantiate(Sparks, point, Quaternion.identity);
    }

	private void FixedUpdate()
	{
		return;
		if (movementSpeed < HighSwashSpeed)
		{
			//AdjustPitch(MidSwashSpeed, HighSwashSpeed);
			tipAudioSource.PlayOneShot(lesserSwishes[Random.Range(0, lesserSwishes.Length)], Random.Range(.45f, .65f));
		}
		else
		{
			//AdjustPitch(HighSwashSpeed, MaxSwashSpeed);
			tipAudioSource.PlayOneShot(greaterSwishes[Random.Range(0, greaterSwishes.Length)], Random.Range(.65f, .8f));
		}
	}

	private void AdjustPitch(float min, float max)
	{
		tipAudioSource.pitch = .85f + movementSpeed / (max - min) * .3f;
	}

	 private void Vibrate(bool hard = false)
	 {
		 if (steamVrObject.attachedToHand == null)
			 return;
		 var source = steamVrObject.attachedToHand.handType;
		 
		 var duration = 0.5f;
		 var frequency = 150f;
		 var amplitude = 75f;
		 haptics.Execute(0, duration, frequency, amplitude, source);
	 }

	private void OnCollisionEnter(Collision other)
	{
		// if (lesserRings.Length > 0)
		// {
		//         var magnitude = other.relativeVelocity.magnitude;
		//         if (magnitude > 1f && magnitude < 6.4f)
		//         {
		// 	        AudioSource.PlayClipAtPoint(lesserRings[Random.Range(0, lesserRings.Length)], trfm.position);
		// 	        Vibrate();
		//         }
		//         else if (magnitude >= 6.4f)
		//         {
		// 	        SpawnParticles(other.contacts[0].point + other.contacts[0].normal * 0.01f);
		// 	        AudioSource.PlayClipAtPoint(greaterRings[Random.Range(0, greaterRings.Length)], trfm.position);
		// 	        Vibrate(true);
		//         }
	 //        }
        }


	// private void OnCollisionExit(Collision other)
    // {
    //     if (other.gameObject.CompareTag("Player"))
		  //   return;
    //     // isTouching = false;
    // }
}
