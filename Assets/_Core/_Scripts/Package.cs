using UnityEngine;
using System.Collections;

public class Package : MonoBehaviour {
	
	public GameObject end;
	public GameObject endlabel;
	
	void OnCollisionEnter(Collision c) {
		if (c.collider.GetComponent<Player>() != null) {
			end.SetActive(true);	
			endlabel.SetActive(true);	
		}
	}
}
