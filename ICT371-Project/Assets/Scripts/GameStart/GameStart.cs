using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public Animation blackFadeAnimation;
    public GameObject blackFadeImage;
    public Dialogue openingGameDialogue;

    private void Start()
    {
        blackFadeImage.GetComponent<Image>().color = new Color(0,0,0,255);
        blackFadeAnimation.Play("blackFadeOut");
        Invoke("TriggerDialogue", 1.3f);
    }

    private void TriggerDialogue()
    {
        DialogueManager.instance.StartDialogue(openingGameDialogue);
        blackFadeImage.SetActive(false);

    }
}
