#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DrawIfAnyAttribute))]
public class DrawIfAnyPropertyDrawer : PropertyDrawer {
	DrawIfAnyAttribute drawIf;

	SerializedProperty comparedField;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		if (!ShowMe(property))
			return 0f;

		// The height of the property should be defaulted to the default height.

		return base.GetPropertyHeight(property, label);
	}

	private bool ShowMe(SerializedProperty property) {
		drawIf = attribute as DrawIfAnyAttribute;
		string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, drawIf.comparedPropertyName).Replace("choiceOptions.Array.", "") : drawIf.comparedPropertyName;
		bool showMe = false;

		comparedField = property.serializedObject.FindProperty(path);

		if (comparedField == null) {
			Debug.LogError("Cannot find property with name: " + path);
			return showMe;
		}

		foreach (object obj in drawIf.comparedValueArray) {
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
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		if (ShowMe(property)) {
			if (drawIf.slider) {
				property.floatValue = EditorGUI.Slider(position, label, property.floatValue, 2, 0);
			} else {
				string path = System.IO.Path.ChangeExtension(property.propertyPath, drawIf.comparedPropertyName).Replace(".Array." + drawIf.comparedPropertyName, "");

				SerializedProperty array = property.serializedObject.FindProperty(path);

				Rect labelPosition = new Rect { x = position.x + 73, y = position.y, width = position.width, height = position.height };

				EditorGUI.BeginProperty(position, label, array);

				drawIf.IsActive = EditorGUI.BeginFoldoutHeaderGroup(labelPosition, drawIf.IsActive, label);
				//var arraySizeFieldRect = 



				EditorGUI.EndFoldoutHeaderGroup();

				EditorGUI.EndProperty();
			}
		}
	}


}
#endif