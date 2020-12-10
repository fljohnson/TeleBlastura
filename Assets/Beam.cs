using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour {
	
	private static GameObject beamPrototype;
	private float afterImageTime = 0f;
	
	void Start() {
		afterImageTime = .01f;
	}
	
	protected static void LoadPrototype() {
		if(beamPrototype == null) {
			beamPrototype = Resources.Load("beam_9mm") as GameObject;
		}
	}
	
	void Update() {
		if(afterImageTime <= 0f) {
			Destroy(gameObject);
		}
		else {
			afterImageTime -= Time.deltaTime;
		}
	}
	
	public static void Create(Vector3 start, Vector3 end) {
		//Debug.DrawLine(origin, endpoint, Color.red,1f,false);
		Vector3 midpoint = (start+end)*0.5f; //that takes care of the where
		Quaternion rotacion = Quaternion.identity;
		Vector3 newDirection = (end-start).normalized;
		rotacion = Quaternion.LookRotation(newDirection);
		LoadPrototype();
		GameObject currentBeam = Instantiate(beamPrototype,start,rotacion);
		Vector3 scala = currentBeam.transform.localScale;
		scala.z = Vector3.Distance(start,end);
		scala.x *=0.25f;
		scala.y *=0.25f;
		currentBeam.transform.localScale = scala; 
	}
}
