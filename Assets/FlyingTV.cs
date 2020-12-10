using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class FlyingTV : MonoBehaviour
{
	public static CameraHarness player;
	public float facehuggedRange = 0.025f;
	public int attackStrength = 30;
	
	protected NavMeshAgent agent;
	private static bool needAttackVector = false;
	public static int numEnemies = 0; //goes into Level class
	protected static int enemiesServiced = 0; //ditto
	
	private GameObject actuel ;
	private Rigidbody rigide ;
	
	private float finalVelocity;
	
	protected enum State {
		IDLE,
		PURSUING,
		HOMING,
		GOT_HIM,
		GOT_SHOT,
		DYING		
	}
	
	protected State state = State.IDLE;
	
	public static GameObject deathAnimPrototype;
	
    // Start is called before the first frame update
    void Start()
    {
		LoadPrototypes();
		numEnemies++;
        agent = GetComponent<NavMeshAgent>();
       // agent.updatePosition = false;
		actuel = transform.Find("TVset").gameObject;
		rigide = actuel.GetComponent<Rigidbody>();
        player = GameObject.Find("Player").GetComponent<CameraHarness>();
    }

    // Update is called once per frame
    void Update()
    {
		switch(state) {
			case State.IDLE: 
				CheckForPursuit();
				break;
			case State.PURSUING:
				UpdatePursuit();
				break;
			case State.HOMING:
				UpdatePursuit();
				break;
			case State.GOT_SHOT:
				HandleGettingShot();
				break;
			case State.GOT_HIM:
				numEnemies--;
				if(numEnemies <= 0) {
					player.Victory();
				}
				Destroy(gameObject);
				break;
			case State.DYING:
				AdvanceDeathSequence();
				break;
		}
       
    }
    
    protected virtual void UpdatePursuit() {

		 if(needAttackVector) {
			enemiesServiced++;
			if(enemiesServiced >= numEnemies) 
				needAttackVector = false;
			agent.SetDestination(player.head);
		}
		
		Vector3 floorVel = agent.velocity;
		if(floorVel.magnitude != 0f) 
		{
			Vector3 closing = (transform.InverseTransformPoint(player.head) - actuel.transform.localPosition);
			//Debug.Log(transform.InverseTransformPoint(player.head));
			float forwardVel = Mathf.Sqrt(Mathf.Pow(floorVel.x,2f)+Mathf.Pow(floorVel.z,2f));
			float forwardDist = Mathf.Sqrt(Mathf.Pow(closing.x,2f)+Mathf.Pow(closing.z,2f));
			float vertDist = closing.y;
			
			float vertVel = (vertDist/forwardDist)*forwardVel;
			Assert.IsTrue(actuel.transform.localPosition.y <1.2f,"aha");
			
			//if(vertVel > 0 && actuel.transform.localPosition.y >= 1.2f){
			//	vertVel =
			
			//Debug.Log(floorVel.ToString("F4")+" "+transform.InverseTransformDirection(floorVel).ToString("F4"));

			rigide.velocity = new Vector3(0,vertVel,0);
			actuel.transform.localRotation.SetLookRotation(closing.normalized);
			if(forwardDist <= 1.0f){
				finalVelocity = agent.speed;
				agent.SetDestination(player.head);
			}
			
			
		}
	}
	/*
	protected void HomeIn() {
		Vector3 closing = (player.head - actuel.transform.position);
		float forwardDist = Mathf.Sqrt(Mathf.Pow(closing.x,2f)+Mathf.Pow(closing.z,2f));
		if(forwardDist > 2.0f) {
			Debug.Log("FALL BACK");
			agent.enabled = true;
			state = State.PURSUING;
			PlayerMoved();
			return;
		}
		
			
	}
	*/
	
    protected void CheckForPursuit() {
		if((player.head - actuel.transform.position).magnitude <=5f) {
			state = State.PURSUING;
			PlayerMoved();
		}
	}
	
	protected void HandleGettingShot() {
		numEnemies--;
		state = State.DYING;
		//Start the death animation
	}
	
	protected void AdvanceDeathSequence() {
		//The death animation has finished
		Instantiate(deathAnimPrototype,actuel.transform.position,Quaternion.identity);
		if(numEnemies <= 0) {
			player.Victory();
		}
		Destroy(gameObject);
				
	}
	
	public void GotIm() {
		state = State.GOT_HIM;
		player.TakeDamage(attackStrength);
	}
	
	public virtual void ImHit(float shotPower) {
		state = State.GOT_SHOT;
		//Debug.Log(gameObject.name+":They got me");
	}
	
	public static void PlayerMoved() {
		enemiesServiced = 0;
		needAttackVector = true;
	}
	
	public static void LoadPrototypes() {
		if(deathAnimPrototype != null) {
			return;
		}
		deathAnimPrototype = Resources.Load("FallingSparks") as GameObject;
	}
	
}
