/*#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OnSentenceInit))]
public class SentenceActionsPropertyDrawer : PropertyDrawer {
	int ActiveElements = 0;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

		var ActionType = property.FindPropertyRelative("actionType");

		label.text = "Action(" + ActionType.enumDisplayNames[ActionType.enumValueIndex] + ")";

		EditorGUI.BeginProperty(position, label, property);

		Rect labelPosition = new Rect { x = position.x, y = position.y, width = position.width, height = 16 };

		property.isExpanded = EditorGUI.Foldout(labelPosition, property.isExpanded, label);

		//Rect contentPosition = EditorGUI.PrefixLabel(position, label);

		if (property.isExpanded) {
			ActiveElements = 1;

			var ActionTypeRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

			EditorGUI.indentLevel++;

			EditorGUI.PropertyField(ActionTypeRect, ActionType);
			ActiveElements++;

			switch (ActionType.intValue) {
				case (int)OnSentenceInit.Actions.AddCharacterToScene: { //Add Character To Scene
						var CharacterNameRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

						var CharacterName = property.FindPropertyRelative("CharacterName");

						EditorGUI.PropertyField(CharacterNameRect, CharacterName);

						ActiveElements++;

						var CharacterVariantRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

						var CharacterVariant = property.FindPropertyRelative("CharacterVariant");

						EditorGUI.PropertyField(CharacterVariantRect, CharacterVariant);

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
				case (int)OnSentenceInit.Actions.MoveCharacter: { //Move Character
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
				case (int)OnSentenceInit.Actions.RemoveCharacterFromScene: { //Remove Character From Scene
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
				case (int)OnSentenceInit.Actions.ChangeCharacterState: { //Change Character State
						var CharacterNameRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

						var CharacterName = property.FindPropertyRelative("CharacterName");

						EditorGUI.PropertyField(CharacterNameRect, CharacterName);

						ActiveElements++;

						var CharacterVariantRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);

						var CharacterVariant = property.FindPropertyRelative("CharacterVariant");

						EditorGUI.PropertyField(CharacterVariantRect, CharacterVariant);

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
				case (int)OnSentenceInit.Actions.Delay: { //Delay
						var DelayRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
						var Delay = property.FindPropertyRelative("Delay");
						EditorGUI.PropertyField(DelayRect, Delay);
						ActiveElements++;
						break;
					}
				case (int)OnSentenceInit.Actions.ShowBG: { //Show BG
						var BG_IDRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
						var BG_ID = property.FindPropertyRelative("BG");
						EditorGUI.PropertyField(BG_IDRect, BG_ID);
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
						break;
					}
				case (int)OnSentenceInit.Actions.ShowCG: { //Show CG
						var CG_IDRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
						var CG_ID = property.FindPropertyRelative("CG");
						EditorGUI.PropertyField(CG_IDRect, CG_ID);
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
						break;
					}
				case (int)OnSentenceInit.Actions.WaitUntilActionIsFinished: { //Wait Until Action Is finished
						var RunOnlyOnceRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
						var RunOnlyOnce = property.FindPropertyRelative("RunOnlyOnce");
						EditorGUI.PropertyField(RunOnlyOnceRect, RunOnlyOnce);
						ActiveElements++;
						break;
					}
				case (int)OnSentenceInit.Actions.SetCharacterIndex: { //Set Character Index
						var CharacterNameRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
						var CharacterName = property.FindPropertyRelative("CharacterName");
						EditorGUI.PropertyField(CharacterNameRect, CharacterName);
						ActiveElements++;

						var CharacterIndexeRect = new Rect(position.x, position.y + 18 * ActiveElements, position.width, 16);
						var CharacterIndex = property.FindPropertyRelative("CharacterIndex");
						EditorGUI.PropertyField(CharacterIndexeRect, CharacterIndex);
						ActiveElements++;
						break;
					}
			}

			EditorGUI.indentLevel--;
		}

		//EditorGUI.EndFoldoutHeaderGroup();

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		float propertyHeight = EditorGUIUtility.singleLineHeight;

		if (property.isExpanded) {
			propertyHeight = 36;

			var ActionType = property.FindPropertyRelative("actionType");
			var Transition = property.FindPropertyRelative("Transition");
			var FadeIn = property.FindPropertyRelative("FadeIn");
			var FadeOut = property.FindPropertyRelative("FadeOut");
			var EnterScene = property.FindPropertyRelative("EnterScene");
			var ExitScene = property.FindPropertyRelative("ExitScene");
			var startingPosition = property.FindPropertyRelative("startingPosition");

			switch (ActionType.intValue) {
				case (int)OnSentenceInit.Actions.AddCharacterToScene: { //Add Character To Scene
						propertyHeight = EditorGUIUtility.singleLineHeight * 7 + 14;

						if (FadeIn.boolValue) {
							propertyHeight += EditorGUIUtility.singleLineHeight + 2;
						}

						if (EnterScene.boolValue) {
							propertyHeight += EditorGUIUtility.singleLineHeight * 2 + 4;

							if (startingPosition.intValue == 2) {
								propertyHeight += EditorGUIUtility.singleLineHeight + 2;
							}
						}
						break;
					}
				case (int)OnSentenceInit.Actions.MoveCharacter: { //Move Character
						propertyHeight = EditorGUIUtility.singleLineHeight * 4 + 16;

						if (Transition.boolValue) {
							propertyHeight += EditorGUIUtility.singleLineHeight + 2;
						}
						break;
					}
				case (int)OnSentenceInit.Actions.RemoveCharacterFromScene: { //Remove Character From Scene
						propertyHeight = EditorGUIUtility.singleLineHeight * 5 + 17;

						if (FadeOut.boolValue) {
							propertyHeight += EditorGUIUtility.singleLineHeight + 2;
						}

						if (ExitScene.boolValue) {
							propertyHeight += EditorGUIUtility.singleLineHeight + 2;
						}
						break;
					}
				case (int)OnSentenceInit.Actions.ChangeCharacterState: { //Change Character State
						propertyHeight = EditorGUIUtility.singleLineHeight * 5 + 18;

						if (Transition.boolValue) {
							propertyHeight += EditorGUIUtility.singleLineHeight + 2;
						}
						break;
					}
				case (int)OnSentenceInit.Actions.Delay: { //Delay
						propertyHeight = EditorGUIUtility.singleLineHeight * 3;
						break;
					}
				case (int)OnSentenceInit.Actions.ShowBG: { //Show BG
						propertyHeight = EditorGUIUtility.singleLineHeight * 4;

						if(FadeIn.boolValue) {
							propertyHeight += EditorGUIUtility.singleLineHeight + 2;
						}
						break;
					}
				case (int)OnSentenceInit.Actions.ShowCG: { //Show CG
						propertyHeight = EditorGUIUtility.singleLineHeight * 4;

						if (FadeIn.boolValue) {
							propertyHeight += EditorGUIUtility.singleLineHeight + 2;
						}
						break;
					}
				case (int)OnSentenceInit.Actions.WaitUntilActionIsFinished: { //Wait Until Action Is Finished
						propertyHeight = EditorGUIUtility.singleLineHeight * 3;
						break;
					}
				case (int)OnSentenceInit.Actions.SetCharacterIndex: { //Set Character Index
						propertyHeight = EditorGUIUtility.singleLineHeight * 4 + 8;
						break;
					}
			}
		}

		return propertyHeight;
	}
}
#endif*/