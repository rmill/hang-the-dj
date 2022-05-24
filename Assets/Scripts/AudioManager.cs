using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] songs;
   
    void Start()
    {
        // Choose a song by random to start
        int index = Random.Range(0, songs.Length);
        AudioClip song = songs[index];
        
        // Set the song to play
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.volume = 0.5f;
        audioSource.clip = song;
        audioSource.Play();
    }
}
