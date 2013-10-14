using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	Flame _flame;
	Player _player;
	
	float doTimeElapsed = 0.0f;
	float doTime = 5.0f;
	
	void Start() {
		_flame = GameObject.Find("Flame").GetComponent<Flame>();
		_player = GameObject.Find("Player").GetComponent<Player>();
		
	}
	
	void OnClick () {
		Application.LoadLevel("Scene0Mike");
	}
	
	void Update() {
		if (doTimeElapsed > doTime) {
			if (_player.HasFlame()) {
				Physics.IgnoreCollision(_player.collider, _flame.collider, false);
				_flame.transform.position += new Vector3(0.0f, 0.0f, 0.3f);
				_flame.rigidbody.isKinematic = false;
				_flame.rigidbody.velocity = new Vector3(0.0f, 0.0f, 5.0f);
				_flame.transform.parent = _player.transform.parent;
				
				doTimeElapsed = 0.0f;
			}
		}
		else {
			if (_player.HasFlame()) {
				doTimeElapsed += Time.deltaTime;	
			}
		}
		
	}
}
