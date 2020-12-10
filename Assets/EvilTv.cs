using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EvilTv : MonoBehaviour
{
	private Vector3 strikeDir;
	public float speed=2f;
	public float turnRate = 1f;
	public static CameraHarness player;
	private Rigidbody body;
	private bool hitTarget = false;
	public int attackStrength = 3;
	public static bool playerMoved = true; //will go into generic Enemy class
	public static int numEnemies = 0; //goes into Level class
	protected static int enemiesServiced = 0; //ditto
	
	public float facehuggedRange = 0.025f;
	
	
    // Start is called before the first frame update
    void Start()
    {
		numEnemies++;
        body = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").GetComponent<CameraHarness>();
    }

    // Update is called once per frame
    void Update()
    {
		float singleStep = turnRate * Time.deltaTime;
        if(CanAttack()) {
			Debug.Log(gameObject.name+": commencing ");
			strikeDir = (player.head - transform.position).normalized;
			
			Vector3 newDirection = Vector3.RotateTowards(transform.forward, strikeDir, singleStep, 0.0f);
			transform.localRotation.SetLookRotation(strikeDir); // = Quaternion.LookRotation(newDirection);
			body.velocity = strikeDir*speed;
		}
		if(hitTarget) {
			player.TakeDamage(attackStrength);
			numEnemies--;
			Destroy(gameObject);
		}
    }
    
    void FixedUpdate() {
		if((player.head - transform.position).magnitude < facehuggedRange) {
			hitTarget = true;
		} 
		else
		{
			AvoidWalls();
		}
	}
	
	void AvoidWalls() {
	}
	
	bool CanAttack() {
		if(hitTarget) {
			return false;
		}
		if(playerMoved) {
			enemiesServiced++;
			if(enemiesServiced >= numEnemies) 
				playerMoved = false;
			if((player.head - transform.position).magnitude <=5f)
				Debug.Log(gameObject.name+": "+(player.head - transform.position).magnitude);
			return (player.head - transform.position).magnitude <=5f;
		}
		return false;
	}
	
	void OnCollisionEnter(Collision collision) {
		//if(collision.gameObject.layer == player.layer){
		//	Assert.IsTrue(false,"GOTCHA:"+collision.gameObject.name);
		//}
			
		playerMoved = true; //make a course correction
	}
	
	public void ImHit(float shotPower) {
		Debug.Log(gameObject.name+":They got me");
		numEnemies--;
		Destroy(gameObject);
		Assert.IsTrue(numEnemies > 0, "Got 'em all");
	}
	
	public static void PlayerMoved() {
		enemiesServiced = 0;
		playerMoved = true;
	}
}
