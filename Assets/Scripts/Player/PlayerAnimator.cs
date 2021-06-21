using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
public class PlayerAnimator : MonoBehaviour {

	private Animator animator;
	private AudioPlayer audioPlayer;

	void Awake() {
		animator = GetComponent<Animator> ();
	}

	public void Idle() {
		animator.SetTrigger ("Idle");
		animator.SetBool("Walk", false);
	}

	public void Walk() {
		animator.SetBool("Walk", true);
	}

	public void Punch(int id) {
		animator.SetTrigger ("Punch" + id);
		StartCoroutine (WaitForAnimationFinish ("Punch" + id));
	}

	public void Kick(int id) {
		animator.SetTrigger ("Kick" + id);
		StartCoroutine (WaitForAnimationFinish ("Kick" + id));
	}

	public void GroundPunch() {
		animator.SetTrigger ("GroundPunch");
		StartCoroutine (WaitForAnimationFinish ("GroundPunch"));
	}

	public void JumpKick() {		
		animator.SetTrigger ("JumpKick");
		StartCoroutine (WaitForAnimationFinish ("JumpKick"));
	}

	public void StartDefend() {		
		animator.SetBool ("Defend", true);
	}

	public void StopDefend() {		
		animator.SetBool ("Defend", false);
	}

	//check if the attack has hit
	public void Check4Hit() {
		transform.parent.GetComponent<PlayerCombat>().CheckForHit ();
	}

	public void Jump() {
		animator.SetBool("Walk", false);
		animator.SetTrigger ("Jump");
		StartCoroutine (WaitForAnimationFinish ("Jump"));
	}

	public void Hit() {
		animator.SetTrigger ("Hit");
		StartCoroutine (WaitForAnimationFinish ("Hit"));
	}

	public void Death() {
		animator.SetTrigger ("Death");
	}

	public void PickUpItem() {
		animator.SetTrigger ("Pickup");
		StartCoroutine (WaitForAnimationFinish ("Pickup"));
	}

	public void Throw() {		
		animator.SetTrigger ("Throw");
		StartCoroutine (WaitForAnimationFinish ("Throw"));
	}

	public void KnockDown() {
		animator.SetTrigger ("KnockDown");
		StartCoroutine (WaitForAnimationFinish ("KnockDown"));
	}

	//show hit effect
	public void ShowHitEffect() {
		GameObject.Instantiate (Resources.Load ("HitEffect"), transform.position, Quaternion.identity);
	}

	//show defend effect
	public void ShowDefendEffect() {
		Vector3 offset = Vector3.up * 1.7f + Vector3.right * (int)transform.parent.GetComponent<PlayerMovement> ().getCurrentDirection () * .2f;
		GameObject.Instantiate (Resources.Load ("DefendEffect"), transform.position + offset, Quaternion.identity);
	}

	//Show dust effect
	public void ShowDustEffect() {
		GameObject.Instantiate (Resources.Load ("SmokePuffEffect"), transform.position, Quaternion.identity);
	}

	//play audio
	public void PlaySFX(string sfxName) {
		GlobalAudioPlayer.PlaySFX (sfxName);
	}

	//camera shake
	public void CamShakeSmall() {
		Camera.main.GetComponent<CameraFollow> ().CamShakeSmall ();
	}

	//adds a small forward force
	public void AddForce(float force) {
		StartCoroutine (AddForceCoroutine(force));
	}

	//adds small force over time
	IEnumerator AddForceCoroutine(float force) {
		Rigidbody2D rb = transform.parent.GetComponent<Rigidbody2D> ();
		DIRECTION dir = transform.parent.GetComponent<PlayerMovement> ().getCurrentDirection ();
		float speed = 2f;
		float t = 0;

		while (t < 1) {
			rb.velocity = Vector2.right * (int)dir * Mathf.Lerp (force, 0, MathUtilities.Sinerp (0, 1, t));
			t += Time.deltaTime * speed;
			yield return null;
		}
	}

	//on animation finish
	IEnumerator WaitForAnimationFinish(string animName) {
		float time = GetAnimDuration(animName);
		yield return new WaitForSeconds(time);
		transform.parent.GetComponent<PlayerCombat>().Ready();
	}

	//returns the duration of an animation
	float GetAnimDuration(string animName) {
		RuntimeAnimatorController ac = animator.runtimeAnimatorController;
		for (int i = 0; i < ac.animationClips.Length; i++) {
			if (ac.animationClips [i].name == animName) {
				return ac.animationClips [i].length;
			}
		}
		print ("no animation found with name: " + animName);
		return 0f;
	}
}
