using UnityEngine;
using System.Collections;

public class FloatingObject : MonoBehaviour {
	public UltimateToonWaterC UTWC;
	public UltimateToonWater UTW;
	
	public FloaterParticles floaterParticles;
	
	public Floater[] floaters;
	public float particleSize = 2f;
	
	public bool isActive = true;
	
	public bool sticky = false;
	public bool emitParticles = true;
	public float emitRate = 1f;
	public float waterDownDragY = 1f;
	
	public Vector3 centerOfMass;
	
	private Vector3 origin;
	
	private bool waitForParticle = false;
	
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
		if (floaterParticles == null) {
			GameObject FloaterP = GameObject.Find("FloaterParticles");
			if (FloaterP != null) {
				floaterParticles = FloaterP.GetComponent<FloaterParticles>();
			}
		}
	}
	
	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().centerOfMass = centerOfMass;
		origin = transform.localPosition;
		
	}

	
	// Update is called once per frame
	void FixedUpdate () {
		if (isActive == true && (UTW != null || UTWC != null)) {
			foreach(Floater floater in floaters) {
				Vector3 actionPoint = floater.buoyancePoint.position;
				
				float targetYPos = 0;
				if (UTW != null) {
					targetYPos = UTW.getHeightByPos(actionPoint);
				} else if (UTWC != null) {
					targetYPos = UTWC.getHeightByPos(actionPoint);
				}
				
				float forceFactor = targetYPos - actionPoint.y;
				forceFactor = Mathf.Min (forceFactor,2.25f);
				
				if (forceFactor > 0f) {
					float buoyance = forceFactor*forceFactor*floater.buoyanceForce;
					Vector3 uplift = -Physics.gravity * buoyance;
					GetComponent<Rigidbody>().AddForceAtPosition(uplift, actionPoint);
					if (drawGizmos == true) {
						Debug.DrawLine(actionPoint,actionPoint+uplift/GetComponent<Rigidbody>().mass,Color.red);
					}
				} else if (forceFactor < 0f && forceFactor > -waterDownDragY) { // Water resistance drag
					float buoyance = (waterDownDragY-forceFactor)*floater.buoyanceForce;
					Vector3 uplift = Physics.gravity * buoyance;
					GetComponent<Rigidbody>().AddForceAtPosition(uplift, actionPoint);
					if (drawGizmos == true) {
						Debug.DrawLine(actionPoint,actionPoint+uplift/GetComponent<Rigidbody>().mass,Color.yellow);
					}
				}
			}
			
			Vector3 pseudoPos = new Vector3(origin.x,transform.localPosition.y,origin.z);
			if (sticky == true) {
				Vector3 newPos = Vector3.Lerp(transform.localPosition,pseudoPos,Time.deltaTime*1f);
				transform.localPosition = newPos;
			}
			
			
			if (emitParticles == true) {
				if (floaterParticles != null) {
					StartCoroutine(particleEmit());
				}
			}
		}
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
	
	IEnumerator particleEmit() {
		if (waitForParticle == false) {
			waitForParticle = true;
			floaterParticles.GetComponent<ParticleSystem>().Emit(transform.position,Vector3.zero,particleSize,3f,Color.white);
			
			yield return new WaitForSeconds(1f/emitRate);
			waitForParticle = false;
		}
	}
}