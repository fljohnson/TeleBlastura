using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FallingSparks : MonoBehaviour
{
	private ParticleSystem visual;
    // Start is called before the first frame update
    void Start()
    {
       // GetComponent<Rigidbody>().velocity = new Vector3(0f,-.5f,0f);
        visual = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
		if(!visual.isPlaying) {
			//Debug.Log("Bye!");
			Destroy(gameObject);
		}
    }
}
