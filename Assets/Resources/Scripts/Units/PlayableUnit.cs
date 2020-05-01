using System;
using JetBrains.Annotations;
using Resources.Scripts;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class PlayableUnit : MonoBehaviour, IDamageable
{
	[Header("Unit properties")]
	public string UnitName;
	
	[SerializeField]
	private bool _isAlive = true;
	public bool IsAlive
	{
		get { return _isAlive;}
		protected set { _isAlive = value; }
	}

	[SerializeField]
	private Sword _sword;
	public Sword Sword
	{
		get { return _sword; }
		protected set {
			_sword = value;
			_sword.Owner = this;
		}
	}

	[SerializeField]
	private float _health = 15;
	public float Health
	{
		get { return _health;}
		protected set { _health = value;}
	}
	
	[SerializeField]
	private float _maxHealth = 15;
	public float MaxHealth
	{
		get { return _maxHealth; }
		protected set { _maxHealth = value; }
	}

	public Vector3 OldPos;
	public Vector3 Velocity;
	
	/// <summary>
	/// Fixes health/maxhealth in editor
	/// </summary>
	private void OnValidate()
	{
		_maxHealth = Mathf.Max(1, _maxHealth);
		_health = Mathf.Max(1, _health);
		if (_health > _maxHealth)
			_health = _maxHealth;

		var requiredCollider = GetComponentInChildren<Collider>() ?? GetComponent<Collider>();
		if (requiredCollider) return;
		//Debug.LogError("GDE KOLLAIDER ALO");
	}

	public virtual void PickupSword(Sword sword)
	{
		Sword = sword;
	}
	public virtual void DropSword()
	{
		Sword.Owner = null;
		Sword = null;
	}

	public Transform HeadTransform;
	public Transform BodyTransform;
	

	// Use this for initialization
	protected virtual void Start()
	{
		IsAlive = true;
		OldPos = transform.position;
		//TODO fix runtime sword switch => PickupSword
		if (Sword){Sword.HitCallbackMethod = SwordCollisionEnter;}
			
	}

	protected void UpdateVelocity()
	{
		Velocity = transform.position - OldPos;
		OldPos = transform.position;
	}
	
	/// <summary>
	/// Gets concrete object we collided with
	/// </summary>
	protected GameObject GetCollidedObject(Collision other)
	{
		return other.contacts?[0].otherCollider.gameObject;
	}

	protected Vector3 GetCollisionPoint(Collision other)
	{
		return other.contacts?[0].point ?? transform.position;
	}
	
	/// <summary>
	/// Calls different handlers for different collided objects
	/// </summary>
	protected virtual void SwordCollisionEnter(Collision other)
	{
		//print(gameObject.name + ": " + Sword.name + " hitted " + other.gameObject.name);
		
		/*
		 * Ищет компонент PlayableUnit и пытается нанести ему урон
		 * Вызывает callback владельца
		 */
		var collidedObject = GetCollidedObject(other);
		var collisionPoint = GetCollisionPoint(other);
		
		//TODO: fix hierarchy
		// hit with unit
		var otherSword = collidedObject.GetComponent<Sword>()??collidedObject.GetComponentInParent<Sword>();
		if (otherSword)
		{
			//print($"{UnitName}: other sword");
			OtherSwordTouchHandler(otherSword, collisionPoint);
			return;
		}
		
		var otherUnit = collidedObject.GetComponent<PlayableUnit>()??collidedObject.GetComponentInParent<PlayableUnit>();
		if (otherUnit)
		{
			//print($"{UnitName}: other unit");
			OtherUnitTouchHandler(otherUnit, collisionPoint, Sword.Damage);
			return;
		}

		//print($"{UnitName}: default");
		DefaultTouchHandler(other);
		
		//try to get Sword -> Playable unit

		//Collision with:
		//sword => (ai: block routines, player: penetration prevention)
		//body(not self) => (deal damage, ai: block routines, player: penetration prevention)
		//other (rigid) => (player: PP)
		//other (non-rigid) => ?
	}

	/// <summary>
	/// Handles sword->unit collision
	/// </summary>
	/// <param name="otherPlayableUnit">Collided unit</param>
	/// <param name="point">Point of collision</param>
	/// <param name="damage">Damage to apply to that unit</param>
	protected virtual void OtherUnitTouchHandler(PlayableUnit otherPlayableUnit, Vector3 point, float damage)
	{
		//TODO: fix copypasting of self damage check
		otherPlayableUnit.DeltaHealth(point, damage);
	}

	/// <summary>
	/// Handles sword->sword collision
	/// </summary>
	/// <param name="otherSword">Collided sword</param>
	/// <param name="point">Point of collision</param>
	protected virtual void OtherSwordTouchHandler(Sword otherSword, Vector3 point)
	{
		
	}

	/// <summary>
	/// Handles any other non-specified collisions e.g. walls, floor etc.
	/// </summary>
	/// <param name="other">Context</param>
	protected virtual void DefaultTouchHandler(Collision other)
	{
		
	}


	protected void DeltaHealth(Vector3 point, float value)
	{
		if (value <= 0)
			DamageEffects(point, value);
		else
			HealEffects(point, value);
		
		Health += value;
		
		if (Health > MaxHealth)
			Health = MaxHealth;

		if (Health <= 0)
			Die();
	}

	public virtual void Die()
	{
		IsAlive = false;
		Sword.Owner = null;
	}

	//TODO: replace this effects with abstract-inherited effects to incapsulate calls
	public abstract void HealEffects(Vector3 position, float value);
	public abstract void DamageEffects(Vector3 position, float value);
}
