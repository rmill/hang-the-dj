using UnityEngine;
using System.Collections;
using System;

public class GameSettings : ScriptableObject {

	[Header ("Application Settings")]
	public int framerate = 60;
	public float timeScale = 1f;

	[Header ("Audio Settings")]
	public float MusicVolume = .7f;
	public float SFXVolume = .9f;

	[Header ("Game Settings")]
	public  bool CameraBacktrack = false; //most beat em up games have a camera that only allows you to go forward. Turn on/off Camerabacktrack if you want to allow the camera to go back.
	public int MaxAttackers  = 3; //the maximum number of enemies that can attack the player simultaneously 
}