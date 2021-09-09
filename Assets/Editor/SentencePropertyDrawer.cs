#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Sentence))]
public class SentencePropertyDrawer : PropertyDrawer {
	int ActiveElements = 0;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		ActiveElements = 1;

		var CharacterName = property.FindPropertyRelative("Name");

		var text = property.FindPropertyRelative("Text");

		EditorGUI.BeginProperty(position, label, property);

		label.text = "Sentence " + label.text.Replace("Element ", "") + " (" + CharacterName.stringValue + ")";

		if (text.stringValue.Split('\n')[0].Length > 16 && text.stringValue.Split('\n').Length > 0) {
			label.text += " [" + text.stringValue.Split('\n')[0].Substring(0, 15) + "...]";
		} else {
			if(text.stringValue.Length > 16) {
				label.text += " [" + text.stringValue.Substring(0, 15) + "...]";
			} else {
				label.text += " [" + text.stringValue + "]";
			}
		}

		Rect labelPosition = new Rect { x = position.x, y = position.y, width = position.width, height = 16 };

		property.isExpanded = EditorGUI.Foldout(labelPosition, property.isExpanded, label);

		if(property.isExpanded) {
			EditorGUI.indentLevel++;

			var CharacterNameRect = new Rect(position.x, position.y + 16 * ActiveElements, position.width, 16);

			EditorGUI.PropertyField(CharacterNameRect, CharacterName);
			ActiveElements++;

			var OverrideNameRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

			var OverrideName = property.FindPropertyRelative("OverrideName");

			EditorGUI.PropertyField(OverrideNameRect, OverrideName);
			ActiveElements++;

			if (OverrideName.boolValue) {
				var DisplayNameRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

				var DisplayName = property.FindPropertyRelative("DisplayName");

				EditorGUI.PropertyField(DisplayNameRect, DisplayName);
				ActiveElements++;
			}

			int lines = text.stringValue.Split('\n').Length;

			lines = (lines > 3) ? (lines < 15) ? lines : 15 : 3;

			var textRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 15 * lines + 24);

			EditorGUI.PropertyField(textRect, text, new GUIContent { text = text.displayName, tooltip = text.tooltip });
			position.y += 15 * lines + 26;

			var transitionRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

			var transition = property.FindPropertyRelative("transition");

			EditorGUI.PropertyField(transitionRect, transition);
			ActiveElements++;

			var ChoiceRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

			var Choice = property.FindPropertyRelative("Choice");

			EditorGUI.PropertyField(ChoiceRect, Choice);
			ActiveElements++;

			if (Choice.boolValue) {

				var ChoiceOptionsRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

				var ChoiceOptions = property.FindPropertyRelative("choiceOptions");

				ActiveElements++;

				ChoiceOptions.isExpanded = EditorGUI.Foldout(ChoiceOptionsRect, ChoiceOptions.isExpanded, new GUIContent { text = ChoiceOptions.displayName, tooltip = "Options for the choice menu" });

				if (ChoiceOptions.isExpanded) {
					EditorGUI.indentLevel++;

					var ChoiceOptionsSizeRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
					ChoiceOptions.arraySize = EditorGUI.IntField(ChoiceOptionsSizeRect, new GUIContent { text = "Size" }, ChoiceOptions.arraySize);
					ActiveElements++;

					for (int i = 0; i < ChoiceOptions.arraySize; i++) {
						var ChoiceOptionsElementRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

						var ChoiceOptionsElement = ChoiceOptions.GetArrayElementAtIndex(i);

						EditorGUI.PropertyField(ChoiceOptionsElementRect, ChoiceOptionsElement);
						ActiveElements++;

						if (ChoiceOptionsElement.isExpanded) {
							position.y += CustomInspectorUtility.GetPropertyHeight(ChoiceOptionsElement, CustomInspectorUtility.GetVariables("Assets/Editor/InspectorVariables/Option_InspectorVariables.asset"));
						}
					}

					EditorGUI.indentLevel--;
				}
			}

			var ViewedRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

			var Viewed = property.FindPropertyRelative("Viewed");

			EditorGUI.PropertyField(ViewedRect, Viewed);
			ActiveElements++;

			var VoicedRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

			var Voiced = property.FindPropertyRelative("Voiced");

			EditorGUI.PropertyField(VoicedRect, Voiced);
			ActiveElements++;

			if (Voiced.boolValue) {
				var VoiceClipRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

				var VoiceClip = property.FindPropertyRelative("VoiceClip");

				EditorGUI.PropertyField(VoiceClipRect, VoiceClip);
				ActiveElements++;
			}

			var onSentenceInitRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

			var onSentenceInit = property.FindPropertyRelative("onSentenceInit");

			EditorGUI.PropertyField(onSentenceInitRect, onSentenceInit, true);

			position.y += GetOnSentenceInitropertyHeight(onSentenceInit);

			EditorGUI.indentLevel--;
		}

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		float propertyHeight = 18;

		if(property.isExpanded) {
			var text = property.FindPropertyRelative("Text");
			var OverrideName = property.FindPropertyRelative("OverrideName");
			var Voiced = property.FindPropertyRelative("Voiced");
			var Choice = property.FindPropertyRelative("Choice");

			int lines = text.stringValue.Split('\n').Length;
			lines = (lines > 3) ? (lines < 15) ? lines : 15 : 3;

			propertyHeight += 18 * 6 + 15 * lines + 26;

			if(OverrideName.boolValue) {
				propertyHeight += 18;
			}

			if(Choice.boolValue) {
				propertyHeight += 18;

				var ChoiceOptions = property.FindPropertyRelative("choiceOptions");

				if(ChoiceOptions.isExpanded) {
					propertyHeight += 18;

					for (int i = 0; i < ChoiceOptions.arraySize; i++) {
						var ChoiceOptionsElement = ChoiceOptions.GetArrayElementAtIndex(i);

						propertyHeight += 18;

						if (ChoiceOptionsElement.isExpanded) {
							propertyHeight += CustomInspectorUtility.GetPropertyHeight(ChoiceOptionsElement, CustomInspectorUtility.GetVariables("Assets/Editor/InspectorVariables/Option_InspectorVariables.asset"));
						}
					}
				}
			}

			propertyHeight += GetOnSentenceInitropertyHeight(property.FindPropertyRelative("onSentenceInit"));

			if (Voiced.boolValue) {
				propertyHeight += 18;
			}
		}

		return propertyHeight;
	}

	public float GetOnSentenceInitropertyHeight(SerializedProperty property) {
		float propertyHeight = 18;
		
		if(property.isExpanded) {

			propertyHeight += 18;

			for (int i = 0; i < property.arraySize; i++) {
				var action = property.GetArrayElementAtIndex(i);

				propertyHeight += 20 + CustomInspectorUtility.GetPropertyHeight(action, CustomInspectorUtility.GetVariables("Assets/Editor/InspectorVariables/OnSentenceInit_InspectorVariables.asset"));
			}
		}

		return propertyHeight;
	}
}

#endif