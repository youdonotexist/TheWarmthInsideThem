using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour
{
	UILabel distanceLabel;
	public GameObject _player;
	
	float startHeight;
	
	void Start() {
		distanceLabel = GameObject.Find ("DistanceLabel").GetComponent<UILabel>();	
		
		_player = GameObject.Find("Player");
		startHeight = 4.378032f;//_player.transform.position.z;
	}
	
	// Update is called once per frame
	void Update ()
	{
		distanceLabel.text = Mathf.Max(0.0f, (_player.transform.position.z - startHeight)).ToString() + "m" ;
	}
}

