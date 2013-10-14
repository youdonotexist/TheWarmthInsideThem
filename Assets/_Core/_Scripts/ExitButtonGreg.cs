using UnityEngine;
using System.Collections;

public class ExitButtonGreg : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Escape))
        {
            Application.LoadLevel("Scene0Mike");
        }
	}
	
	void OnClick () {
		Application.LoadLevel("Scene0Mike");
	}
}
