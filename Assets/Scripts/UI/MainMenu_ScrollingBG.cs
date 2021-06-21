using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu_ScrollingBG : MonoBehaviour {

	private RawImage rawImage;
	public float scrollSpeed = .01f;

	void Awake(){
		rawImage = GetComponent<RawImage>();
	}

	void Update(){
		if(rawImage != null){ 
			rawImage.uvRect = new Rect(Vector2.right * Time.time * scrollSpeed, rawImage.uvRect.size);
		}
	}
}
