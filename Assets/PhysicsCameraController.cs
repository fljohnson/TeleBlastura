using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PhysicsCameraController : MonoBehaviour {

	public float Sensitivity = 0.9f;
	
	[Range(0,100)]
	public float RotationSpeed = 7f;
	public bool LockMouse;

	private float Xaxis;
	private float Yaxis;
	private float LastX;
	private float LastY;
	
	public CameraHarness harness;
 
	private Camera cam;
	
	private Vector3 muzzle;
	
	private int layerMask = ~(1 << 9);
	
	protected float weaponPower = 2f;
	
	void Start()
    {
        cam = GetComponent<Camera>();
        muzzle = transform.Find("fire_sleet/Muzzle").transform.position; //new Vector3(.5f, 0f, cam.nearClipPlane);
    }
    
    void Update() {
		
	}
	
	/*
   private void LateUpdate()
    {
        if(Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            LockMouse = !LockMouse;

            if(!LockMouse)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState  = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
		Move(); 
		if (Input.GetKeyDown(KeyCode.RightShift)) {
			Fire();
		}
    }
    */
    public void OnMlookToggle(InputAction.CallbackContext context) {
		LockMouse = !LockMouse;

		if(!LockMouse)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else
		{
			Cursor.lockState  = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
	
    public void OnKbd(InputAction.CallbackContext context) {
		Application.Quit();
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}
    private void Fire() {
		muzzle = transform.Find("fire_sleet/Muzzle").transform.position;
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        ray.origin = muzzle; //cam.ViewportToWorldPoint(muzzle);
        Vector3 endpoint = ray.GetPoint(4f);
        if (Physics.Raycast(ray, out hit,4f,layerMask))
        {
			endpoint = hit.point;
			Elevatory enemy = hit.transform.gameObject.GetComponentInChildren<Elevatory>();
			if(enemy != null) {
				enemy.ImHit(weaponPower);
			}
			//Debug.Log("HIT:"+hit.transform.name);
		}
		harness.DrawShot(endpoint,muzzle); //cam.ViewportToWorldPoint(muzzle));
	}

	public void OnFire(InputAction.CallbackContext context) {
		if(harness.GameOver())
		{
			return;
		}
		//this eliminates multiple shots on a button press
		if(context.phase == InputActionPhase.Started) {
			Fire();
		}
	}
	
	public void OnLook(InputAction.CallbackContext context) {
		if(harness.GameOver())
		{
			return;
		}
		if(LockMouse) {
			Vector2 dir = context.ReadValue<Vector2>();
			Xaxis = Mathf.Clamp(dir.x,-1f,1f) + LastX * Sensitivity;
			Yaxis = -Mathf.Clamp(dir.y,-1f,1f) + LastY * Sensitivity;

			LastX = Xaxis;
			LastY = Yaxis;

			//Debug.Log(Xaxis.ToString("F4")+" "+Yaxis.ToString("F4"));
			
			float deltaX = RotationSpeed * Yaxis * Time.deltaTime;
			float deltaY = RotationSpeed * Xaxis * Time.deltaTime;
			//Debug.Log(deltaX.ToString("F4")+" "+deltaY.ToString("F4"));
			//some limiters, to prevent control inversion due to tilting more than 90 deg from horizontal
			//(FLJ, 12/1/2020)
			float currentX = transform.eulerAngles.x;
			bool okToTilt = true;
			if(deltaX > 0) {
				okToTilt &= (currentX < 80f || currentX >270f);
			}
			if(deltaX < 0) {
				okToTilt &= (currentX > 260f || currentX < 90f);
			}
			if(okToTilt) 
				transform.Rotate(deltaX, 0,0);
			harness.yaw = RotationSpeed * Xaxis * Time.deltaTime;
		}
		
		Vector3 stayHorizontal = transform.eulerAngles;
		stayHorizontal.z = 0f;
		
		transform.eulerAngles = stayHorizontal;
		
	}
	
	
}
