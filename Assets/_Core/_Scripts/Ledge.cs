using UnityEngine;
using System.Collections;

public class Ledge : MonoBehaviour {
	
	public LedgeAnchor anchor;
	
	public enum LedgeType {
		LEFT,
		RIGHT
	}
	
	public LedgeType type;
	
	// Use this for initialization
	void Start () {
		anchor = GetComponentInChildren<LedgeAnchor>();
	}
	
	// Update is called once per frame
	void Update () {

		
	}
}
