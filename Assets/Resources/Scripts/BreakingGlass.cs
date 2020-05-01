using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingGlass : MonoBehaviour
{
	private int health = 3;
	public float hitTolerance;
	public float hitDelay = 2f;
	private float lastHitTime;
	public GameObject BrokenGlass;
	public GameObject DispatchedGlass;
	public Transform GlassParticles;
	public Rigidbody CoveredSword;


	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("PlayerItem"))
		{
			if (Time.time - lastHitTime > hitDelay)
			{
				var vel = other.gameObject.GetComponent<Rigidbody>().velocity;
				var velPos = other.transform.position;
				var targetPos = transform.position;

				var bestVector = targetPos - velPos;
				var angle = Vector3.Angle(vel, bestVector);
				if (angle < 90)
				{
					Hit(other.contacts[0].point);
				}
			}
				
		}
	}

	void Hit(Vector3 pos)
	{
		print("hit");
		lastHitTime = Time.time;
		Instantiate(GlassParticles, pos, Quaternion.identity);
		health -= 1;
		

		if (health == 0)
		{
			CoveredSword.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			BrokenGlass.SetActive(true);
			DispatchedGlass.SetActive(true);
			gameObject.SetActive(false);
		}
	}

	// Use this for initialization
	void Start ()
	{
		CoveredSword.constraints = RigidbodyConstraints.FreezeAll;
		CoveredSword.GetComponent<Rigidbody>().isKinematic = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
