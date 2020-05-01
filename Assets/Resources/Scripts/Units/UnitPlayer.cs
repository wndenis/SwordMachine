using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlayer : PlayableUnit
{
	public Transform PlayerHurtParticles;
	public AudioClip PlayerHurtSound;
	public Transform HeadPosDebug;
	
	private AudioSource _audioSource;
	

	protected override void Start()
	{
		_audioSource = GetComponent<AudioSource>();
		base.Start();
	}

	private void FixedUpdate()
	{
		UpdateVelocity();
	}

	protected override void OtherUnitTouchHandler(PlayableUnit otherPlayableUnit, Vector3 point, float damage)
	{
		GameManager.Instance.AddScore();
		base.OtherUnitTouchHandler(otherPlayableUnit, point, damage);
	}

	protected override void OtherSwordTouchHandler(Sword otherSword, Vector3 point)
	{
		print("ENABLE NONPENETRATION");
		base.OtherSwordTouchHandler(otherSword, point);
	}

	public override void Die()
	{
		print("you lose, lol");
		base.Die();
	}

	public override void HealEffects(Vector3 position, float value)
	{
		throw new System.NotImplementedException();
	}

	public override void DamageEffects(Vector3 position, float value)
	{
		var blood = Instantiate(PlayerHurtParticles, HeadPosDebug.position + HeadPosDebug.forward * .2f, Quaternion.identity);
		if (!_audioSource.isPlaying)
			_audioSource.PlayOneShot(PlayerHurtSound, Random.Range(.65f, 1.05f));
	}
	
	
}
