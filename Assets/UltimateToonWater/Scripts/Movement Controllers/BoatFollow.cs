using UnityEngine;
using System.Collections;

public class BoatFollow : MonoBehaviour {

	public UltimateToonWaterC UTWC;
	public UltimateToonWater UTW;
	public SimpleBoatController boatController;

	// The target we are following
	private Transform target;
	// The distance in the x-z plane to the target
	public float distance = 10.0f;
	// the height we want the camera to be above the target
	public float height = 5.0f;
	public float offset = 10f;
	// How much we dampen
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	
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

	void Start() {
		target = boatController.transform;
	}
		
	void LateUpdate () {
		// Early out if we don't have a target
		if (!target) {
			return;
		}
		
		// Litte camera effect to enhance feeling of speed
		GetComponent<Camera>().fieldOfView = Mathf.Min (70f,50f + (20f*Mathf.Sqrt(Mathf.Max(0.001f,boatController.speed.z)/boatController.topSpeed)));
		
		// Calculate the current rotation angles
		float wantedRotationAngle = target.eulerAngles.y;
		float wantedHeight = target.position.y + height;
		
		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		
		// Convert the angle into a rotation
		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		Vector3 transPos = target.position;
		transPos-= currentRotation * Vector3.forward * distance;
		
		// Set the height of the camera
		if (UTW != null) {
			transPos.y = Mathf.Max(currentHeight,UTW.getHeightByPos(transPos)+3f);
		} else if (UTWC != null) {
			transPos.y = Mathf.Max(currentHeight,UTWC.getHeightByPos(transPos)+3f);
		}
		transform.position = transPos;
		
		// Always look at the target
		transform.LookAt (target.TransformPoint(new Vector3(0f,0f,offset)));
	}
}
