using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(PlayerState))]
public class PlayerMovement : MonoBehaviour {

	public SpriteRenderer GFX;
	public SpriteRenderer Shadow;

	[Header ("Controls")]
	public KeyCode Left = KeyCode.LeftArrow;
	public KeyCode Right = KeyCode.RightArrow;
	public KeyCode Up = KeyCode.UpArrow;
	public KeyCode Down = KeyCode.DownArrow;

	[Header ("Settings")]
	public float walkSpeed = 1f;
	public float jumpTime = 9f;
	public float jumpHeight = 2.5f;

	public Rigidbody2D rb;
	public DIRECTION currentDirection;
	public Vector2 inputDirection;

	private PlayerAnimator animator;
	private PlayerState playerState;
	private AudioPlayer audioPlayer;
	private bool isDead = false;
	private float screenEdgeHorizontal = 80f; //the distance between the player and the horizontal edge of the screen
	private float screenEdgeVertical = 18f; //the distance between the player and the vertical edge of the screen
	private bool isGrounded;

	//a list of states where the player can move
	private List<PLAYERSTATE> MovementStates = new List<PLAYERSTATE> {
		PLAYERSTATE.IDLE,
		PLAYERSTATE.JUMPING,
		PLAYERSTATE.JUMPKICK,
		PLAYERSTATE.MOVING
	};

	void OnEnable() {
		InputManager.onCombatInputEvent += InputEventAction;
		InputManager.onInputEvent += InputEvent;
	}

	void OnDisable() {
		InputManager.onCombatInputEvent -= InputEventAction;
		InputManager.onInputEvent -= InputEvent;
	}

	void Awake() {
		rb = GetComponent<Rigidbody2D> ();
		animator = GetComponentInChildren<PlayerAnimator> ();
		playerState = GetComponent<PlayerState> ();
		currentDirection = DIRECTION.Right;
		isGrounded = true;
	}

	void Update() {
		UpdateSortingOrder ();
	}

	//input events
	void InputEvent(Vector2 dir) {
		inputDirection = dir;

		if (MovementStates.Contains (playerState.currentState) && !isDead) {

			//fake depth by moving slower in y direction
			dir = new Vector2 (dir.x, dir.y * .7f);
			Move (dir * walkSpeed);

		} else {

			//stop moving 
			Move (Vector3.zero);
		}
	}

	void InputEventAction(string action){

		//jump input
		if (action == "Jump" && MovementStates.Contains (playerState.currentState) && !isDead) {

			//disable diagonal jumping (only left or right)
			inputDirection = new Vector3(inputDirection.x, 0,0);
			Move (inputDirection * walkSpeed);

			//start jump
			if(playerState.currentState != PLAYERSTATE.JUMPING)	StartCoroutine (doJump());
		}
	}

	//jump sequence
	IEnumerator doJump() {
		float t = 0;
		Vector3 startPos = GFX.transform.localPosition;
		Vector3 endPos = new Vector3 (startPos.x, startPos.y + jumpHeight, startPos.z);

		playerState.SetState (PLAYERSTATE.JUMPING);
		isGrounded = false;
		animator.Jump();

		//adjust the jump animation speed so it fits with the height and time parameters
		GFX.GetComponent<Animator>().SetFloat ("AnimationSpeed", 1f/jumpTime);

		//going up
		while (t < 1) {
			GFX.transform.localPosition = Vector3.Lerp (startPos, endPos, MathUtilities.Sinerp (0, 1, t));
			t += Time.deltaTime / (jumpTime / 2);
			yield return null;
		}

		//going down
		while (t > 0) {
			GFX.transform.localPosition = Vector3.Lerp (startPos, endPos, MathUtilities.Sinerp (0, 1, t));
			t -= Time.deltaTime / (jumpTime / 2);
			yield return null;
		}

		GFX.transform.localPosition = startPos;

		//show dust particles
		animator.ShowDustEffect();
		isGrounded = true;
	}

