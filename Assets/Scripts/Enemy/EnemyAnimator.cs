using UnityEngine;
using System.Collections;

public class EnemyAnimator : MonoBehaviour {

	private Animator animator;
	private AudioPlayer audioPlayer;
	private bool animatorActive;

	void Awake(){
		animator = GetComponent<Animator>();
	}

	public void Idle(){
		if(animator.isInitialized){ 
			animator.SetTrigger("Idle");
		}
	}

	public void Walk(){
		if(animator.isInitialized){ 
			animator.SetTrigger("Walk");
		}
	}

	public void Attack1(){		
		if(animator.isInitialized){ 
			animator.SetTrigger("Attack1");
			CancelInvoke();
			Invoke("WaitForAnimationFinish", getAnimationLength("Enemy_Attack1"));
		}
	}

	public void Hit(){		
		if(animator.isInitialized){ 
			animator.SetTrigger("Hit");
			CancelInvoke();
			Invoke("WaitForAnimationFinish", getAnimationLength("Enemy_Hit"));
		}
	}

	public void Death(){
		if(animator.isInitialized){ 
			animator.SetTrigger("Death");
		}
	}

	public void KnockDown(){
		if(animator.isInitialized) {
			animator.SetTrigger("KnockDown");
			CancelInvoke();
			Invoke("WaitForAnimationFinish", getAnimationLength("Enemy_KnockDown"));
		}
	}

	public void Check4Hit(){
		transform.parent.GetComponent<EnemyActions>().CheckForHit();
	}

	public void WaitForAnimationFinish(){
		transform.parent.GetComponent<EnemyAI>().IDLE();
	}

	public void PlaySFX(string name){
		GlobalAudioPlayer.PlaySFX(name);
	}

	public void CamShakeSmall(){
		Camera.main.GetComponent<CameraFollow>().CamShakeSmall();
	}

	//returns the length (time) of an animation
	float getAnimationLength(string animName){
		if(animator.isInitialized){
			RuntimeAnimatorController ac = animator.runtimeAnimatorController;
			for(int i = 0; i<ac.animationClips.Length; i++){
				if(ac.animationClips[i].name == animName){
					return ac.animationClips[i].length;
         		}
         	}
		}
		return 0;
	}
}
