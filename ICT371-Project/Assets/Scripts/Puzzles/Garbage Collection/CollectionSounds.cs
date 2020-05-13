using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionSounds : MonoBehaviour
{
    public AudioClip CorrectItemSound;
    public AudioClip InCorrectItemSound;

    public AudioClip IndisposibleItemSound;

    private AudioSource audioSource;
    private void Start() 
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetAudioClipAndPlay(bool isCorrect, bool isIndisposible)
    {
        if(isIndisposible)
        {
            audioSource.clip = IndisposibleItemSound;
            audioSource.Play();
        }
        else if(isCorrect)
        {
            audioSource.clip = CorrectItemSound;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = InCorrectItemSound;
            audioSource.Play();
        }
    }
}
