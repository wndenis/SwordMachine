using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

/*
 * TODO: MAXIMUM ATTACKS/ACTIONS PER TIME (e.g. guarantee idle once a 3 seconds )
 */
public class UnitAI : PlayableUnit
{
	public bool isActive = true;
	[Header("Visual/audio properties")] 
	[Space(5)]
	public AudioClip[] EnemyHurtSounds;
	public GameObject EnemyHurtParticles;
	public AudioClip[] EnemyFootStepSounds;
	
	private int FootStepOffset = -1;
	private AudioSource audioSource;
	private Vector3 startingPos;

	


	protected override void Start ()
	{
		base.Start();
		// Sword.Owner = this;
		// Sword.colliders.gameObject.SetActive(false);
		audioSource = GetComponent<AudioSource>();
		
		//todo: god mode
		MaxHealth = 10000;
		Health = MaxHealth;

		startingPos = transform.position;
	}

	private void FixedUpdate()
	{
		if (!isActive)
			return;
		UpdateVelocity();
	}

	//=========================================================================================================
//	protected override void PickupSword(Sword sword)
//	{
//		base.PickupSword(sword);
//	}

//	protected override void DropSword()
//	{
//		GestureCapture.Sword = null;
//		base.DropSword();
//	}


	protected override void SwordCollisionEnter(Collision other)
	{
		base.SwordCollisionEnter(other);
	}

	protected override void OtherUnitTouchHandler(PlayableUnit otherPlayableUnit, Vector3 point, float damage)
	{
		// if (!selfDamage && (otherPlayableUnit == this || otherPlayableUnit.CompareTag(tag)))
		// 	return;
		if (!Sword.IsReady())
			return;
		Sword.Delay();
		base.OtherUnitTouchHandler(otherPlayableUnit, point, damage);
		otherPlayableUnit.DamageEffects(point, damage);
	}

	protected override void OtherSwordTouchHandler(Sword otherSword, Vector3 point)
	{
		base.OtherSwordTouchHandler(otherSword, point);
		if (!otherSword.Owner)
			return;
	}

	public override void Die()
	{
		base.Die();
		var ed = GetComponent<SmartEnemyDriver>();
		ed.animator.enabled = false;
		var pm = GetComponent<PuppetMaster>();
		pm.internalCollisions = true;
		pm.Kill();
		Destroy(this);
		//Destroy(Sword.gameObject);
		//Destroy(ed);
		//Destroy(gameObject, 4);
		//TODO: DIE PARTICLES, LEVEL RESTART/SPAWN NEW ENEMY
	}

	public override void HealEffects(Vector3 position, float value)
	{
		
	}

	public override void DamageEffects(Vector3 position, float value)
	{
		if (false && !audioSource.isPlaying)
		{
			GameManager.Instance.RandPitch(audioSource);
			audioSource.PlayOneShot(EnemyHurtSounds[Random.Range(0, EnemyHurtSounds.Length)], Random.Range(.85f, 1.15f));
		}
		Instantiate(EnemyHurtParticles, position, Quaternion.identity);
	}

	private AudioClip FootStepCycle()
	{
		FootStepOffset++;
		FootStepOffset %= EnemyFootStepSounds.Length;
		return EnemyFootStepSounds[FootStepOffset];
	}
}
