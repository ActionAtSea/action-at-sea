using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{

    //Variables
    public float movementVelocity = 10.0f;
    public float turningAmount = 50.0f;
    public Vector2 maxVelocity = new Vector2(10.0f, 10.0f);

    private Rigidbody2D body2D;

    private Vector3 mousePos;
    

    // Use this for initialization
	void Start () 
    {
        body2D = GetComponent<Rigidbody2D>();
        //fireScript = GetComponent<BulletFireScript>();
        
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
	
	// Update is called once per frame
	void Update () 
    {
        UpdateMousePos();
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

        //Debug.Log(Vector2.Angle(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero));
        //Debug.Log(mousePos.normalized);
       // TODO: remove this. fireScript.AimWeapon(mousePos);
        
	}

    private void UpdateMousePos()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    }
}
