using UnityEngine;
using System.Collections;

using Vectrosity;

public class SceneTracker : MonoBehaviour
{
	public enum MOUSE_STATE {
		FIRST_DOWN,
		HELD,
		MOVING,
		JUST_RELEASED,
		RELEASED
	}
	
	MOUSE_STATE mouse_state = MOUSE_STATE.RELEASED;
	float x;
	float y;
	
	//Visual
	VectorLine line = null;
	public Material lineMaterial = null;
	
	//Vector3 firstTouch = Vector3.zero;
	//Vector3 secondTouch = Vector3.zero;
	
	GameObject firstObject = null;
	GameObject secondObject = null;
	
	// Use this for initialization
	void Start ()
	{
		//Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
		Time.timeScale = 1;
	}
	
	

	// Update is called once per frame
	void Update ()
	{
		getMouseState();
		
		if (Input.GetKey (KeyCode.A)) {
			Vector3 vec = Camera.mainCamera.transform.position;
			vec.x -= 0.2f;
			Camera.mainCamera.transform.position = vec;
		}
		if (Input.GetKey (KeyCode.D)) {
			Vector3 vec = Camera.mainCamera.transform.position;
			vec.x += 0.2f;
			Camera.mainCamera.transform.position = vec;
		}
		if (Input.GetKey (KeyCode.W)) {
			Vector3 vec = Camera.mainCamera.transform.position;
			vec.y += 0.2f;
			Camera.mainCamera.transform.position = vec;
		}
		if (Input.GetKey (KeyCode.S)) {
			Vector3 vec = Camera.mainCamera.transform.position;
			vec.y -= 0.2f;
			Camera.mainCamera.transform.position = vec;
		}
	}
	
	public Vector3 checkPointA() {
		RaycastHit pA;
		
		if (Physics.Raycast(firstObject.transform.position, 
		                (secondObject.transform.position - firstObject.transform.position).normalized,
		                out pA, Mathf.Infinity)) {
			if (pA.rigidbody == firstObject.rigidbody)
				return pA.point;
		}	
		
		return Vector3.zero;
	}
	
	public Vector3 checkPointB() {
		RaycastHit pB;
		
		if (Physics.Raycast(secondObject.transform.position, 
		                (firstObject.transform.position - secondObject.transform.position).normalized,
		                out pB, Mathf.Infinity)) {
			if (pB.rigidbody == secondObject.rigidbody)
				return pB.point;
		}
		
		return Vector3.zero;
	}
	
	public MOUSE_STATE getMouseState() {
		bool mouseDown 	= Input.GetMouseButtonDown(0);
		bool mouseUp 	= Input.GetMouseButtonUp(0);
		
		if (mouseDown == true) {
			mouse_state = SceneTracker.MOUSE_STATE.FIRST_DOWN;
			return mouse_state;
		}
		
		if (mouseUp == true) {
			mouse_state = SceneTracker.MOUSE_STATE.JUST_RELEASED;
			return mouse_state;
		}
		
		
		if (!mouseUp && !mouseDown) {
			if (mouse_state == SceneTracker.MOUSE_STATE.FIRST_DOWN)
				mouse_state = SceneTracker.MOUSE_STATE.HELD;
			else if (mouse_state == SceneTracker.MOUSE_STATE.JUST_RELEASED)
				mouse_state = SceneTracker.MOUSE_STATE.RELEASED;	
		}
		
		return mouse_state;
	}
}

