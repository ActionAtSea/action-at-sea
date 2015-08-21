////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Player.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{
    public float movementVelocity = 10.0f;
    public float turningAmount = 50.0f;
    public Vector2 maxVelocity = new Vector2(10.0f, 10.0f);
    private Rigidbody2D body2D = null;

    void Start () 
    {
        body2D = GetComponent<Rigidbody2D>();
    }
    
    void Update () 
    {
        if (Input.GetKey("a"))
        {
            transform.Rotate(Vector3.forward, turningAmount * Time.deltaTime);
        }

        if (Input.GetKey("d"))
        {
            transform.Rotate(Vector3.forward, -turningAmount * Time.deltaTime);
        }

        if (Input.GetKey("w"))
        {
            body2D.AddForce(transform.up * movementVelocity);
        }

        if (Input.GetKey("s"))
        {
            body2D.AddForce(transform.up * (-movementVelocity));
        }

        body2D.AddForce(transform.forward* movementVelocity);
    }
}
