using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
	public float dragAmount = 0.15f;
	public bool useCameraDrag = true;
	private Vector3 velocity = Vector3.zero;
	private Vector3 fixedPosition;
	private Vector3 viewportPosition;
	private GameObject target = null;

	void Start ()
	{
		viewportPosition = new Vector3 (0.5f, 0.5f, 0.0f);
		fixedPosition = new Vector3 (0.0f, 0.0f, transform.position.z);
	}

	void FixedUpdate ()
	{
		if(target == null)
		{
			target = GameObject.FindWithTag("Player");
			return;
		}

		if(useCameraDrag)
		{
			// Reference: http://answers.unity3d.com/questions/29183/2d-camera-smooth-follow.html
			var camera = GetComponent<Camera> ();
			viewportPosition.z = camera.WorldToViewportPoint(target.transform.position).z;
			Vector3 delta = target.transform.position - camera.ViewportToWorldPoint(viewportPosition);
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dragAmount);
		}
		else
		{
			fixedPosition.x = target.transform.position.x;
			fixedPosition.y = target.transform.position.y;
			transform.position = fixedPosition;
		}
	}
}
