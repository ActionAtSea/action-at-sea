////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - BulletFireScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletFireScript : MonoBehaviour
{
    public float fireTime = 0.05f;
    public float bulletSpeed = 100.0f;
    public Vector3 SpawnOffset = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector2 firingDirection;
    private SoundManager soundManager = null;

    void Start()
    {
        soundManager = SoundManager.Get();
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
        obj.GetComponent<Bullet>().Owner = owner;

        Vector2 bulletVelocity = transform.right * bulletSpeed;
        obj.GetComponent<Rigidbody2D>().AddForce(bulletVelocity);

        if(PlayerPlacer.IsCloseToPlayer(obj.transform.position))
        {
            soundManager.PlaySound(SoundManager.SoundID.FIRE);
        }
    }
}
