using UnityEngine;
using System.Collections;
using Vectrosity;

public class Slingshot : MonoBehaviour {
	public GameObject tracker = null;
	private SceneTracker trackerComponent = null;
	
	public float maxForce = 1000.0f;
	public float maxDist = 2.0f;
	public Vector3 flameDockPos = new Vector3(0.0f, 0.0f, 2.0f);
	
	private GameObject controlledObject = null;
	
	private Vector3 tempDir = Vector3.zero;
	
	bool mouseControlled = false;
	
	VectorLine slingPosition;
	
	AudioSource _source;
	public AudioClip pull;
	public AudioClip throws;
	
	// Use this for initialization
	void Start () {
		_source = gameObject.AddComponent<AudioSource>();
		if (tracker != null) {
			trackerComponent = tracker.GetComponent<SceneTracker>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		handleInput();
		
		if (slingPosition != null) {
			slingPosition.vectorObject.transform.position = transform.position;	
		}
	}
	
	public GameObject getControlledObject() {
		return controlledObject;	
	}
	
	public void setControlledObject(GameObject go) {
		if (controlledObject != null && controlledObject.rigidbody != null) {
			//controlledObject.rigidbody.isKinematic = false;
		}
		
		controlledObject = go;
		
		if (controlledObject != null) {
			//controlledObject.rigidbody.isKinematic = true;
			controlledObject.transform.position = transform.position + flameDockPos;
			Physics.IgnoreCollision(collider, controlledObject.collider, true);
			
			if (slingPosition == null) {
				Vector3 flameHome = flameDockPos;
				Vector3 left = new Vector3(flameHome.x - maxDist, flameHome.y, flameHome.z);
				Vector3 right = new Vector3(flameHome.x + maxDist, flameHome.y, flameHome.z);
				Vector3 bottom = new Vector3(flameHome.x, flameHome.y, flameHome.z - maxDist);
				
				slingPosition = new VectorLine("sling", new Vector3[] {left, right, bottom, left}, null, 2.0f, LineType.Continuous);
				slingPosition.Draw3D();
				slingPosition.vectorObject.transform.parent = transform;
			}
			else {
				Vector3 flameHome = flameDockPos;
				Vector3 left = new Vector3(flameHome.x - maxDist, flameHome.y, flameHome.z);
				Vector3 right = new Vector3(flameHome.x + maxDist, flameHome.y, flameHome.z);
				Vector3 bottom = new Vector3(flameHome.x, flameHome.y, flameHome.z - maxDist);
				
				slingPosition.points3 = new Vector3[] {left, right, bottom, left};
				slingPosition.Draw3D();
				
				
			}
		}
		else {
			if (slingPosition != null) {
				slingPosition.points3 = new Vector3[] {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
				slingPosition.Draw3D();	
			}
		}
	}
	
	private void handleInput() {
		if (controlledObject != null) {
		SceneTracker.MOUSE_STATE mouse_state = trackerComponent.getMouseState();
			if (mouse_state == SceneTracker.MOUSE_STATE.FIRST_DOWN) {
				Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
				RaycastHit o = new RaycastHit();
				LayerMask drobMask = 1 << LayerMask.NameToLayer("Flame");
				
				if (Physics.Raycast(ray, out o, Mathf.Infinity, drobMask)) {
					GameObject temp = o.collider.gameObject;
					_source.PlayOneShot(pull);
					mouseControlled = true;
				}
			}
			else if (mouse_state == SceneTracker.MOUSE_STATE.JUST_RELEASED) {
				if (controlledObject != null && mouseControlled == true) {
					Vector3 flameHome = transform.position + flameDockPos;
					Vector3 shottedPos = controlledObject.transform.position;
					
					Vector3 dir = ((flameHome - shottedPos).normalized);
					float dist	= Vector3.Distance(flameHome, shottedPos);
					dir = dir * ((dist/maxDist) * maxForce);
					tempDir = dir;
					
					//controlledObject.rigidbody.isKinematic = false;
					//controlledObject.rigidbody.detectCollisions = true;
					controlledObject.transform.parent = null;
					
					controlledObject.AddComponent<Rigidbody>();
					controlledObject.rigidbody.useGravity = true;
					controlledObject.rigidbody.AddForce(dir);
					controlledObject.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
					controlledObject.rigidbody.constraints = ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);
					
					
					Physics.IgnoreCollision(collider, controlledObject.collider, false);
					_source.PlayOneShot(throws);
					
					mouseControlled = false;
					setControlledObject(null);
					
					slingPosition.points3 = new Vector3[] {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
					slingPosition.Draw3D();
				}
			}
			else if (mouse_state == SceneTracker.MOUSE_STATE.HELD) {
				if (controlledObject != null && mouseControlled == true) {
					Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
					
					Vector3 flameHome = transform.position + flameDockPos;
					Vector3 intendedPos = ray.origin; intendedPos.y = 0.0f;
					
					Vector3 dir = intendedPos - flameHome;
					float length = dir.magnitude;
					
					//Check to see if we're in front of our original point
					if (length > maxDist) {
						Vector3 normalizeDir = dir.normalized;
						normalizeDir.Scale(new Vector3(maxDist, 0.0f, maxDist));
						Vector3 f = flameHome + normalizeDir;
						if (f.z > flameHome.z) {
							f.z = flameHome.z;
						}
						controlledObject.transform.position = f;
					}
					else {
						if (intendedPos.z > flameHome.z) {
							intendedPos.z = flameHome.z;
						}
						controlledObject.transform.position = intendedPos;	
					}
				}
			}	
		}
	}
}
