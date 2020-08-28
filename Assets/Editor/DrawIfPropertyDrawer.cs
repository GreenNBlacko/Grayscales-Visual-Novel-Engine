﻿using UnityEditor;
using UnityEngine;

/// <summary>
/// Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
/// </summary>
[CustomPropertyDrawer(typeof(DrawIfAttribute))]
public class DrawIfPropertyDrawer : PropertyDrawer
{
    #region Fields

    // Reference to the attribute on the property.
    DrawIfAttribute drawIf;

    // Field that is being compared.
    SerializedProperty comparedField;
    SerializedProperty comparedField2;

    #endregion

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShowMe(property) && drawIf.disablingType == DrawIfAttribute.DisablingType.DontDraw || !ShowMe2(property) && drawIf.disablingType == DrawIfAttribute.DisablingType.DontDraw)
            return 0f;

        // The height of the property should be defaulted to the default height.
        return base.GetPropertyHeight(property, label);
    }

    /// <summary>
    /// Errors default to showing the property.
    /// </summary>
    private bool ShowMe(SerializedProperty property)
    {
        drawIf = attribute as DrawIfAttribute;
        // Replace propertyname to the value from the parameter
        string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, drawIf.comparedPropertyName) : drawIf.comparedPropertyName;

        comparedField = property.serializedObject.FindProperty(path);

        if (comparedField == null)
        {
            Debug.LogError("Cannot find property with name: " + path);
            return true;
        }

        // get the value & compare based on types
        switch (comparedField.type)
        { // Possible extend cases to support your own type
            case "bool": {
                    if (comparedField.boolValue.Equals(drawIf.comparedValue) || drawIf.secondComparedValue == null)
                        return comparedField.boolValue.Equals(drawIf.comparedValue);
                    else
                        return comparedField.boolValue.Equals(drawIf.secondComparedValue);
                }
            case "Enum": {
                    if (comparedField.enumValueIndex.Equals((int)drawIf.comparedValue) || drawIf.secondComparedValue == null)
                        return comparedField.enumValueIndex.Equals((int)drawIf.comparedValue);
                    else
                        return comparedField.enumValueIndex.Equals((int)drawIf.secondComparedValue);
				}
                
            default:
                Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
                return true;
        }
    }

    private bool ShowMe2(SerializedProperty property) {
        drawIf = attribute as DrawIfAttribute;
        // Replace propertyname to the value from the parameter
        string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, drawIf.secondComparedPropertyName) : drawIf.secondComparedPropertyName;

        comparedField2 = property.serializedObject.FindProperty(path);

        if (comparedField2 == null || drawIf.secondComparedPropertyName == null) {
            return true;
        }

        // get the value & compare based on types
        switch (comparedField2.type) { // Possible extend cases to support your own type
            case "bool": {
                    return comparedField2.boolValue.Equals(drawIf.thirdComparedValue);
                }
            case "Enum": {
                    return comparedField2.enumValueIndex.Equals((int)drawIf.thirdComparedValue);
                }

            default:
                Debug.LogError("Error: " + comparedField2.type + " is not supported of " + path);
                return true;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // If the condition is met, simply draw the field.
        if (ShowMe(property) && ShowMe2(property))
        {
            EditorGUI.PropertyField(position, property);
        } //...check if the disabling type is read only. If it is, draw it disabled
        else if (drawIf.disablingType == DrawIfAttribute.DisablingType.ReadOnly)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property);
            GUI.enabled = true;
        }
    }

}