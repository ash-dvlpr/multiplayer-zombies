using System.Reflection;
using System;
using UnityEditor;
using UnityEngine;
using Cinemachine.Editor;
using System.Linq;
using CustomExtensions.Collections;

[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldAttributeDrawer : PropertyDrawer {
    private bool _toShow = true;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        // Don't really know why, but skip for if the attribute is not from our custom propperty
        if (!( attribute is ConditionalFieldAttribute conditional )) return EditorGUI.GetPropertyHeight(property);

        //? Set the _toShow flag if the field contains one of the compareValues
        // Get the "fieldToCheck" field
        var propertyPath = property.propertyPath;
        var lastDot = propertyPath.LastIndexOf(".");
        var fieldToCheckPath = $"{propertyPath[..lastDot]}.{conditional.fieldToCheck}";
        var fieldToCheck = property.serializedObject.FindProperty(fieldToCheckPath);

        // Check the "compareValues"
        _toShow = false;
        switch (fieldToCheck.type) {
            case "Enum":
                var value = fieldToCheck.enumValueIndex;
                //Debug.Log($"Value: {value} -> {conditional.compareValues.ToDebugString()}");

                _toShow = conditional.compareValues.Contains(value.ToString());
                break;
            default:
                Debug.LogError($"ConditionalFieldAttributeDrawer: Unsupported field type: '{fieldToCheck.type}'");
                break;
        }

        // Invert if set
        _toShow = conditional.inverse ? !_toShow : _toShow;

        // Look into this if you wanna extend this (change to reflexion)
        // https://forum.unity.com/threads/get-a-general-object-value-from-serializedproperty.327098/

        if (!_toShow) return -2;
        else return EditorGUI.GetPropertyHeight(property, label);

        //if (_customPropertyDrawer != null) return _customPropertyDrawer.GetPropertyHeight(property, label);
        //return EditorGUI.GetPropertyHeight(property);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (!_toShow) return;

        EditorGUI.PropertyField(position, property, label, true);
    }
}