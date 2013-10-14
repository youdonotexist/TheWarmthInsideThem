using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

	Player _player;
	private SceneTracker _trackerComponent = null;
	private bool swipeDown = false;
	
	//Swipe State Variables
	private Vector3 swipeNormalizedVector;
	Vector3 swipeStartPosition;
    float swipeStartTime;
	
	Vector3 startTouch;
	
	AudioSource _swipeSound;
	public AudioClip _swipeSoundClip;
	
	public LayerMask touchLayers;
	public TrailRenderer _trailRendererPrefab;
	public TrailRenderer _trailRenderer;
	// Use this for initialization
	void Start () {
		_player = GameObject.Find("Player").GetComponent<Player>();
		_trackerComponent = GameObject.Find("Main Camera").GetComponent<SceneTracker>();
		_swipeSound = gameObject.AddComponent<AudioSource>();
	}
	
	
	
	// Update is called once per frame
	void Update () {
		HandleInput();
		/*HandleInput();
		if (HandlePlayerClick()) {}
		else if(HandleLedgeClick()) {}
		else if (!_player.attached) {
			if (_trackerComponent.getMouseState() == SceneTracker.MOUSE_STATE.FIRST_DOWN) {
				Vector3 pos = Camera.main.ScreenToWorldPoint( Input.mousePosition ); pos.y = 0.0f;
				_player.MovePlayer(pos);
			}
		}
		//else if (swipeDown) {
		//	_player.Detach(swipeNormalizedVector);
		//}*/
	}
	
	private bool HandlePlayerClick() {
		SceneTracker.MOUSE_STATE mouse_state = _trackerComponent.getMouseState();
		if (mouse_state == SceneTracker.MOUSE_STATE.FIRST_DOWN) {
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit o = new RaycastHit();
			LayerMask drobMask = 1 << LayerMask.NameToLayer("Player");	
			if (Physics.Raycast(ray, out o, Mathf.Infinity, drobMask)) {
				
				return true;
			}
		}
		return false;
	}
	
	private bool HandleLedgeClick() {
		SceneTracker.MOUSE_STATE mouse_state = _trackerComponent.getMouseState();
		if (mouse_state == SceneTracker.MOUSE_STATE.FIRST_DOWN) {
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit o = new RaycastHit();
			LayerMask drobMask = 1 << LayerMask.NameToLayer("Ledge");
			
			if (Physics.Raycast(ray, out o, Mathf.Infinity, drobMask)) {
				
				return true;
			}
		}
		
		return false;
	}
	
	private void HandleInput() {
		if (Input.GetKey(KeyCode.Space)) {
			Time.timeScale = 0.3f;
			Time.fixedDeltaTime = 0.04f;// * (0.7f);// * Time.timeScale;
		}
		else {
			Time.timeScale = 1.0f;	
			Time.fixedDeltaTime = 0.04f;
		}	
	}
	
	// Subscribe to events 
	void OnEnable(){
		EasyTouch.On_TouchStart += On_TouchStart; 
		EasyTouch.On_TouchUp += On_TouchEnd;
		
		EasyTouch.On_SwipeStart += On_SwipeStart;
		EasyTouch.On_Swipe += On_Swipe;
		EasyTouch.On_SwipeEnd += On_SwipeEnd;
		
		EasyTouch.On_DragStart += On_DragStart;
		EasyTouch.On_Drag += On_Drag;
		EasyTouch.On_DragEnd += On_DragEnd;
	}
	// Unsubscribe 
	void OnDisable() {
		EasyTouch.On_TouchStart -= On_TouchStart; 
		EasyTouch.On_TouchUp -= On_TouchEnd;
		
		EasyTouch.On_SwipeStart -= On_SwipeStart;
		EasyTouch.On_Swipe -= On_Swipe;
		EasyTouch.On_SwipeEnd -= On_SwipeEnd;
		
		EasyTouch.On_DragStart -= On_DragStart;
		EasyTouch.On_Drag -= On_Drag;
		EasyTouch.On_DragEnd -= On_DragEnd;
	}
	
	// Unsubscribe 
	void OnDestroy(){
		EasyTouch.On_TouchStart -= On_TouchStart; 
		EasyTouch.On_TouchUp -= On_TouchEnd;
		
		EasyTouch.On_SwipeStart -= On_SwipeStart;
		EasyTouch.On_Swipe -= On_Swipe;
		EasyTouch.On_SwipeEnd -= On_SwipeEnd;
		
		EasyTouch.On_DragStart -= On_DragStart;
		EasyTouch.On_Drag -= On_Drag;
		EasyTouch.On_DragEnd -= On_DragEnd;
	}
	
	// Touch start event
	public void On_TouchStart(Gesture gesture) {
		GameObject pickedObject = gesture.pickObject;
		if (pickedObject != null) {
			if (LayerMask.LayerToName(pickedObject.layer) == "Ledge") {
				//Ledge ledge = pickedObject.GetComponent<Ledge>();
				//_player.JumpPlayer(ledge.anchor.transform.position);
				//_player.rigidbody.isKinematic = true;
				//_player.attached = true;	
			}
			else if (LayerMask.LayerToName(pickedObject.layer) == "Player") {
				startTouch = gesture.position;	
			}
		}
		else {
				
		}
	}
	
	public void On_TouchEnd(Gesture gesture) {
		Debug.Log ("Touch End");
		GameObject pickedObject = gesture.pickObject;
		if (pickedObject != null) {
			Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(gesture.position);
			Vector3 inBoundsPoint = new Vector3(touchWorldPos.x, 0.0f, touchWorldPos.z);
			if (_player.collider.bounds.Contains(inBoundsPoint)) {
				if (LayerMask.LayerToName(pickedObject.layer) == "Player") {
					_player.StartSlingshot();		
				}
			}
		}
		else {
			Vector3 pos = Camera.main.ScreenToWorldPoint(gesture.position);
			pos.y = 0.0f;
			_player.MovePlayer(pos);	
		}
	}
	
	//Swipes happen globally
	private void On_SwipeStart(Gesture gesture){
		InitTrail(gesture);
	}
	private void On_Swipe(Gesture gesture){
		UpdateTrail(gesture);
	}
	
	public void On_SwipeEnd( Gesture gesture) {
		_player.OnSwipe(gesture.swipeVector);
		_swipeSound.PlayOneShot(_swipeSoundClip);
	}
	
	//Drags happen on an object
	private void On_DragStart(Gesture gesture){
		InitTrail(gesture);
	}
	private void On_Drag(Gesture gesture){
		UpdateTrail(gesture);
	}
	
	public void On_DragEnd( Gesture gesture) {
		Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(gesture.position);
		Vector3 inBoundsPoint = new Vector3(touchWorldPos.x, 0.0f, touchWorldPos.z);
		if (!_player.collider.bounds.Contains(inBoundsPoint)) {
			_player.OnSwipe(gesture.swipeVector);
		}
		
		_swipeSound.PlayOneShot(_swipeSoundClip);
	}
	
	void InitTrail(Gesture gesture) {
		if (gesture.fingerIndex==0 && _trailRenderer==null){ 
			
			// the world coordinate from touch for z=5
			Vector3 position = Camera.main.ScreenToWorldPoint(gesture.position); position.y = 0.0f;
			_trailRenderer = Instantiate( _trailRendererPrefab,position,Quaternion.identity) as TrailRenderer;
		}
	}
	
	void UpdateTrail(Gesture gesture) {
		if (_trailRenderer!=null){
			// the world coordinate from touch for z=5
			Vector3 position = Camera.main.ScreenToWorldPoint(gesture.position); position.y = 0.0f;
			_trailRenderer.transform.position = position;
		}	
	}
}
