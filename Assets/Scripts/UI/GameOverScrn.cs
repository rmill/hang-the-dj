using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScrn : MonoBehaviour {

	public Text text;
	public Gradient ColorTransition;
	public float speed = 3.5f;
	public UIFader fader;
	private bool restartInProgress = false;

	void Start(){
		HideText();
	}

	void OnEnable() {
		PlayerCombat.OnPlayerDeath += ShowGameOverScrn;
		InputManager.onCombatInputEvent += InputEvent;
	}

	void OnDisable() {
		PlayerCombat.OnPlayerDeath -= ShowGameOverScrn;
		InputManager.onCombatInputEvent -= InputEvent;
	}

	void ShowGameOverScrn(){
		fader.Fade(UIFader.FADE.FadeOut, .5f, 1);
		Invoke("ShowText", 1.4f);
	}

	void ShowText(){
		text.gameObject.SetActive(true);
	}

	void HideText(){
		text.gameObject.SetActive(false);
	}

	//restart level on button press
	void InputEvent(string action){
		if(text.gameObject.activeSelf){ 
			RestartLevel();
		}
	}

	void Update(){

		//text effect
		if(text != null && text.gameObject.activeSelf){
			float t = Mathf.PingPong(Time.time * speed, 1f);
			text.color = ColorTransition.Evaluate(t);
		}

		//restart button pressed
		if(Input.GetMouseButtonDown(0) && text.gameObject.activeSelf){
			RestartLevel();
		}
	}

	//restarts the current level
	void RestartLevel(){
		if(!restartInProgress){
			restartInProgress = true;

			//sfx
			GlobalAudioPlayer.PlaySFX("ButtonStart");

			//button flicker
			ButtonFlicker bf =  GetComponentInChildren<ButtonFlicker>();
			if(bf != null) bf.StartButtonFlicker();

			Invoke("RestartScene", 1f);
		}
	}

	void RestartScene(){
		SceneManager.LoadScene("Game");
	}
}
