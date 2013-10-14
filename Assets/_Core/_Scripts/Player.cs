using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	Flame _flame;
	Slingshot _slingshot;
	
	public float walkSpeed = 3.0f;
	public float jumpSpeed = 15.0f;
	
	Vector3 startPos;
	Vector3 endPos;
	float xDist;
	float lastDist;
	bool jumping;
	bool walking;
	
	public bool attached = false;
	GameObject currentAttached = null;
	
	public Color warmColor;
	public Color coldColor;
	
	Color toColor;
	Color fromColor;
	float colorElapsed = 0.0f;
	float colorDuration = 2.0f;
	
	public float jumpPower = 100.0f;
	float basePower;
	Vector3 lastVelocity;
	
	AudioSource _jumpAudio;
	public AudioClip _jumpAudioClip;
	public AudioClip _ledgeAudioClip;
	public AudioClip _flameGetClip;
	
	// Use this for initialization
	void Start () {
		//_flame = GameObject.Find("Flame").GetComponent<Flame>();
		_slingshot = GetComponent<Slingshot>();
		_jumpAudio = gameObject.AddComponent<AudioSource>();
		
		startPos = transform.position;
		endPos = transform.position;
		
		basePower = jumpPower;
		
		PickupFlame(GameObject.Find("Flame"));
	}
	
	// Update is called once per frame
	void Update () {
		if (jumping) {
			float dist = Vector3.Distance(transform.position, endPos);
			if (dist > jumpSpeed * Time.deltaTime) {
				Vector3 movePos = ((endPos - transform.position).normalized * jumpSpeed * Time.deltaTime);
				rigidbody.MovePosition (rigidbody.position + movePos);
			}
			else {
				rigidbody.MovePosition(endPos);
				jumping = false;
			} 
		}
		else if (!attached) {
			if (walking) {
				float dist = Vector3.Distance(transform.position, endPos);
				if (dist > walkSpeed * Time.deltaTime) {
					Vector3 movePos = ((endPos - transform.position).normalized * walkSpeed * Time.deltaTime);
					movePos.z = 0.0f;
					rigidbody.MovePosition (rigidbody.position + movePos);
				}
				else {
					walking = false;
				}
			}
		}
		
		if (HasFlame()) {
			jumpPower = basePower * 0.7f;
			colorElapsed += Time.deltaTime;
		}
		else {
			jumpPower = basePower;
			colorElapsed -= Time.deltaTime;
		}
		
		if (!OnGround() && !HasFlame()) {
			//_slingshot.setControlledObject(null);	
			//PickupFlame(_flame.gameObject);
		}
		
		
		if (!_flame.inSunHole) {
			float maxDist = 20.0f;
			float d = Mathf.Abs(transform.position.z - _flame.transform.position.z);
			if (transform.position.z > _flame.transform.position.z) { d = 0.0f; };
			RenderSettings.ambientLight = Color.Lerp(Color.black, new Color(0.1f, 0.1f, 0.1f, 1.0f), d/maxDist);
		}
		
		colorElapsed = Mathf.Min(Mathf.Max (0.0f, colorElapsed), colorDuration);
		renderer.material.color = Color.Lerp(coldColor, warmColor, colorElapsed/colorDuration);
	}
	
	public void OnSwipe(Vector2 direction) {
		if (_slingshot.getControlledObject() == null) {
			if (attached) {
				Detach(direction);	
			}
			else {
				Vector2 power = new Vector2(0.0f, 0.0f);
				if (HasFlame() && !OnGround()) {
					power.x = 0.0f;
					power.y = 0.0f;
				}
				else if (!OnGround ()) {
					power.x = jumpPower * 0.4f;//Mathf.Min (400.0f, direction.magnitude);	
					power.y = 0.0f;
				}
				else {
					if (Mathf.Abs (direction.x) > Mathf.Abs(direction.y)) { //We're jumping sideways
						Debug.Log ("Jumping sideways " + Time.deltaTime);
						power.x = jumpPower;
						power.y = jumpPower;// * 1.9f;	
					}
					else {
						power.x = jumpPower;
						power.y = jumpPower;	
					}
				}
				
				direction.Normalize();
				rigidbody.AddForce(new Vector3(direction.x * power.x, 0.0f, direction.y * power.y));
				_jumpAudio.PlayOneShot(_jumpAudioClip);
			}
			
			walking = false;
		}
	}
	
	public bool OnGround() {
		return 	Mathf.Abs(rigidbody.velocity.z) < 0.1f;
	}
	
	public bool HasFlame() {
		return _flame.transform.parent == gameObject.transform;	
	}
	
	public void PickupFlame(GameObject flame) {
		if (flame.name == "Flame2") {
			Physics.gravity = new Vector3(0.0f, 0.0f, -9.81f);	
		}
		_flame = flame.GetComponent<Flame>();
		flame.transform.parent = transform;	
		flame.transform.localPosition = Vector3.zero;
		Destroy (flame.rigidbody);
		//flame.rigidbody.isKinematic = true;
		//flame.rigidbody.detectCollisions = false;
		Physics.IgnoreCollision(collider, flame.collider, true);
	}
	
	public void StartSlingshot() {
		if (OnGround()) {
			if (HasFlame()) {
				if (!_slingshot.getControlledObject()) {
					_slingshot.setControlledObject(_flame.gameObject);	
				}
				else {
					_slingshot.setControlledObject(null);	
					PickupFlame(_flame.gameObject);
				}
			}
		}
	}
	
	public void MovePlayer(Vector2 newPos) {
		if (OnGround()) {
			Vector3 pos = new Vector3(newPos.x, 0.0f, transform.position.z);
			startPos = transform.position;
			endPos = pos;	
			lastDist = Vector3.Distance(startPos, endPos);
			walking = true;
		}
		else {
			//Debug.Log ("in the air! " + rigidbody.velocity.z);	
		}
	}
	
	void FixedUpdate() {
		lastVelocity = rigidbody.velocity;	
	}
	
	void OnCollisionEnter(Collision c) {
		Flame f = c.collider.GetComponent<Flame>();
		if (f != null) {
			PickupFlame(f.gameObject);
			if (!rigidbody.isKinematic) {
				rigidbody.velocity = lastVelocity;
			}
			_jumpAudio.PlayOneShot(_flameGetClip);
		}
	}
	
	void OnTriggerEnter(Collider c) {
		/*Ledge l = c.collider.GetComponent<Ledge>();
		if (l != null && l.gameObject != currentAttached) {
			if (Mathf.Abs (c.contacts[0].normal.x) > 0) {
				Debug.Log(c.gameObject.name);
				Vector3 offset = transform.position - c.contacts[0].point;
				transform.position = c.contacts[0].point + offset;
				rigidbody.isKinematic = true;
				attached = true;
				currentAttached = l.gameObject;
			}
		}*/
		
		Bounds b = collider.bounds;
		Transform parent = c.transform.parent;
		Ledge l = parent.GetComponent<Ledge>();
		float dir = l.type == Ledge.LedgeType.LEFT ? 1.0f : -1.0f;
		transform.position = c.transform.position + new Vector3(dir * b.size.x * 0.5f, 0.0f, 0.0f);
		rigidbody.isKinematic = true;
		attached = true;
		_jumpAudio.PlayOneShot(_ledgeAudioClip);
		
	}
	
	public void JumpPlayer(Vector3 pos) {
		jumping = true;
		startPos = transform.position;
		endPos = pos;
	}
	
	void OnDrawGizmos() {
		Gizmos.DrawWireSphere(startPos, 1.0f);	
		Gizmos.DrawWireSphere(endPos, 1.0f);
	}
	
	public void Detach(Vector2 direction) {
		Vector2 power;
		if (Mathf.Abs (direction.x) > Mathf.Abs(direction.y)) { //We're jumping sideways
			power.x = jumpPower;
			power.y = jumpPower * 1.8f;	
		}
		else {
			power.x = jumpPower;
			power.y = jumpPower;	
		}
		
		direction.Normalize();
		attached = false;
		rigidbody.isKinematic = false;
		rigidbody.AddForce(new Vector3(direction.x * power.x, 0.0f, direction.y * power.y));
		_jumpAudio.PlayOneShot(_jumpAudioClip);
	}
}
