using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour 
{
	public GameObject gameboard;
	public GameObject terrain;
	private float gameboardOffset = 20.0f;

	public float maxForwardSpeed = 8.0f;
	public float minForwardSpeed = 0.0f;
	public float minRotateTime = 2.0f;
	public float maxRotateTime = 3.0f;
	
	private float rotationTime = 0.0f;
	private float rotationTimePassed = 0.0f;
	private float rotateSpeed = 1.0f;
	private float minforwardTime = 2.0f;
	private float maxforwardTime = 4.0f;
	private float forwardTimePassed = 0.0f;
	private float forwardTime = 0.0f;
	private bool movingForward = true;
	private bool rotatingLeft = false;
	private Vector3 forwardForce = new Vector3();
	private SoundEffectHandler soundEffects;

	/**
	* Places the enemy randomly within the game board
	*/
	void Start () 
	{
		soundEffects = FindObjectOfType<SoundEffectHandler>();
		if (!soundEffects)
		{
			Debug.Log("SoundEffectHandler could not be found in scene.");
		}

		bool foundPosition = false;
		float x = 0.0f;
		float y = 0.0f;

		var boardBounds = gameboard.GetComponent<SpriteRenderer> ().bounds;
		while (!foundPosition) 
		{
			foundPosition = true;
			x = Random.Range (-boardBounds.extents.x + gameboardOffset, boardBounds.extents.x - gameboardOffset);
			y = Random.Range (-boardBounds.extents.y + gameboardOffset, boardBounds.extents.y - gameboardOffset);

			var islands = terrain.GetComponentsInChildren<SpriteRenderer>();
			for(int i = 0; i < islands.Length; ++i)
			{
				var islandBounds = islands[i].bounds;
				if(x > islandBounds.center.x - islandBounds.extents.x &&
				   x < islandBounds.center.x + islandBounds.extents.x &&
				   y > islandBounds.center.y - islandBounds.extents.y &&
				   y < islandBounds.center.y + islandBounds.extents.y)
				{
					foundPosition = false;
					break;
				}
			}
		}

		float rotate = Random.Range (0.0f, 360.0f);
		transform.position = new Vector3 (x, y, transform.position.z);
		transform.Rotate (new Vector3 (0, 0, rotate));

		forwardTime = Random.Range (minforwardTime, maxforwardTime);
	}

	/**
	* Determines if the enemy is about to hit an island or the game board
	*/
	bool CloseToCollision()
	{
		var forwardLook = 5.0f;
		var forwardPoint = transform.position + (transform.up * forwardLook);

		Vector3 collisionPoint = new Vector3 ();
		collisionPoint.x = forwardPoint.x;
		collisionPoint.y = forwardPoint.y;

		var islands = terrain.GetComponentsInChildren<SpriteRenderer>();
		for(int i = 0; i < islands.Length; ++i)
		{
			var islandBounds = islands[i].bounds;
			collisionPoint.z = islandBounds.center.z;

			if(islandBounds.Contains(collisionPoint))
			{
				return true;
			}
		}

		forwardLook += gameboardOffset;
		forwardPoint = transform.position + (transform.up * forwardLook);

		var boardBounds = gameboard.GetComponent<SpriteRenderer> ().bounds;
		collisionPoint.x = forwardPoint.x;
		collisionPoint.y = forwardPoint.y;
		collisionPoint.z = boardBounds.center.z;

		if(!boardBounds.Contains(collisionPoint))
		{
			return true;
		}	

		return false;
	}

	/**
	* Moves the enemy randomly
	*/
	void Update () 
	{
		var rb = GetComponent<Rigidbody2D> ();

		if(movingForward)
		{
			float forwardSpeed = Random.Range (minForwardSpeed, maxForwardSpeed);
			forwardForce.x = transform.up.x * forwardSpeed;
			forwardForce.y = transform.up.y * forwardSpeed;
			rb.AddForce(forwardForce);

			forwardTimePassed += Time.deltaTime;
			if (CloseToCollision() || forwardTimePassed >= forwardTime)
			{
				forwardTimePassed = 0.0f;
				movingForward = false;
			    rotationTime = Random.Range(minRotateTime, maxRotateTime);
				forwardTime = Random.Range (minforwardTime, maxforwardTime);
				rotatingLeft = Vector2.Dot(Random.insideUnitCircle, Vector2.up) < 0.0f;
			}
		}
		else
		{
			rb.AddTorque(rotatingLeft ? rotateSpeed : -rotateSpeed);

			rotationTimePassed += Time.deltaTime;
			if (rotationTimePassed >= rotationTime)
			{
				rotationTimePassed = 0.0f;
				movingForward = true;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			soundEffects.PlayOnRam();
		}
	}
}
