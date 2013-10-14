using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour 
{
	public Transform transform1;
	public Transform transform2;
	
	public float minZoom = 5.66f;
	public float minZ = 6.0f;
	
	void LateUpdate() {
		//Bounds b = new Bounds();
		//b.Encapsulate(transform1.renderer.bounds);
		//b.Encapsulate(transform2.renderer.bounds);
		
		camera.orthographicSize = Mathf.Max(Mathf.Abs(transform1.position.z - transform2.position.z), minZoom);
		//Vector3 pos = (transform1.position + transform2.position) * 0.5f; 
		//float distanceBetweenCenterAndMinZ = Mathf.Abs (minZ - camera.orthographicSize);
		float midPoint = (transform1.position.z + transform2.position.z) * 0.5f;
		float midPointx = (transform1.position.x + transform2.position.x) * 0.5f;
		Vector3 pos = new Vector3(midPointx, 0.0f, midPoint + (camera.orthographicSize * 0.5f) - 1.0f); 
		
		
		pos.x = pos.x;
		pos.y = 50; 
		pos.z = Mathf.Max(minZ, pos.z);
		
		camera.transform.position = pos;
	}
}