using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour 
{
	public bool controllable = false;
	public float forwardSpeed = 10.0f;
	public float rotationSpeed = 1.0f;
	private Vector3 forwardForce = new Vector3();
	private SoundEffectHandler soundEffects;

	void Start()
	{
		soundEffects = FindObjectOfType<SoundEffectHandler>();
		if (!soundEffects)
		{
			Debug.Log("SoundEffectHandler could not be found in scene.");
		}

	}

	void FixedUpdate() 
	{
		if(GameInformation.IsPVP() && !controllable)
		{
			return;
		}

		var rb = GetComponent<Rigidbody2D> ();

		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow))
		{
			forwardForce.x = transform.up.x * forwardSpeed;
			forwardForce.y = transform.up.y * forwardSpeed;
			rb.AddForce(forwardForce);
		}
		if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow))
		{
			rb.AddTorque(rotationSpeed);
		}
		if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow))
		{
			rb.AddTorque(-rotationSpeed);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "EnemyPlayer")
		{
			if(soundEffects.IsCloseToPlayer(other.transform.position))
			{
				soundEffects.PlayOnRam();
			}
		}
	}
}
