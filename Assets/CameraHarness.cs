using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Assertions;

public class CameraHarness : MonoBehaviour
{
		
	[Range(0,100)]
	public float Speed = 20f;
	
	public PhysicsCameraController cam;
	private float AdvanceSpeed = 1f;
	private int health = 100;
	private Rigidbody body;
	private AudioSource soundplayer;
	public AudioClip imhit;
	public AudioClip shot;
	public AudioClip down;
	public AudioClip winlevel;
	public Image lifebar;
	public Image lifebarBG;
   
    public float yaw {
		set {
			transform.Rotate(0,value,0);
			
			Vector3 stayHorizontal = transform.eulerAngles;
			stayHorizontal.z = 0f;
			
			transform.eulerAngles = stayHorizontal; 
		}
	}
	
	private Transform headpoint; //where the flying enemies are aimed
	
	public Vector3 head {
		get {
			return headpoint.position;
		}
	}
	
	public int layer {
		get {
			return headpoint.gameObject.layer;
		}
	}
    // Start is called before the first frame update
    
	void Start() {
		soundplayer = GetComponent<AudioSource>();
        body = GetComponent<Rigidbody>();
        headpoint = transform.Find("Main Camera/HeadshotTarget");
        Assert.IsNotNull(headpoint, "Whoops!");
        cam.harness = this;
	}

	
	public void OnMove(InputAction.CallbackContext context) {
		if(GameOver()) {
			return;
		}
		if(cam.LockMouse) {
		/*	if(Input.GetKey(KeyCode.Space))
            {
                AdvanceSpeed = 5f;   
            }*/
		}
		else
        {
            AdvanceSpeed = 1;
        }
		Vector2 dir = context.ReadValue<Vector2>();
		Vector3 velocityVector = body.velocity;
		velocityVector.x = AdvanceSpeed * Speed * dir.x;
		velocityVector.z = AdvanceSpeed * Speed * dir.y;
		
		if(velocityVector.x != 0f || velocityVector.z != 0f) 
			FlyingTV.PlayerMoved();
		velocityVector = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * velocityVector;
		
		body.velocity = velocityVector;
	}
    // Update is called once per frame
    void Update()
    {
        
    }
    
	void OnCollisionEnter(Collision collision) {
		//Debug.Log(collision.gameObject.name);
	}
	
	public void TakeDamage(int howMuch) {
		health = Mathf.Max(0, health - howMuch);
		float full = lifebarBG.rectTransform.rect.width;
		Rect wreckd = lifebar.rectTransform.rect;
		wreckd.width = full*health/100f;
		lifebar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,full*health/100f);
		//lifebar.value = health;
		if(health <= 0)
		{
			DeathSequence();
		}
		else {
			soundplayer.PlayOneShot(imhit);
		}
	}
	
	private void DeathSequence() {
		body.velocity = Vector3.zero;
		body.constraints = RigidbodyConstraints.FreezeRotationZ|RigidbodyConstraints.FreezeRotationY;
		body.AddRelativeTorque(Vector3.right * Mathf.PI*2f);
		soundplayer.PlayOneShot(down);
	}

	
	public void DrawShot(Vector3 ending, Vector3 starting) {
		
		Vector3 origin = ending - (ending - starting)*.95f ;
		Beam.Create(starting,ending);
		soundplayer.PlayOneShot(shot);
	}
	
	public bool GameOver() {
		return (health <= 0 || FlyingTV.numEnemies == 0);
	}
	
	public void Victory() {
		body.velocity = Vector3.zero;
		soundplayer.PlayOneShot(winlevel);
		
	}
}
