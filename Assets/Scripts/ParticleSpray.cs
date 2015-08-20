using UnityEngine;
using System.Collections;

public class ParticleSpray : MonoBehaviour 
{	
	public float minSpeedForEmission = 1.0f;
	public GameObject parent;
	private ParticleSystem particles;

	void Start () 
	{
		particles = GetComponent<ParticleSystem> ();
		particles.GetComponent<Renderer>().sortingLayerName = "World";
		particles.GetComponent<Renderer>().sortingOrder = parent.GetComponent<SpriteRenderer>().sortingOrder - 1;
	}

	void Update () 
	{
		particles.enableEmission = parent.GetComponent<Rigidbody2D>().velocity.magnitude >= minSpeedForEmission;
	}
}
