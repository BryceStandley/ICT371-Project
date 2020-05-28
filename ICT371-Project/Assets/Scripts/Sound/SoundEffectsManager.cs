using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager instance;
    public AudioSource mainAudioSource;

    #region Sound Effects Variables
    public AudioClip objectiveCompleteClip;
    public AudioClip correctBinItemClip;
    public AudioClip incorrectBinItemClip;
    public AudioClip inDisposableBinItemClip;
    public AudioClip washingCompleteClip;
    public AudioClip actionClickClip;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    public void PlayObjectiveCompleteClip()
    {
        mainAudioSource.clip = objectiveCompleteClip;
        mainAudioSource.Play();
    }

    public void PlayCorrectBinItemClip()
    {
        mainAudioSource.clip = correctBinItemClip;
        mainAudioSource.Play();
    }

    public void PlayIncorrectBinItemClip()
    {
        mainAudioSource.clip = incorrectBinItemClip;
        mainAudioSource.Play();
    }

    public void PlayInDisposableBinItemClip()
    {
        mainAudioSource.clip = inDisposableBinItemClip;
        mainAudioSource.Play();
    }

    public void PlayWashingCompleteClip()
    {
        mainAudioSource.clip = washingCompleteClip;
        mainAudioSource.Play();
    }

    public void PlayActionClickClip()
    {
        mainAudioSource.clip = actionClickClip;
        mainAudioSource.Play();
    }
}
