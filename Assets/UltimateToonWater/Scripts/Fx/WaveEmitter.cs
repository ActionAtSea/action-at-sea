using UnityEngine;
using System.Collections;

public class WaveEmitter : MonoBehaviour {

	public UltimateToonWaterC UTWC;
	public UltimateToonWaterC UTW;
	public ParticleSystem waveParticles;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void EmitWaves() {
		int particleDiff = waveParticles.maxParticles -waveParticles.particleCount;
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[waveParticles.particleCount];
		waveParticles.GetParticles(particles);
		for(int i=0;i<particles.Length;i++) {
			ParticleSystem.Particle particle = particles[i];
			particles[i].position = new Vector3(particle.position.x,UTW.getHeightByPos(particle.position),particle.position.z);
		}

		waveParticles.SetParticles(particles,particles.Length);
		
		if (particleDiff > 0) {
			Vector3 campos = Camera.main.transform.position;
			for (int i = 0; i < particleDiff; i++ ) {
				Vector2 randPos = Random.insideUnitCircle.normalized * Random.Range(100f,UTW.config.size/2f);
				Vector2 posC = randPos + new Vector2(campos.x,campos.z);
				Vector3 pos = new Vector3(posC.x, UTW.getHeightByPos(posC),posC.y);
				
				waveParticles.Emit(pos,Vector3.zero,Random.Range(5f,15f),Random.Range(1f,2.5f),waveParticles.startColor);
			}
		}
	}
}
