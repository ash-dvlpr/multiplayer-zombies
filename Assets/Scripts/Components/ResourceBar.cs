using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ResourceBar : MonoBehaviour {

    // ====================== References =====================

    [Header("Customization")]
    [SerializeField] Color borderColor;
    [SerializeField] Color backgroundColor;
    [SerializeField] Color barColor;

    [Header("Configuration")]
    [SerializeField] Resource trackedResource;
    [SerializeField] bool showValues;

    [Header("References")]
    [SerializeField] Image border;
    [SerializeField] Image background;
    [SerializeField] Image bar;
    [SerializeField] TMP_Text displayText;
    Slider barSlider;

    // ====================== Unity Code ======================
    void Awake() {
        barSlider = GetComponent<Slider>();
        ReloadBar();
    }

    void OnEnable() {
        if (trackedResource) trackedResource.OnChange += OnResourceChanged;
    }

    void OnDisable() {
        if (trackedResource) trackedResource.OnChange -= OnResourceChanged;
    }

#if UNITY_EDITOR
    void OnValidate() {
        ReloadBar();
    }
#endif

    // ===================== Custom Code =====================
    void ReloadBar() {
        if (displayText) displayText.enabled = showValues;

        border.color = borderColor;
        background.color = backgroundColor;
        bar.color = barColor;
    }

    void OnResourceChanged(Resource resource) {
        var percent = resource.Amount / (float) resource.Max;
        barSlider.value = percent;
        if (showValues) {
            displayText.text = $"{resource.Amount} / {resource.Max}";
        }
    }
}
