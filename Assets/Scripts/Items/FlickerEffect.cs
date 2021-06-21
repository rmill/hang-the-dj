using UnityEngine;
using System.Collections;

public class FlickerEffect : MonoBehaviour {

	public float flickerSpeed = 40f;
	public float flickerDuration = 3f;
	public GameObject[] GFX;

	public void Start () {
		StartCoroutine(FlickerCoroutine());
	}
	
	IEnumerator FlickerCoroutine(){
		float t =0;

		while(t < flickerDuration){
			float i = Mathf.Sin(Time.time * flickerSpeed);
			foreach(GameObject g in GFX) g.SetActive(i>0);
			t += Time.deltaTime;
			yield return null;
		}

		foreach(GameObject g in GFX) g.SetActive(false);
	}
}
