using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoundClearScrn : MonoBehaviour {

	public Text text;
	public Gradient ColorTransition;
	public float speed = 3.5f;
	public UIFader fader;
	private bool restartInProgress = false;

	void Start(){
		HideText();
	}

	void OnEnable() {
		EnemyWaveSystem.onLevelComplete += ShowLevelCompleteScrn;
		InputManager.onCombatInputEvent += InputEvent;
	}

	void OnDisable() {
		EnemyWaveSystem.onLevelComplete -= ShowLevelCompleteScrn;
		InputManager.onCombatInputEvent -= InputEvent;
	}

	void ShowLevelCompleteScrn(){
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
