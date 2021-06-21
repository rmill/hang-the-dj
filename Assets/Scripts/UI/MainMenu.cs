using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public UIFader UI_Fader;
	private GameSettings settings;
	private bool startGameInProgress = false;

	void OnEnable() {
		InputManager.onCombatInputEvent += InputEvent;
	}

	void OnDisable() {
		InputManager.onCombatInputEvent -= InputEvent;
	}

	void InputEvent(string action){
			StartGame();
	}

	void Start(){

		//Start fade in
		Invoke("FadeIn", 1f);
	}

	public void StartGame(){
		if(!startGameInProgress){

			startGameInProgress = true;

			//play sfx
			GlobalAudioPlayer.PlaySFX("ButtonStart");

			//flicker button
			ButtonFlicker bf =  GetComponentInChildren<ButtonFlicker>();
			if(bf != null) bf.StartButtonFlicker();

			//fade out
			FadeOut();

			//start Game
			Invoke("startGame", 1.8f);
		}
	}

	void FadeIn(){
		UI_Fader.Fade(UIFader.FADE.FadeIn, .5f, 0f);
	}

	void FadeOut(){
		UI_Fader.Fade(UIFader.FADE.FadeOut, .5f, 1f);
	}

	//Start game
	void startGame(){
		FadeIn();
		gameObject.SetActive(false);

		//start 1st enemy wave
		EnemyWaveSystem EWS = GameObject.FindObjectOfType<EnemyWaveSystem>();
		if(EWS != null) EWS.OnLevelStart();
	}
}