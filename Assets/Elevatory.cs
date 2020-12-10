using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevatory : MonoBehaviour
{
	private FlyingTV controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = transform.parent.GetComponent<FlyingTV>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.name == "HeadshotTarget") {
			controller.GotIm();
		}
	}
	
	public void ImHit(float shotPower) {
		controller.ImHit(shotPower);
	}
}
