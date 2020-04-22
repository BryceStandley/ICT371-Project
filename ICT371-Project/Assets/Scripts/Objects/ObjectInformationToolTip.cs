using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectInformationToolTip : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;

    public GameObject promptObject;

    private static ObjectInformationToolTip instance;

    private void Awake()
    {
        instance = this;
        HideTip();
    }

    private void ShowTooltip(string tooltipName, string tooltipStats)
    {
        gameObject.SetActive(true);

        nameText.text = tooltipName;
        statsText.text = tooltipStats;

        
    }

    private void ShowToolTipPrompt()
    {
        promptObject.SetActive(true);
    }

    private void HideTooltip()
    {
        gameObject.SetActive(false);

    }

    private void HideToolTipPrompt()
    {
        promptObject.SetActive(false);
    }

    public static void ShowTip(string tooltipName, string tooltipStats)
    {
        instance.ShowTooltip(tooltipName, tooltipStats);
    }

    public static void ShowPrompt()
    {
        instance.ShowToolTipPrompt();
    }

    public static void HideTip()
    {
        instance.HideTooltip();
    }

    public static void HidePrompt()
    {
        instance.HideToolTipPrompt();
    }
}
