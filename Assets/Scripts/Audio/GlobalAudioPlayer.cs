using UnityEngine;
using System.Collections;

public static class GlobalAudioPlayer {

	public static AudioPlayer audioPlayer;

	public static void PlaySFX(string sfxName){
		if(audioPlayer != null && sfxName != "") audioPlayer.playSFX(sfxName);
	}

	public static void PlayMusic(string musicName){
		if(audioPlayer != null) audioPlayer.playMusic(musicName);
	}
}
