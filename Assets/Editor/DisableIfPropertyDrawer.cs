using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisableIfAttribute))]
public class DisableIfPropertyDrawer : PropertyDrawer {

	DisableIfAttribute disableIf;
	SerializedProperty comparedField;
	SerializedProperty tmpValue;
	SerializedProperty tmpBool;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		if (!ShowMe(property))
			return 0f;

		// The height of the property should be defaulted to the default height.
		return base.GetPropertyHeight(property, label);
	}

	private bool ShowMe(SerializedProperty property) {
		disableIf = attribute as DisableIfAttribute;
		// Replace propertyname to the value from the parameter
		string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, disableIf.comparedPropertyName) : disableIf.comparedPropertyName;

		comparedField = property.serializedObject.FindProperty(path);

		if (comparedField == null) {
			Debug.LogError("Cannot find property with name: " + path);
			return true;
		}

		// get the value & compare based on types
		switch (comparedField.type) { // Possible extend cases to support your own type
			case "bool": {
					return comparedField.boolValue.Equals(disableIf.comparedValue);
				}
			case "Enum": {
					return comparedField.enumValueIndex.Equals((int)disableIf.comparedValue);
				}

			default:
				Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
				return true;
		}
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		disableIf = attribute as DisableIfAttribute;

		string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, disableIf.tmpValueName) : disableIf.tmpValueName;

		tmpValue = property.serializedObject.FindProperty(path);

		string path2 = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, disableIf.tmpBoolName) : disableIf.tmpBoolName;

		tmpBool = property.serializedObject.FindProperty(path2);

		if (ShowMe(property)) {
			EditorGUI.PropertyField(position, property);
			if (!tmpBool.boolValue) {
				property.boolValue = tmpValue.boolValue;
				tmpBool.boolValue = true;
			}
		} else {
			if (tmpBool.boolValue) {
				tmpValue.boolValue = property.boolValue;
				property.boolValue = true;
				tmpBool.boolValue = false;
			}
		}
	}
}
