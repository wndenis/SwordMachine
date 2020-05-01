//Marshall Heffernan aka itsmars 2015
//Special thanks to Duck, TheKusabi, alucardj, dorpeleg, TonyLi & superpig on the Unity Forums!

//This is the CURVES version of my Footstep script. For this to work, you'll first need to...

//1) Add two floats to your Animator Controller. "FootstepLeft" and "FootstepRight".
//2) Add a CURVE to each footstep animation. See video tutorial for a better explaination.
//3) Tag your Terrain GameObject with "Terrain".
//4) Tag your Non-Terrain GameObject floors with their respective names (for example, a wooden floor would need the tag "Surface_Wood").

//Watch the video tutorial! https://www.youtube.com/watch?v=ISoBKFxQLic

// Refactoring by wndenis 2020

using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
	//This will add an Audio Source component if you don't have one.

public class FootstepMaster_Curves : MonoBehaviour {
	private AudioSource mySound;		//The AudioSource component
	private Animator anim;				//Lets us pull floats out of our Animator's curves.
	public static GameObject floor;		//what are we standing on?
	private string currentFoot;			//Each foot does it's own Raycast

	private float currentFrameFootstepLeft;
	private float currentFrameFootstepRight;
	private float lastFrameFootstepLeft;
	private float lastFrameFootstepRight;

	//-----------------------------------------------------------------------------------------

	[Space(5.0f)]
	private float currentVolume;
	[Range(0.0f,1.0f)]
	public float volume = 1.0f;				//Volume slider bar; set this between 0 and 1 in the Inspector.
	[Range(0.0f,0.2f)]
	public float volumeVariance = 0.04f;	//Variance in volume levels per footstep; set this between 0.0 and 0.2 in the inspector. Default is 0.04f.
	private float pitch;
	[Range(0.0f,0.2f)]
	public float pitchVariance = 0.08f;		//Variance in pitch levels per footstep; set this between 0.0 and 0.2 in the inspector. Default is 0.08f.
	[Space(5.0f)]
	public GameObject leftFoot;			//Drag your player's RIG/MESH/BIP/BONE for the left foot here, in the inspector.
	public GameObject rightFoot;		//Drag your player's RIG/MESH/BIP/BONE for the right foot here, in the inspector.
	[Space(5.0f)]
	public AudioClip[] defaults = new AudioClip[0];

	[Space(5f)] 
	public LayerMask GroundLayers;

	private float midPitch;

	private static readonly int FootstepLeft = Animator.StringToHash("FootstepLeft");
	private static readonly int FootstepRight = Animator.StringToHash("FootstepRight");
	private static readonly int Forward = Animator.StringToHash("Forward");
	//-----------------------------------------------------------------------------------------

	//Start
	void Start () {
		anim = GetComponent<Animator>();
		mySound = GetComponent<AudioSource>();
		midPitch = mySound.pitch;
	}

	//-----------------------------------------------------------------------------------------

	//Update
	void Update () {
		if (Mathf.Abs(anim.GetFloat(Forward)) < 0.005f)
			return;

		currentFrameFootstepLeft = anim.GetFloat(FootstepLeft);
		currentFrameFootstepRight = anim.GetFloat(FootstepRight);
		var groundTouched = false;
		var rayOrigin = Vector3.zero;
		if (currentFrameFootstepLeft > 0 && lastFrameFootstepLeft < 0)
		{
			groundTouched = true;
			rayOrigin = leftFoot.transform.position;
		}
		if (currentFrameFootstepRight < 0 && lastFrameFootstepRight > 0)
		{
			groundTouched = true;
			rayOrigin = rightFoot.transform.position;
		}

		lastFrameFootstepLeft = currentFrameFootstepLeft;
		lastFrameFootstepRight = currentFrameFootstepRight;

		if (!groundTouched)
			return;

		var ray = new Ray(rayOrigin + new Vector3(0, 1.5f, 0), Vector3.down);
		if (Physics.Raycast(ray, out var surfaceHit, 2f, GroundLayers))
		{
			if (surfaceHit.transform.gameObject != null)
				PlayDefault();
		}
	}

	void PlayDefault(){
		currentVolume = volume + Random.Range(-volumeVariance, volumeVariance);
		pitch = midPitch + Random.Range(-pitchVariance, pitchVariance);
		mySound.pitch = pitch;
		if (defaults.Length > 0) {
			mySound.PlayOneShot (defaults [Random.Range (0, defaults.Length)], currentVolume);
		} else Debug.LogError ("trying to play the default sound, but no default sounds in array!");
	}
	}