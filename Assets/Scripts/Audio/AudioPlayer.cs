using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour {

	public AudioItem[] AudioList;
	private AudioSource source;
	private float musicVolume = 1f; //global volume multiplier
	private float sfxVolume = 1f; //global volume multiplier

	void Awake(){
		GlobalAudioPlayer.audioPlayer = this;
		source = GetComponent<AudioSource>();

		//set settings
		GameSettings settings = Resources.Load("GameSettings", typeof(GameSettings)) as GameSettings;
		if(settings != null){
			musicVolume = settings.MusicVolume;
			sfxVolume = settings.SFXVolume;
		}
	}

	public void playSFX(string name){
		foreach(AudioItem s in AudioList){
			if(s.name == name){
				int rand = Random.Range(0, s.clip.Length);
				source.PlayOneShot(s.clip[rand]);
				source.volume = s.volume * sfxVolume;
			}
		}
	}

	public void playMusic(string name){

		//create a separate gameobject designated for playing music
		GameObject music = new GameObject();
		music.name = "Music";
		AudioSource AS = music.AddComponent<AudioSource>();

		//get music track from audiolist
		foreach(AudioItem s in AudioList){
			if(s.name == name){
				AS.clip = s.clip[0];
				AS.loop = true;
				AS.volume = s.volume * musicVolume;
				AS.Play();
			}
		}
	}
}
