using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(PlayerState))]
public class PlayerCombat : MonoBehaviour {

	[Header("Attack Data")]
	public DamageObject[] PunchAttackData; //a list of punch attacks
	public DamageObject[] KickAttackData; //a list of kick Attacks
	public DamageObject JumpKickData; //jump kick Attack
	public Item currentWeapon; //the current weapon the player is holding

	public GameObject itemInRange; //an item that is currently in interactable range
	public Transform weaponBone; //the bone were weapon will be parented on
	public delegate void PlayerEventHandler();
	public static event PlayerEventHandler OnPlayerDeath;

	[SerializeField] private int attackNum = 1; //the current attack number
	private bool continuePunchCombo; //true if a punch combo needs to continue
	private bool continueKickCombo; //true if the a kick combo needs to  continue
	private PlayerAnimator animator; //link to the animator component
	private PlayerState playerState; //the state of the player
	private float LastAttackTime = 0; //time of the last attack
	private bool targetHit; //true if the last hit has hit a target
	private bool ChangeDirDuringCombo = false; //allows player to change direction at the start of an attack
	private bool ChangeDirAtLastHit = true; //allows player to change direction at the last hit
	private bool BlockAttacksFromBehind = true; //block enemy attacks coming from behind
	private int HitKnockDownThreshold = 3; //the number of times the player can be hit before being knocked down
	private int HitKnockDownCount = 0; //the number of times the player is hit in a row
	private int HitKnockDownResetTime = 2; //the time before the hitknockdown counter resets
	private float LastHitTime = 0; // the time when we were hit 
	private List<PLAYERSTATE> AttackStates = new List<PLAYERSTATE> { PLAYERSTATE.IDLE, PLAYERSTATE.MOVING, PLAYERSTATE.JUMPING, PLAYERSTATE.PUNCH, PLAYERSTATE.KICK, PLAYERSTATE.DEFENDING }; //a list of states where the player can attack
	private List<PLAYERSTATE> HitStates = new List<PLAYERSTATE> { PLAYERSTATE.HIT, PLAYERSTATE.DEATH, PLAYERSTATE.KNOCKDOWN }; //a list of states where the player was hit
	private float yHitDistance = 0.4f;  //the Y distance from which the player is able to hit an enemy
	private bool isDead = false; //true if this player has died
	private bool jumpKickActive; //true if a jump kick has been done
	private bool defend = false; //true if the defend button is down

	private void OnEnable() {
		InputManager.onCombatInputEvent += CombatInputEvent;
	}

	private void OnDisable() {
		InputManager.onCombatInputEvent -= CombatInputEvent;
	}

	private void Awake() {
		animator = GetComponentInChildren<PlayerAnimator> ();
		playerState = GetComponent<PlayerState> ();

		if (animator == null) { 
			Debug.Log ("No player animator found assigned in gameobject " + gameObject.name);
		}
	}

	private void Update(){

		//checks for a jump kick hit each frame when jumpKickActive is true
		if(jumpKickActive && !GetComponent<PlayerMovement>().playerIsGrounded()){
			CheckForHit();
		}

		//checks if the defend button is being held down and otherwise goes back to the idle state
		if(defend){
			defend = false;
			Defend();
		} else {
			if (playerState.currentState == PLAYERSTATE.DEFENDING){
				GetComponent<PlayerMovement>().Idle();
				animator.StopDefend();
			}
		}
	}

	//a combat input event has taken place
	private void CombatInputEvent(string action) {
		if (AttackStates.Contains (playerState.currentState) && !isDead) {

			//the punch button was pressed
			if (action == "Punch" && playerState.currentState != PLAYERSTATE.KICK) {

				//do a jump kick when the punch button is pressed during a jump
				if (playerState.currentState == PLAYERSTATE.JUMPING) {
					doJumpKickAttack();

				} else {

					//pick up an item when we are in range
					if (itemInRange != null && ObjInYRange (itemInRange)) {
						InteractWithItem();

					} else if (playerState.currentState != PLAYERSTATE.PUNCH) {

						//throw an item if we are holding one
						if (currentWeapon != null && currentWeapon.itemName == "Knife") {
							StartThrowAttack();
						} else {

							//do a punch
							doPunchAttack();
						}

					} else {

						//automatically continue to the next punch when the player presses the button during an attack
						if (attackNum < PunchAttackData.Length - 1) {
							continuePunchCombo = true;
							continueKickCombo = false;
						}
					}
				}
			}

			//the kick button was pressed
			if (action == "Kick" && playerState.currentState != PLAYERSTATE.PUNCH) {

				//do a jump kick when the kick button is pressed during a jump
				if (playerState.currentState == PLAYERSTATE.JUMPING) {
					doJumpKickAttack();

				} else {

					//pick up an item when we are in range
					if (itemInRange != null && ObjInYRange (itemInRange)) {
						InteractWithItem ();

					} else if (playerState.currentState != PLAYERSTATE.KICK) {

						//do a kick attack
						doKickAttack ();

					} else {

						//automatically continue to the next kick when the player presses the attack button during an attack
						if (attackNum < KickAttackData.Length - 1) { 
							continueKickCombo = true;
							continuePunchCombo = false;
						}
					}
				}
			}

			//the defend button was pressed
			if(action == "Defend" && GetComponent<PlayerMovement>().playerIsGrounded()){
				defend = true;
			}
		}
	}

