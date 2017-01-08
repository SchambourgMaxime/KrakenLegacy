using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

    public AudioClip dashSound;
    public AudioClip nomNomSound;
    public AudioClip fullBarSound;
    public AudioClip destructionWallSound;
    public AudioClip takeDamage;
    public AudioClip death;

    private AudioSource playerAudioSource;

	// Use this for initialization
	void Start ()
    {
        playerAudioSource = GetComponent<AudioSource>();
	}
	
    public void PlayDashSound()
    {
        playerAudioSource.clip = dashSound;
        playerAudioSource.pitch = 1.0f;
        playerAudioSource.volume = 0.2f;
        playerAudioSource.Play();
    }

    public void PlayNomNomSound()
    {
        playerAudioSource.clip = nomNomSound;
        playerAudioSource.pitch = 1.0f;
        playerAudioSource.volume = 0.5f;
        playerAudioSource.Play();
    }

    public void PlayFullBarSound(float pitch)
    {
        playerAudioSource.clip = fullBarSound;
        playerAudioSource.pitch = pitch;
        playerAudioSource.volume = 1.0f;
        playerAudioSource.Play();
    }

    public void PlayWallDestruction()
    {
        playerAudioSource.clip = destructionWallSound;
        playerAudioSource.pitch = 1.0f;
        playerAudioSource.volume = 1.0f;
        playerAudioSource.Play();
    }

    public void PlayTakeDamage()
    {
        playerAudioSource.clip = takeDamage;
        playerAudioSource.pitch = 1.5f;
        playerAudioSource.volume = 0.5f;
        playerAudioSource.Play();
    }

    public void PlayDeath()
    {
        playerAudioSource.clip = takeDamage;
        playerAudioSource.pitch = 0.4f;
        playerAudioSource.volume = 0.5f;
        playerAudioSource.Play();
    }
}
