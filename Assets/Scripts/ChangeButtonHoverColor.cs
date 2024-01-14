using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonHoverColor : MonoBehaviour
{
    //[SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Color wantedColor;
    private Color originalColor;
    //private ColorBlock cb;
    
    private void Start()
    {
        //cb = button.colors;
        //originalColor = cb.selectedColor;
        originalColor = buttonText.color;
    }

    public void OnHoverChange()
    {
        //cb.selectedColor = wantedColor;
        //button.colors = cb;
        buttonText.color = wantedColor;
    }

    public void OnExitHoverChange()
    {
        //cb.selectedColor = originalColor;
        //button.colors = cb;
        buttonText.color = originalColor;
    }
}
