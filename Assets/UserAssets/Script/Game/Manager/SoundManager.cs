using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static AudioSource mAudioSource;

    private void Awake() => mAudioSource = GetComponent<AudioSource>();

    public static void Play(AudioClip audioClip)
    {
        mAudioSource.clip = audioClip;
        mAudioSource.Play();
    }

    public static void PlayOneShot(AudioClip audioClip) 
        =>mAudioSource.PlayOneShot(audioClip);
    
    public static bool IsPlaying() 
        => mAudioSource.isPlaying;
}
