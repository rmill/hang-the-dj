using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

	[HideInInspector]
	public float AdditionalOffset;

	//cam shake presets
	public void CamShakeSmall(){
		StopAllCoroutines();
		StartCoroutine( DoCamShake(40f, 0.06f, 0.9f));
	}

	public void CamShakeMedium(){
		StopAllCoroutines();
		StartCoroutine( DoCamShake(50f, 0.10f, 1.1f));
	}

	public void CamShakeBig(){
		StopAllCoroutines();
		StartCoroutine( DoCamShake(50f, 0.18f, 1.3f));
	}

	//camera shake coroutine
	IEnumerator DoCamShake(float speed, float intensity, float duration){
		float t=0;
		float dampValue = 1f;
		AdditionalOffset = 0;
		while (t<1){
			AdditionalOffset = Mathf.Sin(t*speed) * intensity * dampValue;
			dampValue = MathUtilities.Sinerp(1f, 0f, t);
			t += Time.deltaTime / duration;
			yield return null;
		}
		AdditionalOffset = 0;
	}
}
