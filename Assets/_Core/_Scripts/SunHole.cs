using UnityEngine;
using System.Collections;

public class SunHole : MonoBehaviour {
	
	Player _player;
	GameObject gate;
	
	// Use this for initialization
	void Start () {
		_player = GameObject.Find("Player").GetComponent<Player>();
		gate = GameObject.Find("Gate");
		
		if (gameObject.name == "SunHole2") {
			renderer.material.color = Color.blue;	
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider c) {
		Debug.Log ("hit hole");
		Flame flame = c.collider.GetComponent<Flame>();
		flame.rigidbody.isKinematic = true;
		flame.rigidbody.useGravity = false;
		flame.collider.enabled = false;
		flame.inSunHole = true;
		flame.transform.position = transform.position;
		
		Physics.gravity = new Vector3(0.0f, 0.0f, 9.81f);
		Camera.main.GetComponent<FollowCam>().transform2 = _player.transform;
		
		StartCoroutine(LightUpLevel());
	}
	
	IEnumerator LightUpLevel() {
		float elapsed = 0.0f;
		float duration = 3.0f;
		Color startAmbience = RenderSettings.ambientLight;
		//Bounds b = gate.collider.bounds;
		//Vector3 startPos = gate.transform.position;
		//Vector3 endPos = startPos - new Vector3(0.0f, 0.0f, b.size.z);
		while (elapsed < duration) {
			RenderSettings.ambientLight = Color.Lerp(startAmbience, Color.white, elapsed/duration);
			//gate.transform.position = Vector3.Lerp(startPos, endPos, elapsed/duration);
			elapsed += Time.deltaTime;
			yield return null;
		}
	}
}
