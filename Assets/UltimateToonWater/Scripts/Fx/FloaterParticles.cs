using UnityEngine;
using System.Collections;

public class FloaterParticles : MonoBehaviour {

	public UltimateToonWaterC UTWC;
	public UltimateToonWater UTW;
	
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
	
	}

	private ParticleSystem.Particle[] particles;

	// Update is called once per frame
	void Update () {
		particles = new ParticleSystem.Particle[GetComponent<ParticleSystem>().particleCount];
		GetComponent<ParticleSystem>().GetParticles(particles);
		for(int i=0;i<particles.Length;i++) {
			ParticleSystem.Particle particle = particles[i];
			float h = 0;
			if (UTW != null) {
				h = UTW.getHeightByPos(particle.position);
			} else if (UTWC != null) {
				h = UTWC.getHeightByPos(particle.position);
			}
			if (particle.position.y - h > 2f) {
				particles[i].lifetime = 0f;
			}
			particles[i].position = new Vector3(particle.position.x,h,particle.position.z);
		}
		GetComponent<ParticleSystem>().SetParticles(particles,particles.Length);
	}
}
