using UnityEngine;
using System.Collections;

public class CameraFollow : CameraShake {

	public GameObject followTarget;

	[Header ("Clamped ViewArea")]
	public Vector2 LeftClamp;
	public Vector2 RightClamp;

	private IEnumerator camShakeCoroutine;
	private bool allowBacktrack;

	void OnEnable(){
		EnemyWaveSystem.onLevelStart += setPlayerAsTarget;
	}

	void OnDisable(){
		EnemyWaveSystem.onLevelStart -= setPlayerAsTarget;
	}

	void Start(){

		//set camera backtracking
		GameSettings settings = Resources.Load("GameSettings", typeof(GameSettings)) as GameSettings;
		if(settings != null) allowBacktrack = settings.CameraBacktrack;
	}

	void Update(){
		if(followTarget != null){

			float x = followTarget.transform.position.x;
			float y = followTarget.transform.position.y + AdditionalOffset;

			//the current viewport position
			Vector3 currentX = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0,0,0));

			//clamped x and y position
			float clampedX = (LeftClamp.x > currentX.x) ? transform.position.x :x;
			y = (y>RightClamp.y) ? y : RightClamp.y;
			Vector3 ClampedPos = new Vector3(Mathf.Clamp(x, clampedX, RightClamp.x), y, transform.position.z);

			//apply camera position + additional offset
			transform.position = ClampedPos + (Vector3.up * AdditionalOffset);

			//disable backtrack by adjusting the left clamp position (change this in Tools/Game Settings)
			if(!allowBacktrack){
				if(x > LeftClamp.x && LeftClamp.x < RightClamp.x) LeftClamp.x = x;
			}
		}
	}

	//sets the clamped camera view to the next wave position
	public void SetNewClampPosition(Vector2 Pos, float lerpTime){
		StartCoroutine(LerpToNewClamp(Pos, lerpTime));
	}

	//sets the clamped area of the utmost left camera area
	public void SetLeftClampedPosition(Vector2 Pos){
		LeftClamp = Pos;
	}

	//smoothly move towards the new position
	private IEnumerator LerpToNewClamp(Vector2 Pos, float lerpTime){
		float camExtentV =  GetComponent<Camera>().orthographicSize;
		float camExtentH = (camExtentV * Screen.width) / Screen.height;
		float t=0;
		Vector2 startPos = RightClamp;
		Vector2 endPos = new Vector2(Pos.x - camExtentH, Pos.y + camExtentV);

		while(t<1){
			RightClamp = Vector2.Lerp(startPos, endPos, MathUtilities.CoSinLerp(0,1,t));
			t += Time.deltaTime / lerpTime;
			yield return 0;
		}

		RightClamp = endPos;
	}

	//set player as follow target
	void setPlayerAsTarget(){
		followTarget = GameObject.FindGameObjectWithTag("Player");
	}
}