	//do a punch attack
	private void doPunchAttack() {
		playerState.SetState (PLAYERSTATE.PUNCH);
		animator.Punch (GetNextAttackNum ());
		LastAttackTime = Time.time;
	}

	//do a kick attack
	void doKickAttack() {
		playerState.SetState (PLAYERSTATE.KICK);
		animator.Kick (GetNextAttackNum ());
		LastAttackTime = Time.time;
	}

	//do jump kick attack
	void doJumpKickAttack(){
		playerState.SetState (PLAYERSTATE.JUMPKICK);
		jumpKickActive = true;
		animator.JumpKick();
		LastAttackTime = Time.time;
	}

	//start defending
	private void Defend(){
		playerState.SetState (PLAYERSTATE.DEFENDING);
		animator.StartDefend();
	}

	//returns the next attack number in the combo chain
	private int GetNextAttackNum() {
		if (playerState.currentState == PLAYERSTATE.PUNCH) {
			attackNum = Mathf.Clamp (attackNum += 1, 0, PunchAttackData.Length - 1);
			if (Time.time - LastAttackTime > PunchAttackData [attackNum].comboResetTime || !targetHit)
				attackNum = 0;
			return attackNum;

		} else if (playerState.currentState == PLAYERSTATE.KICK) {
			attackNum = Mathf.Clamp (attackNum += 1, 0, KickAttackData.Length - 1);
			if (Time.time - LastAttackTime > KickAttackData [attackNum].comboResetTime || !targetHit)
				attackNum = 0;
			return attackNum;
		}
		return 0;
	}

	//the attack is finished and the player is ready for new input
	public void Ready() {

		//continue to the next attack
		if (continuePunchCombo || continueKickCombo) {

			if (continuePunchCombo) { 
				doPunchAttack ();
				continuePunchCombo = false;

			} else if (continueKickCombo) { 
				doKickAttack ();
				continueKickCombo = false;
			} else {
				attackNum = 0;
			}

			//allow direction change during a combo or if we haven't hit anything
			if (ChangeDirDuringCombo || !targetHit) {
				GetComponent<PlayerMovement> ().updateDirection ();
			}

			//allow a direction change at the last attack of a combo
			if (playerState.currentState == PLAYERSTATE.PUNCH && ChangeDirAtLastHit && attackNum == PunchAttackData.Length - 1) { 
				GetComponent<PlayerMovement> ().updateDirection ();
			} else if (playerState.currentState == PLAYERSTATE.KICK && ChangeDirAtLastHit && attackNum == KickAttackData.Length - 1) { 
				GetComponent<PlayerMovement> ().updateDirection ();
			} 

		} else {

			//go back to idle
			playerState.SetState (PLAYERSTATE.IDLE);
		}
		jumpKickActive = false;
	}

	//checks if we have hit something (animation event)
	public void CheckForHit() {

		int dir = (int)GetComponent<PlayerMovement> ().getCurrentDirection ();
		Vector3 playerPos = transform.position + Vector3.up * 1.5f;
		LayerMask enemyLayerMask = LayerMask.NameToLayer ("Enemy");
		LayerMask itemLayerMask = LayerMask.NameToLayer ("Item");

		//do a raycast to see which enemies/objects are in attack range
		RaycastHit2D[] hits = Physics2D.RaycastAll (playerPos, Vector3.right * dir, getAttackRange(), 1 << enemyLayerMask | 1 << itemLayerMask);
		Debug.DrawRay (playerPos, Vector3.right * dir, Color.red, getAttackRange());

		//we have hit something
		for (int i = 0; i < hits.Length; i++) {

			LayerMask layermask = hits [i].collider.gameObject.layer;

			//we have hit an enemy
			if (layermask == enemyLayerMask) {
				GameObject enemy = hits [i].collider.gameObject;
				if (ObjInYRange (enemy)) {
					DealDamageToEnemy (hits [i].collider.gameObject);
					targetHit = true;
				}
			}

			//we have hit an item
			if (layermask == itemLayerMask) {
				GameObject item = hits [i].collider.gameObject;
				if (ObjInYRange (item)) {
					item.GetComponent<ItemInteractable> ().ActivateItem (gameObject);
					ShowHitEffectAtPosition (hits [i].point);
				}
			}
		}

		//we havent hit anything
		if(hits.Length == 0){ 
			targetHit = false;
		}
	}

