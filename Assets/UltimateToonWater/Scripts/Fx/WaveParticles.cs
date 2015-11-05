using UnityEngine;
using System.Collections;

public class WaveParticles : MonoBehaviour {

	public UltimateToonWaterC UTWC;
	public UltimateToonWater UTW;
	
	public float radius = 100f;
	
	public Vector2 velocity = Vector3.zero;
	public Vector2 size = new Vector2(5f,15f);
	public Vector2 filetime = new Vector2(1f,2.5f);
	
	public bool snapToWater = true;
	
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
	
	// Update is called once per frame
	void Update () {
		int particleDiff = GetComponent<ParticleSystem>().maxParticles - GetComponent<ParticleSystem>().particleCount;
		
		if (snapToWater == true) {
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[GetComponent<ParticleSystem>().particleCount];
			GetComponent<ParticleSystem>().GetParticles(particles);
			
			for(int i=0;i<particles.Length;i++) {
				ParticleSystem.Particle particle = particles[i];
				if (UTW != null) {
					particles[i].position = new Vector3(particle.position.x,UTW.getHeightByPos(particle.position),particle.position.z);
				} else if (UTWC != null) {
					particles[i].position = new Vector3(particle.position.x,UTWC.getHeightByPos(particle.position),particle.position.z);
				}
			}
			
			GetComponent<ParticleSystem>().SetParticles(particles,particles.Length);
		}
		
		if (particleDiff > 0) {
			Vector3 campos = Camera.main.transform.position;
			for (int i = 0; i < particleDiff; i++ ) {

				Vector2 randPos = Random.insideUnitCircle.normalized * Random.Range(100f,radius/2f);
				Vector2 posC = randPos + new Vector2(campos.x,campos.z);
				Vector3 pos = Vector3.zero;
				if (UTW != null) {
					pos = new Vector3(posC.x, UTW.getHeightByPos(posC),posC.y);
				} else if (UTWC != null) {
					pos = new Vector3(posC.x, UTWC.getHeightByPos(posC),posC.y);
				}
				
				GetComponent<ParticleSystem>().Emit(pos,new Vector3(0,Random.Range(velocity.x,velocity.y),0),Random.Range(size.x,size.y),Random.Range(filetime.x,filetime.y),GetComponent<ParticleSystem>().startColor);
			}
		}
	}
}