	//move the character
	void Move(Vector3 vector) {
		if (playerState.currentState != PLAYERSTATE.JUMPING && playerState.currentState != PLAYERSTATE.JUMPKICK) {

			//removes any existing y offset
			if(isGrounded) GFX.transform.localPosition = Vector3.zero;

			//move player
			if(rb != null)	rb.velocity = vector;

			//play walk or idle animation
			if (Mathf.Abs (vector.x + vector.y) > 0) {

				//calculate current direction
				int i = Mathf.Clamp (Mathf.RoundToInt (vector.x), -1, 1); //change direction based on the current movement vector
				if(i == 0) i = Mathf.RoundToInt(GFX.transform.localScale.x); //stay in the same direction while going up or down

				currentDirection = (DIRECTION)i;
				animator.Walk();

			} else {
				animator.Idle ();
			}
			LookToDir (currentDirection);
			isGrounded = true;
		}
		KeepPlayerInCameraView();
	}

	//keep the player within camera view
	void KeepPlayerInCameraView(){
		Vector2 playerPosScreen = Camera.main.WorldToScreenPoint(transform.position);

		if(playerPosScreen.x + screenEdgeHorizontal > Screen.width && (playerPosScreen.y - screenEdgeVertical < 0)){
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width-screenEdgeHorizontal, screenEdgeVertical, transform.position.z - Camera.main.transform.position.z));

		} else if(playerPosScreen.x + screenEdgeHorizontal > Screen.width){
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width-screenEdgeHorizontal, playerPosScreen.y, transform.position.z - Camera.main.transform.position.z));

		} else if(playerPosScreen.x - screenEdgeHorizontal < 0f && (playerPosScreen.y - screenEdgeVertical < 0)){
			transform.position = Camera.main.ScreenToWorldPoint( new Vector3(screenEdgeHorizontal, screenEdgeVertical, transform.position.z - Camera.main.transform.position.z));

		} else if(playerPosScreen.x - screenEdgeHorizontal < 0f){
			transform.position = Camera.main.ScreenToWorldPoint( new Vector3(screenEdgeHorizontal, playerPosScreen.y, transform.position.z - Camera.main.transform.position.z));

		} else if((playerPosScreen.y - screenEdgeVertical < 0) && (playerPosScreen.x + screenEdgeHorizontal > Screen.width)){
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width-screenEdgeHorizontal, screenEdgeVertical, transform.position.z - Camera.main.transform.position.z));

		} else if((playerPosScreen.y - screenEdgeVertical < 0) && (playerPosScreen.x - screenEdgeHorizontal < 0f)){
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenEdgeHorizontal, screenEdgeVertical, transform.position.z - Camera.main.transform.position.z));

		} else if(playerPosScreen.y - screenEdgeVertical < 0){
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(playerPosScreen.x, screenEdgeVertical, transform.position.z - Camera.main.transform.position.z));
		}
	}

	//idle
	public void Idle() {
		if (playerState.currentState != PLAYERSTATE.JUMPING) {
			animator.Idle ();
			playerState.SetState (PLAYERSTATE.IDLE);
			rb.velocity = Vector3.zero;
		}
	}

	//look towards the movement direction (up and down are ignored because we don't want to update the left/right direction when going up/down);
	public void LookToDir(DIRECTION dir) {
		if (dir == DIRECTION.Left)
			GFX.transform.localScale = new Vector3(-1,1,1);
		else if (dir == DIRECTION.Right)
			GFX.transform.localScale = new Vector3(1,1,1);
	}

	//set the sortingOrder so this enemy appears behind or in front of other objects
	void UpdateSortingOrder() {
		GFX.sortingOrder = Mathf.RoundToInt (transform.position.y * -10f);
		Shadow.sortingOrder = GFX.sortingOrder - 1;
	}

	//returns the current direction
	public DIRECTION getCurrentDirection() {
		return currentDirection;
	}

	//update based on the current input
	public void updateDirection() {
		if(inputDirection.magnitude > 0){
			int i = Mathf.Clamp (Mathf.RoundToInt (inputDirection.x), -1, 1);
			currentDirection = (DIRECTION)i;
			LookToDir (currentDirection);
		}
	}

	//the player has died
	void Death(){
		isDead = true;
	}

	//player is grounded
	public bool playerIsGrounded(){
		return isGrounded;
	}
}

public enum DIRECTION {
	Left = -1,
	Right = 1,
	Up = 2,
	Down = -2,
};