	//returns true if the object is within y range
	bool ObjInYRange(GameObject obj) {
		float YDist = Mathf.Abs (obj.transform.position.y - transform.position.y);
		if (YDist < yHitDistance) {
			return true;
		} else {
			return false;
		}
	}

	//spawns the hit effect
	private void ShowHitEffectAtPosition(Vector3 pos) {
		GameObject.Instantiate (Resources.Load ("HitEffect"), pos, Quaternion.identity);
	}

	//deals damage to an enemy target
	private void DealDamageToEnemy(GameObject enemy) {
		DamageObject d = new DamageObject (0, gameObject);

		if (playerState.currentState == PLAYERSTATE.PUNCH) {
			d = PunchAttackData [attackNum];
		} else if (playerState.currentState == PLAYERSTATE.KICK) {
			d = KickAttackData [attackNum];
		} else if (playerState.currentState == PLAYERSTATE.THROWKNIFE) {
			d.damage = currentWeapon.data;
			d.attackType = AttackType.KnockDown;
		} else if(playerState.currentState == PLAYERSTATE.JUMPKICK) {
			d = JumpKickData;
			jumpKickActive = false; //hit only 1 enemy
		}

		d.inflictor = gameObject;

		//subsctract health from enemy
		HealthSystem hs = enemy.GetComponent<HealthSystem>();
		if(hs != null){
			hs.SubstractHealth (d.damage);
		}

		enemy.GetComponent<EnemyAI>().Hit (d);
	}

	//returns true is the player is facing the enemy
	public bool isFacingTarget(GameObject g) {
		DIRECTION dir = GetComponent<PlayerMovement> ().getCurrentDirection ();
		if ((g.transform.position.x > transform.position.x && dir == DIRECTION.Right) || (g.transform.position.x < transform.position.x && dir == DIRECTION.Left))
			return true;
		else
			return false;
	}

	//returns the attack range of the current attack
	private float getAttackRange() {
		if (playerState.currentState == PLAYERSTATE.PUNCH && attackNum <= PunchAttackData.Length) {
			return PunchAttackData [attackNum].range;
		} else if (playerState.currentState == PLAYERSTATE.KICK && attackNum <= KickAttackData.Length) {
			return KickAttackData [attackNum].range;
		} else if(jumpKickActive){
			return JumpKickData.range;
		} else {
			return 0f;
		}
	}

	//starts the throw weapon attack
	void StartThrowAttack() {
		playerState.SetState (PLAYERSTATE.THROWKNIFE);
		animator.Throw ();
		Invoke ("ThrowKnife", .08f);
		Destroy(weaponBone.GetChild(0).gameObject);
	}

	//spawns a throwing knife projectile
	public void ThrowKnife() {
		GameObject knife = GameObject.Instantiate (Resources.Load ("ThrowingKnife")) as GameObject;
		int dir = (int)GetComponent<PlayerMovement> ().getCurrentDirection ();
		knife.transform.position = transform.position + Vector3.up * 1.5f + Vector3.right * dir * .7f;
		knife.GetComponent<ThrowingKnife> ().ThrowKnife (dir);
		resetWeapon ();
	}

