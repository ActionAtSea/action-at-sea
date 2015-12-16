using UnityEngine;
using System.Collections;

public class RogueAI : MonoBehaviour 
{
    private CannonController cannonController = null;
    private NavMeshAgent navAgent = null;
    private Rigidbody body = null;
    


    /// <summary>
    /// The maximum change of heading added in each update.
    /// </summary>
    [RangeAttribute(0, 100)]
    public float wanderChange = 10.0f;

    /// <summary>
    /// Radius of the wander sphere.
    /// </summary>
    [RangeAttribute(0, 100)]
    public float wanderRadius = 25.0f;

    /// <summary>
    /// The distance of the wander sphere in front of the agent.
    /// </summary>
    [RangeAttribute(0, 100)]
    public float wanderDistance = 80.0f;

    private float wanderTheta = 0.0f;
    private Vector3 circleCentre = Vector3.zero;
    private Vector3 previousWanderTarget = Vector3.zero;

	// Use this for initialization
	void Start () 
    {
        navAgent = GetComponent<NavMeshAgent>();
        body = GetComponent<Rigidbody>();
        cannonController = GetComponentInChildren<AICannonController>();
        if(navAgent != null)
        {
            if (navAgent.isOnNavMesh)
            {
                //Test destination to see if navigation works.
                navAgent.SetDestination(new Vector3(10, 1, 100));
            }
        }
	}
	
	// Update is called once per frame
	void Update() 
    {
        if (navAgent.isOnNavMesh)
        {
            navAgent.SetDestination(Wander());
        }
	}

    Vector3 Wander()
    {
        // Get a random value between -wanderChange and wanderChange. 
        float randomValue = Random.Range(-wanderChange, wanderChange);
        wanderTheta += randomValue;

        //Calculating the new location to steer towards on the circle
        if(navAgent.velocity.magnitude != 0.0f)
        {
            circleCentre = navAgent.velocity.normalized * wanderDistance + transform.position;
        }
        else
        {
            /*
            If the agent isn't already moving this ensures the wander circle is placed relative
            to the agent's starting position.
            */
            circleCentre = Vector3.forward * wanderDistance + transform.position;
        }
        //angle of the velocity vector along the x and z axes relative to 0.
        float h = Vector2.Angle
            (new Vector2(navAgent.velocity.x, navAgent.velocity.z), 
             new Vector2(transform.position.x, transform.position.z));

        Vector2 circleOffset = new Vector2(wanderRadius*Mathf.Cos(wanderTheta * h), wanderRadius*Mathf.Sin(wanderTheta + h));
        Vector3 target = new Vector3(circleCentre.x + circleOffset.x, 0, circleCentre.z+circleOffset.y);
        return target;
    }
}
