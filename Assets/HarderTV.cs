using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class HarderTV : FlyingTV {
	
	public float hitPoints = 6f;
	protected float processingHit = 0f;
	public AudioClip dentSound;

	protected override void UpdatePursuit() {
		base.UpdatePursuit();
		
		if(agent.remainingDistance <= 1f) {
			PlayerMoved();
		}
	}
	
	
	public override void ImHit(float shotPower) {
		if(processingHit - Time.time > 0f) {
			return;
		}
		processingHit = 0.1f + Time.time;
		hitPoints -= shotPower;
		
		if(hitPoints > 0) {
			//Debug.Log("THUNK");
			AudioSource.PlayClipAtPoint(dentSound,transform.position);
			return;
		}
		base.ImHit(shotPower);
	}
}
