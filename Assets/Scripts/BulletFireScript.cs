////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - BulletFireScript.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletFireScript : MonoBehaviour
{
    public Vector3 SpawnOffset = new Vector3(0.0f, 0.0f, 0.0f);

    private float m_bulletSpeed = 200.0f;
    private Vector2 m_firingDirection;

    /**
    * Returns the position fired from
    */
    public Vector3 FirePosition()
    {
        return transform.position;
    }

    /**
    * Returns the rotation fired from
    */
    public Quaternion FireRotation()
    {
        return transform.rotation;
    }

    /**
    * Fires a bullet
    */
    public void Fire(string owner, Vector3 firePosition, Quaternion fireRotation)
    {
        GameObject obj = NewObjectPooler.Get().GetPooledObject();

        if (obj == null)
        {
            Debug.LogError("Could not generate bullet from pooler");
            return;
        }

        obj.transform.position = firePosition;
        obj.transform.rotation = fireRotation;
        obj.transform.Translate(SpawnOffset);
        obj.SetActive(true);
        obj.GetComponent<Bullet>().Owner = owner;

        Vector2 bulletVelocity = transform.right * m_bulletSpeed;
        obj.GetComponent<Rigidbody2D>().AddForce(bulletVelocity);

        if(PlayerPlacer.IsCloseToPlayer(obj.transform.position))
        {
            SoundManager.Get().PlaySound(SoundManager.SoundID.FIRE);
        }
    }
}
