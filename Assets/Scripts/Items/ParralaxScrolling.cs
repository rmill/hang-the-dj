using UnityEngine;
using System.Collections;

public class ParralaxScrolling : MonoBehaviour {

	public Transform mainCam;
	public float parralaxValue = -0.3f;
	private float lastCamX;
	private float currentCamX;

	void Start(){
		mainCam = Camera.main.transform;
		if(mainCam == null) Debug.Log("no camera specified");
	}

	void LateUpdate() {
		if(mainCam != null){
			currentCamX = mainCam.position.x;
			transform.position = new Vector3(transform.position.x + (currentCamX - lastCamX) * parralaxValue , transform.position.y, transform.position.z);
			lastCamX = currentCamX;
		}
	}
}
