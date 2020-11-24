#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OnSentenceInit))]
public class SentenceActionsPropertyDrawer : PropertyDrawer {
	int ActiveElements = 0;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);

		Rect contentPosition = EditorGUI.PrefixLabel(position, label);
		ActiveElements = 1;

		var ActionTypeRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

		var ActionType = property.FindPropertyRelative("actionType");

		EditorGUI.indentLevel++;

		EditorGUI.PropertyField(ActionTypeRect, ActionType);
		ActiveElements++;

		switch (ActionType.intValue) {
			case 0: { //Add Character To Scene
					var CharacterNameRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

					var CharacterName = property.FindPropertyRelative("CharacterName");

					EditorGUI.PropertyField(CharacterNameRect, CharacterName);

					ActiveElements++;

					var CharacterStateRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

					var CharacterState = property.FindPropertyRelative("CharacterState");

					EditorGUI.PropertyField(CharacterStateRect, CharacterState);

					ActiveElements++;

					var FadeInRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
					var FadeIn = property.FindPropertyRelative("FadeIn");
					EditorGUI.PropertyField(FadeInRect, FadeIn);
					ActiveElements++;

					if (FadeIn.boolValue) {
						var FadeSpeedRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
						var FadeSpeed = property.FindPropertyRelative("FadeSpeed");
						EditorGUI.PropertyField(FadeSpeedRect, FadeSpeed);
						ActiveElements++;
					}

					var EnterSceneRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
					var EnterScene = property.FindPropertyRelative("EnterScene");
					EditorGUI.PropertyField(EnterSceneRect, EnterScene);
					ActiveElements++;

					if (EnterScene.boolValue) {
						var transitionSpeedRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
						var transitionSpeed = property.FindPropertyRelative("transitionSpeed");
						EditorGUI.PropertyField(transitionSpeedRect, transitionSpeed);
						ActiveElements++;

						var startingPositionRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
						var startingPosition = property.FindPropertyRelative("startingPosition");
						EditorGUI.PropertyField(startingPositionRect, startingPosition);
						ActiveElements++;

						if (startingPosition.intValue == 2) {
							var customStartingPositionRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
							var customStartingPosition = property.FindPropertyRelative("customStartingPosition");
							EditorGUI.PropertyField(customStartingPositionRect, customStartingPosition);
							ActiveElements++;
						}
					}

					var PositionRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
					var Position = property.FindPropertyRelative("Position");
					EditorGUI.PropertyField(PositionRect, Position);
					ActiveElements++;

					break;
				}
			case 1: { //Move Character
					var CharacterNameRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

					var CharacterName = property.FindPropertyRelative("CharacterName");

					EditorGUI.PropertyField(CharacterNameRect, CharacterName);

					ActiveElements++;

					var TransitionRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
					var Transition = property.FindPropertyRelative("Transition");
					EditorGUI.PropertyField(TransitionRect, Transition);
					ActiveElements++;

					if (Transition.boolValue) {
						var transitionSpeedRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
						var transitionSpeed = property.FindPropertyRelative("transitionSpeed");
						EditorGUI.PropertyField(transitionSpeedRect, transitionSpeed);
						ActiveElements++;
					}

					var PositionRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
					var Position = property.FindPropertyRelative("Position");
					EditorGUI.PropertyField(PositionRect, Position);
					ActiveElements++;

					break;
				}
			case 2: { //Remove Character From Scene
					var CharacterNameRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

					var CharacterName = property.FindPropertyRelative("CharacterName");

					EditorGUI.PropertyField(CharacterNameRect, CharacterName);

					ActiveElements++;

					var ExitSceneRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
					var ExitScene = property.FindPropertyRelative("ExitScene");
					EditorGUI.PropertyField(ExitSceneRect, ExitScene);
					ActiveElements++;

					if (ExitScene.boolValue) {
						var transitionSpeedRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
						var transitionSpeed = property.FindPropertyRelative("transitionSpeed");
						EditorGUI.PropertyField(transitionSpeedRect, transitionSpeed);
						ActiveElements++;
					}

					var FadeOutRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

					var FadeOut = property.FindPropertyRelative("FadeOut");

					EditorGUI.PropertyField(FadeOutRect, FadeOut);

					ActiveElements++;

					if (FadeOut.boolValue) {
						var FadeSpeedRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
						var FadeSpeed = property.FindPropertyRelative("FadeSpeed");
						EditorGUI.PropertyField(FadeSpeedRect, FadeSpeed);
						ActiveElements++;
					}

					var PositionRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
					var Position = property.FindPropertyRelative("Position");
					EditorGUI.PropertyField(PositionRect, Position);
					ActiveElements++;
					break;
				}
			case 3: { //Change Character State
					var CharacterNameRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

					var CharacterName = property.FindPropertyRelative("CharacterName");

					EditorGUI.PropertyField(CharacterNameRect, CharacterName);

					ActiveElements++;

					var CharacterStateRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

					var CharacterState = property.FindPropertyRelative("CharacterState");

					EditorGUI.PropertyField(CharacterStateRect, CharacterState);

					ActiveElements++;

					var TransitionRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
					var Transition = property.FindPropertyRelative("Transition");
					EditorGUI.PropertyField(TransitionRect, Transition);
					ActiveElements++;

					if (Transition.boolValue) {
						var transitionSpeedRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
						var transitionSpeed = property.FindPropertyRelative("transitionSpeed");
						EditorGUI.PropertyField(transitionSpeedRect, transitionSpeed);
						ActiveElements++;
					}
					break;
				}
			case 4: { //Delay
					var DelayRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
					var Delay = property.FindPropertyRelative("Delay");
					EditorGUI.PropertyField(DelayRect, Delay);
					ActiveElements++;
					break;
				}
		}

		EditorGUI.indentLevel--;

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		float propertyHeight = 36;

		var ActionType = property.FindPropertyRelative("actionType");
		var Transition = property.FindPropertyRelative("Transition");
		var FadeIn = property.FindPropertyRelative("FadeIn");
		var FadeOut = property.FindPropertyRelative("FadeOut");
		var EnterScene = property.FindPropertyRelative("EnterScene");
		var ExitScene = property.FindPropertyRelative("ExitScene");
		var startingPosition = property.FindPropertyRelative("startingPosition");

		if (ActionType.intValue == 0) { //Add Character To Scene
			propertyHeight = EditorGUIUtility.singleLineHeight * 6 + 12;

			if (FadeIn.boolValue) {
				propertyHeight += EditorGUIUtility.singleLineHeight + 2;
			}

			if (EnterScene.boolValue) {
				propertyHeight += EditorGUIUtility.singleLineHeight * 2 + 4;

				if (startingPosition.intValue == 2) {
					propertyHeight += EditorGUIUtility.singleLineHeight + 2;
				}
			}
		} else if (ActionType.intValue == 1) {//Move Character
			propertyHeight = EditorGUIUtility.singleLineHeight * 4 + 16;

			if (Transition.boolValue) {
				propertyHeight += EditorGUIUtility.singleLineHeight + 2;
			}
		} else if (ActionType.intValue == 2) { //Remove Character From Scene
			propertyHeight = EditorGUIUtility.singleLineHeight * 5 + 17;

			if (FadeOut.boolValue) {
				propertyHeight += EditorGUIUtility.singleLineHeight + 2;
			}

			if (ExitScene.boolValue) {
				propertyHeight += EditorGUIUtility.singleLineHeight + 2;
			}
		} else if (ActionType.intValue == 3) { //Change Character State
			propertyHeight = EditorGUIUtility.singleLineHeight * 4 + 16;

			if (Transition.boolValue) {
				propertyHeight += EditorGUIUtility.singleLineHeight + 2;
			}
		} else if (ActionType.intValue == 4) { //Delay
			propertyHeight = EditorGUIUtility.singleLineHeight * 3;
		}

		return propertyHeight;
	}
}
#endif