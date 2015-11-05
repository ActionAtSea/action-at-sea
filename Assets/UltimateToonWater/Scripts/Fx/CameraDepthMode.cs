using UnityEngine;
using System.Collections;

public class CameraDepthMode : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}

}
