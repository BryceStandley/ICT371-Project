using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectInformationToolTip : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;

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

    private void HideTooltip()
    {
        gameObject.SetActive(false);

    }

    public static void ShowTip(string tooltipName, string tooltipStats)
    {
        instance.ShowTooltip(tooltipName, tooltipStats);
    }

    public static void HideTip()
    {
        instance.HideTooltip();
    }
}
