using UnityEngine;
using System.Collections;

public class ScoreZoneParticleEmitter {
	public ParticleSystem emitter;
	public ScoreZoneParticleEmitter()
	{
		emitter = new ParticleSystem ();
//		emitter.duration = 5.0f;
		emitter.loop = true;
		emitter.startLifetime = 2;
		emitter.startSize = 0.5f;
		emitter.startColor = new Color32 (0,248,144,255);
		emitter.playOnAwake = false;
		emitter.maxParticles = 200;
		emitter.emissionRate = 30;
//		emitter.

	}
}