	//we are hit
	private void Hit(DamageObject d) {
		if (!HitStates.Contains (playerState.currentState)) {
		
			bool wasHit = true;
			UpdateHitCounter ();

			//defend
			if(playerState.currentState == PLAYERSTATE.DEFENDING){
				if(BlockAttacksFromBehind || isFacingTarget (d.inflictor)) wasHit = false;
				if(!wasHit){
					GlobalAudioPlayer.PlaySFX ("Defend");
					animator.ShowDefendEffect();
					animator.CamShakeSmall();

					if(isFacingTarget(d.inflictor)){ 
						animator.AddForce(-1.5f);
					} else {
						animator.AddForce(1.5f);
					}
				}
			}

			//knockdown hit
			if (HitKnockDownCount >= HitKnockDownThreshold) { 
				d.attackType = AttackType.KnockDown; 
			}

			//getting hit while being in the air also causes a knockdown
			if(!GetComponent<PlayerMovement>().playerIsGrounded()){
				d.attackType = AttackType.KnockDown; 
			}

			//we are dead
			if(GetComponent<HealthSystem>() != null && GetComponent<HealthSystem>().CurrentHp == 0){
				gameObject.SendMessage("Death");
			}

			//play hit SFX
			if(wasHit) GlobalAudioPlayer.PlaySFX ("PunchHit");

			//start knockDown sequence
			if(wasHit && playerState.currentState != PLAYERSTATE.KNOCKDOWN) {
				GetComponent<HealthSystem> ().SubstractHealth (d.damage);
				animator.ShowHitEffect ();

				if (d.attackType == AttackType.KnockDown) {
					playerState.SetState (PLAYERSTATE.KNOCKDOWN);
					StartCoroutine (KnockDown (d.inflictor));

				} else {
					playerState.SetState (PLAYERSTATE.HIT);
					animator.Hit ();
				}
			}
		}
	}

	//updates the hit counter
	void UpdateHitCounter() {
		if (Time.time - LastHitTime < HitKnockDownResetTime) { 
			HitKnockDownCount += 1;
		} else {
			HitKnockDownCount = 1;
		}
		LastHitTime = Time.time;
	}

	//equips a weapon
	public void EquipWeapon(Item weapon) {
		currentWeapon = weapon;

		if(weapon.itemName == "Knife"){ 
			GameObject knife = GameObject.Instantiate(Resources.Load("KnifeHandWeapon"), weaponBone.position, Quaternion.identity) as GameObject;
			knife.transform.parent = weaponBone;
			knife.transform.localPosition = Vector3.zero;
			knife.transform.localRotation = Quaternion.identity;
		}
	}

	//resetWeapon
	public void resetWeapon() {
		currentWeapon = null;
	}

	//interact with an item in range
	public void InteractWithItem(){
		if (itemInRange != null){
			Item item = itemInRange.GetComponent<ItemInteractable>().item;
			if (item != null && item.isPickup){
				itemInRange.GetComponent<ItemInteractable> ().ActivateItem (gameObject);
				animator.PickUpItem();
				playerState.SetState (PLAYERSTATE.PICKUPITEM);
			}
		}
	}

	//player knockDown coroutine
	public IEnumerator KnockDown(GameObject inflictor) {
		animator.KnockDown ();
		float t = 0;
		float travelSpeed = 2f;
		Rigidbody2D rb = GetComponent<Rigidbody2D> ();

		//get the direction of the attack
		int dir = inflictor.transform.position.x > transform.position.x ? 1 : -1;

		//look towards the direction of the incoming attack
		GetComponent<PlayerMovement>().LookToDir ((DIRECTION)dir);

		while (t < 1) {
			rb.velocity = Vector2.left * dir * travelSpeed;
			t += Time.deltaTime;
			yield return 0;
		}

		//stop traveling
		rb.velocity = Vector2.zero;
		yield return new WaitForSeconds (1);

		//reset
		playerState.currentState = PLAYERSTATE.IDLE;
		animator.Idle ();
		HitKnockDownCount = 0;
	}

	//returns the closest enemy in front of us
	GameObject GetClosestEnemyFacing() {
		List<GameObject> FacingEnemies = new List<GameObject> ();
		GameObject nearestEnemy = null;
		Vector3 playerTrans = transform.position;
		float distance = Mathf.Infinity;

		//generate a list of enemies standing in front of us (on the same line (y))
		for (int i = EnemyManager.enemyList.Count - 1; i > -1; i--) {
			GameObject enemy = EnemyManager.enemyList [i];
			if (enemy.activeSelf && isFacingTarget (enemy)) { 
				float YDist = Mathf.Abs (enemy.transform.position.y - playerTrans.y);
				if (YDist < yHitDistance)
					FacingEnemies.Add (enemy);
			}
		}

		//find closest enemy
		for (int i = 0; i < FacingEnemies.Count; i++) {
			float enemyDist = Mathf.Abs (FacingEnemies [i].transform.position.x - playerTrans.x);
			if (enemyDist < distance) { 
				distance = enemyDist;
				nearestEnemy = FacingEnemies [i];
			}
		}

		return nearestEnemy;
	}

	//the player has died
	void Death(){
		isDead = true;
		animator.Death();
		Invoke("GameOver", 2f);
		EnemyManager.PlayerHasDied();
	}

	//gameOver
	void GameOver(){
		OnPlayerDeath();
	}
}
