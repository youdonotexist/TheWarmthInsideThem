using UnityEngine;
using System.Collections;

public class Flame : MonoBehaviour {
	
	Player _player;
	Light _light;
	
	float defaultLight;
	float defaultDistance;
	
	public bool inSunHole = false;
	
	AudioSource _source;
	public AudioClip _hitClip;
	
	// Use this for initialization
	void Start () {
		_player = GameObject.Find("Player").GetComponent<Player>();
		_light = GetComponent<Light>();
		_source = gameObject.AddComponent<AudioSource>();
		
		defaultLight = _light.range;
		defaultDistance = Vector3.Distance(_player.transform.position, transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		float dist = Vector3.Distance(_player.transform.position, transform.position);
		if (dist > defaultDistance) {
			if (_player.transform.position.z <= transform.position.z) { 
				_light.range = Mathf.Max (defaultLight,  dist);	
			}
		}
		else {
			_light.range = defaultLight;	
		}
	}
	
	void OnCollisionEnter(Collision c) {
		_source.PlayOneShot(_hitClip);
	}
}
