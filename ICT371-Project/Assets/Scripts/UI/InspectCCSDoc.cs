using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InspectCCSDoc : MonoBehaviour
{
    public GameObject CCSUI;

    private void Awake()
    {
        CCSUI.SetActive(false);
    }
    public void OpenCCS1UI()
    {
        CCSUI.SetActive(true);
        PauseMenu.instance.inDialogue = true;
        PlayerInputController.instance.DisablePlayerControls();
    }

    private void HideBuyUI()
    {
        PauseMenu.instance.inDialogue = false;
        PlayerInputController.instance.EnablePlayerControls();
    }
}
