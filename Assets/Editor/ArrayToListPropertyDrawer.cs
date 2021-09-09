using System;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;

[CustomPropertyDrawer(typeof(ArrayToListAttribute))]
public class ArrayToListPropertyDrawer : PropertyDrawer {
    ArrayToListAttribute ArrayToList;

	SerializedProperty comparedField;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		if (!ShowMe(property))
			return 0f;

		// The height of the property should be defaulted to the default height.
		return base.GetPropertyHeight(property, label);
	}

	private bool ShowMe(SerializedProperty property) {
		ArrayToList = attribute as ArrayToListAttribute;
		if(ArrayToList.comparedValueArray != default && ArrayToList.comparedPropertyName != default) {
			string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, ArrayToList.comparedPropertyName) : ArrayToList.comparedPropertyName;
			bool showMe = false;

			comparedField = property.serializedObject.FindProperty(path);

			if (comparedField == null) {
				Debug.LogError("Cannot find property with name: " + path);
				return showMe;
			}

			foreach (object obj in ArrayToList.comparedValueArray) {
				if (showMe)
					break;
				switch (comparedField.type) {
					case "bool": {
							showMe = comparedField.boolValue.Equals(obj);
							continue;
						}
					case "Enum": {
							showMe = comparedField.enumValueIndex.Equals((int)obj);
							continue;
						}
				}
			}
			return showMe;
		} else {
			return true;
		}
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		ArrayToList = attribute as ArrayToListAttribute;

		string[] stringArray = new string[0];

		if (ArrayToList.MethodName == "GetChapterNames")
			stringArray = SentenceTools.GetChapterNames();

		if(ArrayToList.MethodName == "GetCharacterNames")
			stringArray = SentenceTools.GetCharacterNames();

		if(ArrayToList.MethodName == "GetCharacterVariants") {
			string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, ArrayToList.AdditionalVariable) : ArrayToList.AdditionalVariable;

			SerializedProperty AdditionalProperty;

			AdditionalProperty = property.serializedObject.FindProperty(path);
			stringArray = SentenceTools.GetCharacterVariants(AdditionalProperty.stringValue);
		}

		if (ArrayToList.MethodName == "GetCharacterStates") {
			string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, ArrayToList.AdditionalVariable) : ArrayToList.AdditionalVariable;

			SerializedProperty AdditionalProperty = property.serializedObject.FindProperty(path);

			path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, ArrayToList.SecondAdditionalVariable) : ArrayToList.SecondAdditionalVariable;

			SerializedProperty SecondAdditionalProperty = property.serializedObject.FindProperty(path);

			stringArray = SentenceTools.GetCharacterStates(AdditionalProperty.stringValue, SecondAdditionalProperty.stringValue);
		}

		if (ArrayToList.MethodName == "GetBGList")
			stringArray = SentenceTools.GetBGList();

		if (ArrayToList.MethodName == "GetCGList")
			stringArray = SentenceTools.GetCGList();

		if (ShowMe(property)) {
			if (stringArray.Length > 0) {
				int index = 0;

				if (property.type == "string") {
					index = Mathf.Max(0, Array.IndexOf(stringArray, property.stringValue));
					index = EditorGUI.Popup(position, property.displayName, index, stringArray);
				} else {
					index = property.intValue;
					if (ArrayToList.MethodName == "GetChapterNames" || ArrayToList.MethodName == "GetBGList" || ArrayToList.MethodName == "GetCGList") index = EditorGUI.Popup(position, property.displayName, index + 1, stringArray) - 1;
					else index = EditorGUI.Popup(position, property.displayName, index, stringArray);
				}
					

				if(property.type == "string")
					property.stringValue = stringArray[index];
				else
					property.intValue = index;
			} else {
				if (property.type == "string")
					property.stringValue = EditorGUI.TextField(position, property.stringValue);
				else
					property.intValue = EditorGUI.IntField(position, property.intValue);
			}
		}
	}
}
#endif