using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletFireScript : MonoBehaviour
{
    public float fireTime = 0.05f;
    public float bulletSpeed = 100.0f;
    public Vector3 SpawnOffset = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector2 firingDirection;
    private Rigidbody2D body2D;
	private SoundEffectHandler soundEffects;
    // Use this for initialization

    void Start()
    {
        body2D = GetComponent<Rigidbody2D>();
        //InvokeRepeating("Fire", fireTime, fireTime);

		soundEffects = FindObjectOfType<SoundEffectHandler>();
		if (!soundEffects)
		{
			Debug.Log("SoundEffectHandler could not be found in scene.");
		}
    }

    void Update()
    {

    }

	public Vector3 FirePosition()
	{
		return transform.position;
	}

	public Quaternion FireRotation()
	{
		return transform.rotation;
	}

    public void Fire(string owner, Vector3 firePosition, Quaternion fireRotation)
    {
        GameObject obj = NewObjectPooler.current.GetPooledObject();

        if (obj == null)
        {
            return;
        }

		obj.transform.position = firePosition;
		obj.transform.rotation = fireRotation;
        obj.transform.Translate(SpawnOffset);
        obj.SetActive(true);
		obj.GetComponent<Bullet>().owner = owner;

        //Vector2 bulletVelocity = transform.TransformDirection(Vector3.right * 100.0f);

        //Vector2 bulletVelocity = (firingDirection.normalized * bulletSpeed);
        Vector2 bulletVelocity = transform.right * bulletSpeed;

        //bulletVelocity += body2D.velocity;
        //Debug.Log(body2D.velocity);
        //obj.GetComponent<Rigidbody2D>().velocity = body2D.velocity;
        obj.GetComponent<Rigidbody2D>().AddForce(bulletVelocity);

		if(soundEffects.IsCloseToPlayer(obj.transform.position))
		{
			soundEffects.PlayOnFire();
		}
    }
}
