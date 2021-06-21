using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyActions : Enemy {

	public GameObject target; //the enemy target
	public RANGE range; //the range to target
	public ENEMYTACTIC enemyTactic;//the current tactic
	public ENEMYSTATE enemyState;
	public EnemyAnimator animator;
	public GameObject GFX;
	public SpriteRenderer Shadow;
	public Rigidbody2D rb;
	public bool isDead;
	public float YHitDistance = 0.4f; //the Y distance from which the enemy is able to hit the player
	private float MoveThreshold = 0.1f; //margin threshold before the enemy starts moving
	public float lastAttackTime; //the time of the last attack
	private bool attackWhilePlayerIsgrounded = true; //only attack when the player is on the ground, and not while he's jumping

	public void ATTACK() {
		if(target == null) SetTarget2Player();

		if(Time.time - lastAttackTime > attackInterval){

			//checks if we should only attack when the player is grounded
			if(attackWhilePlayerIsgrounded){

				if(target.GetComponent<PlayerMovement>().playerIsGrounded()){
					enemyState = ENEMYSTATE.ATTACK;
					Move(Vector3.zero, 0); //when commented the character runs to the other side
					LookAtTarget();
					animator.Attack1();
					lastAttackTime = Time.time;
				}

			} else {

					enemyState = ENEMYSTATE.ATTACK;
					Move(Vector3.zero, 0); //when commented the character runs to the other side
					LookAtTarget();
					animator.Attack1();
					lastAttackTime = Time.time;
			}
		}
	}

	//move to the target
	public void MoveTo(float distance, float speed){
		LookAtTarget();

		//horizontal movement
		if (Mathf.Abs(DistanceToTargetX()-distance) > MoveThreshold){

			//move closer on horizontal line
			Move(Vector3.right * (int)Dir2DistPoint(distance), speed);

		} else if (Mathf.Abs(DistanceToTargetY()-distance) > MoveThreshold){

			//move closer on vertical line
			Move(Vector3.up * DirToVerticalLine(), speed/1.5f);
		} 
	}

	//returns the direction to the distance point
	DIRECTION Dir2DistPoint(float distance){
		if(target == null) SetTarget2Player();

		//go Left
		if(transform.position.x < target.transform.position.x){
			float distancepointX = target.transform.position.x - distance;
			if(transform.position.x < distancepointX) 
				return DIRECTION.Right;
			else 
				return DIRECTION.Left;
		} else {

		//go Right
			float distancepointX = target.transform.position.x + distance;
			if(transform.position.x > distancepointX) 
					return DIRECTION.Left;
				else 
					return DIRECTION.Right;
		}
	}

	//set the layer order so this enemy appears behind or in front of other objects
	public void UpdateSpriteSorting(){
		GFX.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y*-10f); //100 is multiplied to spraid out the values
		Shadow.sortingOrder = GFX.GetComponent<SpriteRenderer>().sortingOrder-1;
	}

	//flips the sprite towards the current target
	public void LookAtTarget()	{
		if(target != null){
			if(transform.position.x >= target.transform.position.x){
				GFX.GetComponent<SpriteRenderer>().flipX = true;
			} else {
				GFX.GetComponent<SpriteRenderer>().flipX = false;
			}
		}
	}

	//returns the current direction
	public DIRECTION getCurrentDirection(){
		if(GFX.GetComponent<SpriteRenderer>().flipX){
			return DIRECTION.Left;
		} else 
			return DIRECTION.Right;
	}

	//return the direction to the wanted position
	public DIRECTION DirectionToPos(float xPos){
		if (transform.position.x >= xPos) return DIRECTION.Left;
		return DIRECTION.Right;
	}

	//return the direction to the target on a vertical ine
	int DirToVerticalLine(){
		if(transform.position.y > target.transform.position.y) 
			return -1;
		else 
			return 1;
	}

	//idle
	public void IDLE()	{
		Move(Vector3.zero, 0);
		enemyState = ENEMYSTATE.IDLE;
		animator.Idle();
	}

	//move towards a vector
	public void Move(Vector3 vector, float speed){
		rb.velocity = vector * speed;

		if(speed>0) 
			animator.Walk();
		else
			animator.Idle();
	}

	//wait for an x number of seconds
	public IEnumerator Wait(float delay)	{
		yield return new WaitForSeconds (delay);
		enemyState = ENEMYSTATE.IDLE;
	}

	//x distance to the target
	public float DistanceToTargetX(){
		if(target != null)
			return Mathf.Abs(transform.position.x - target.transform.position.x);
		else
			return -1;
	}

	//y distance to the target
	public float DistanceToTargetY(){
		if(target != null)
			return Mathf.Abs(transform.position.y - target.transform.position.y);
		else
			return -1;
	}

	//check if the attack hits and deal damage to the target
	public void CheckForHit(){
		if(DistanceToTargetX() < attackRange && DistanceToTargetY()<YHitDistance){ 
			DealDamageToTarget();
		}
	}

	//deal damage to the current target
	void DealDamageToTarget(){
		if(target != null){
			DamageObject d = new DamageObject(attackDamage, gameObject);
			d.attackType = AttackType.Default;
			target.SendMessage("Hit", d, SendMessageOptions.DontRequireReceiver);
		}
	}

	//enemy knockDown
	public IEnumerator KnockDown(DIRECTION dir){
		float t=0;
		float travelSpeed = 2f;

		//play knockdown animation
		animator.KnockDown();

		//knock down air
		while(t<1 && !isDead){
			rb.velocity = Vector2.left * (int)dir * travelSpeed;
			t += Time.deltaTime;
			yield return 0;
		}

		//knock down grounded
		rb.velocity = Vector2.zero;
		enemyState = ENEMYSTATE.KNOCKDOWNGROUNDED;
	}

	//remove enemy from the playing field with a flicker effect
	public IEnumerator RemoveEnemy(){

		float osc;
		float speed = 3f;
		float startTime = Time.time;

		//pause before flickering start
		yield return new WaitForSeconds(1.5f);

		while(true){
			osc = Mathf.Sin((Time.time - startTime) * speed);
			speed += Time.deltaTime * 10;

			//Switch between sprites
			GFX.GetComponent<SpriteRenderer>().enabled = (osc>0);

			if(speed > 35) DestroyUnit();
			yield return null;
		}
	}

	//show hit effect
	public void ShowHitEffectAtPosition(Vector3 pos) {
		GameObject.Instantiate (Resources.Load ("HitEffect"), pos, Quaternion.identity);
	}

	//set the player as target
	public void SetTarget2Player(){
		target = GameObject.FindGameObjectWithTag("Player");
	}

	//randomizes values for variation
	public void RandomizeValues(){
		walkSpeed *= Random.Range(.8f, 1.2f);
		attackInterval *= Random.Range(.8f, 1.2f);
	}
}