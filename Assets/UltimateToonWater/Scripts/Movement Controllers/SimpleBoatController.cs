using UnityEngine;
using System.Collections;

public class SimpleBoatController : MonoBehaviour {
	public UltimateToonWater UTW;
	public UltimateToonWaterC UTWC;
	public Floater[] floaters;
	public bool canControl = false;
	public ParticleSystem trail;
	public float forwardForce = 500f;
	public float rotationSpeed = 1f;
	public float sideDrag = 10f;
	public float forwardDrag = 100f;
	public float downForce = 100f;
	public float waterDownDragY = 1.75f;
	public float maxUplift = 3f;
	
	private float vertical;
	private float horizontal;

	public Vector3 centerOfMass = Vector3.zero;

	public Vector3 speed;
	public float topSpeed;
	
	private ParticleSystem.Particle[] particles;
	
	public bool drawGizmos = true;

	[System.Serializable]
	public class Floater {
		public Transform buoyancePoint;
		public float buoyanceForce;
	}
	
	void Awake() {
		//Auto load if not set
		if (UTWC == null) {
			GameObject WaterC = GameObject.Find("WaterC");
			if (WaterC != null) {
				UTWC = WaterC.GetComponent<UltimateToonWaterC>();
			}
		}
		if (UTW == null) {
			GameObject Water = GameObject.Find("Water");
			if (Water != null) {
				UTW = Water.GetComponent<UltimateToonWater>();
			}
		}
	}

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().centerOfMass = centerOfMass;

	}

	void FixedUpdate () {
		// Do note, top speed is estimated!
		topSpeed = 1.025f * forwardForce / (GetComponent<Rigidbody>().mass*(forwardDrag/GetComponent<Rigidbody>().mass + GetComponent<Rigidbody>().drag));
		speed = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
		bool inWater = false;

		float totalForceFactor = 0f;
		foreach(Floater floater in floaters) {
			Vector3 actionPoint = floater.buoyancePoint.position;
			float targetYPos = 0f;
			if (UTW != null) {
			 	targetYPos = UTW.getHeightByPos(actionPoint);
			} else if (UTWC != null) {
				targetYPos = UTWC.getHeightByPos(actionPoint);
			}
			float forceFactor = targetYPos - actionPoint.y;
			forceFactor = Mathf.Min (forceFactor,maxUplift);
			
			float speedFactor = 0.5f+(speed.z/topSpeed);
			
			if (forceFactor > 0f) {
				totalForceFactor += forceFactor;
				float buoyance = forceFactor*forceFactor*floater.buoyanceForce;
				Vector3 uplift = -Physics.gravity * buoyance * speedFactor;
				uplift.y -= speed.y*forwardDrag;
				GetComponent<Rigidbody>().AddForceAtPosition(uplift, actionPoint);
				if (drawGizmos == true) {
					Debug.DrawLine(actionPoint,actionPoint+uplift/GetComponent<Rigidbody>().mass,Color.red);
				}
				inWater = true;
			} else if (forceFactor < 0f && forceFactor > -waterDownDragY) { // Water resistance stickyness
				float buoyance = (waterDownDragY-forceFactor)*floater.buoyanceForce;
				Vector3 uplift = Physics.gravity * buoyance *speedFactor;
				uplift.y -= speed.y*forwardDrag;
				GetComponent<Rigidbody>().AddForceAtPosition(uplift, actionPoint);
				
				if (drawGizmos == true) {
					Debug.DrawLine(actionPoint,actionPoint+uplift/GetComponent<Rigidbody>().mass,Color.yellow);
				}
				inWater = true;
			}
		}
		
		

		GetComponent<Rigidbody>().angularDrag = 1f + (totalForceFactor/3.5f);

		if (canControl == true && inWater == true) {
			GetComponent<Rigidbody>().AddRelativeForce(new Vector3(-speed.x*sideDrag,0f,0f));
			GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f,0f,-speed.z*forwardDrag));
		}
		GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f,-speed.z*downForce,0f));

		vertical = 0;
		horizontal = 0;

		vertical = Input.GetAxis("Vertical");
		horizontal = Input.GetAxis("Horizontal")*rotationSpeed;

		if (Input.GetKey(KeyCode.LeftShift)) {
			vertical *= 2f;
		}

		if (vertical < 0f) {
			vertical *= 0.1f;
		}

		if (inWater == false) {
			vertical *= 0.125f;
			horizontal *= 0.125f;
			if (trail.isPlaying == true) {
				trail.Stop();
			}
		} else {
			if (trail.isPlaying == false) {
				trail.Play();
			}
		}

	 	GetComponent<Rigidbody>().AddRelativeForce(vertical*forwardForce*Vector3.forward);	
		transform.Rotate(horizontal*Vector3.up);

		// Little boost for if we go slow
		if (vertical > 0) {
			vertical += 1f-Mathf.Max(0,speed.z)/topSpeed;
		}

		// Particle handling for the boat
		particles = new ParticleSystem.Particle[trail.particleCount];
		trail.GetParticles(particles);
		for(int i=0;i<particles.Length;i++) {
			ParticleSystem.Particle particle = particles[i];
			
			float h = 0f;
			if (UTW != null) {
				h = UTW.getHeightByPos(particle.position);
			} else if (UTWC != null) {
				h = UTWC.getHeightByPos(particle.position);
			}

			if (particle.position.y - h > 1f) {
				particles[i].lifetime = 0f;
			}
			particles[i].position = new Vector3(particle.position.x,h,particle.position.z);
		}
		trail.SetParticles(particles,particles.Length);
	}

	void OnGUI() {
		GUI.color = Color.black;
		GUI.Box(new Rect(Screen.width/2f-100f, 20, 200, 20), Mathf.RoundToInt(Mathf.Abs(speed.z)*3.6f) + "/" + Mathf.RoundToInt(topSpeed*3.6f) + "Km/h");
	}
	
	void OnDrawGizmos() {
		if (drawGizmos == true) {
			
			foreach(Floater f in floaters) {
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(f.buoyancePoint.position, 1);

				
			}
			
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(GetComponent<Rigidbody>().worldCenterOfMass,1);
			Gizmos.DrawLine(GetComponent<Rigidbody>().worldCenterOfMass,GetComponent<Rigidbody>().worldCenterOfMass+Physics.gravity);
		}
	}
}
