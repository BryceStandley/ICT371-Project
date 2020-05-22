using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Slider slider;
    public AudioClip washingCompleteSound;
    public AudioSource audioSource;
    
    public void UpdateSliderVal(float val)
    {
        slider.value = val;
        if(val == 100)
        {
            audioSource.clip = washingCompleteSound;
            audioSource.Play();
            Destroy(slider.gameObject);
        }
    }
}
