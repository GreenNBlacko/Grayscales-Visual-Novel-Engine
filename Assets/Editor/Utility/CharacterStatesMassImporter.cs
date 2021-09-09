#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterStatesMassImporter : EditorWindow {

	public CharacterInfo characterInfo;

	public int selectedIndex;

	public bool newVariant;

	public string variantName = "";

	public List<Sprite> States;

	public Vector2 scrollPos;

	[MenuItem("Utility/Character States Mass Importer")]
	public static void ShowWindow() {
		GetWindow(typeof(CharacterStatesMassImporter), false, "Character States Mass Importer");
	}

	public void OnGUI() {
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		if(characterInfo == null) {
			characterInfo = FindObjectOfType<CharacterInfo>();
		}

		List<string> characters = new List<string>();

		foreach(Character character in characterInfo.Characters) {
			characters.Add(character.CharacterName);
		}

		selectedIndex = EditorGUILayout.Popup("Character", selectedIndex, characters.ToArray());

		if (characterInfo.Characters[selectedIndex].characterVariants.Length > 0) newVariant = EditorGUILayout.Toggle("Create Character Variant", newVariant);
		else newVariant = true;

		if(newVariant) {
			variantName = EditorGUILayout.TextField("Variant Name", variantName);
		} else {
			List<string> CharacterVariants = new List<string>();

			int index = 0;

			foreach(CharacterVariant variant in characterInfo.Characters[selectedIndex].characterVariants) {
				CharacterVariants.Add(variant.VariantName);

				if(variant.VariantName == variantName) {
					index = CharacterVariants.IndexOf(variant.VariantName);
				}
			}

			variantName = CharacterVariants[EditorGUILayout.Popup("Character Variant", index, CharacterVariants.ToArray())];
		}

		ScriptableObject target = this;
		SerializedObject so = new SerializedObject(target);
		SerializedProperty stringsProperty = so.FindProperty("States");

		EditorGUILayout.PropertyField(stringsProperty, true);
		so.ApplyModifiedProperties();
		if(variantName != "") {
			if (GUILayout.Button("Import States")) {
				Character character = characterInfo.Characters[selectedIndex];

				if(newVariant) {
					List<CharacterVariant> characterVariants = new List<CharacterVariant>(character.characterVariants);

					List<CharacterState> states = new List<CharacterState>();

					foreach (Sprite sprite in States) {
						states.Add(new CharacterState(sprite.texture.name, CharacterState.StateType.SingleLayer, sprite));
					}

					characterVariants.Add(new CharacterVariant { VariantName = variantName, variantStates = states.ToArray() });

					character.characterVariants = characterVariants.ToArray();

					Debug.Log("Finished importing " + states.Count + " states in '" + variantName + "' variant!");

					States.Clear();
				} else {
					foreach(CharacterVariant variant in character.characterVariants) {
						if(variant.VariantName == variantName) {
							List<CharacterState> states = new List<CharacterState>(variant.variantStates);

							foreach (Sprite sprite in States) {
								states.Add(new CharacterState(sprite.texture.name, CharacterState.StateType.SingleLayer, sprite));
							}

							variant.variantStates = states.ToArray();

							Debug.Log("Finished importing " + states.Count + " states in '" + variantName + "' variant!");

							States.Clear();
							break;
						}
					}
				}
			}
		} else {
			EditorGUILayout.HelpBox("Create or select pre-existing character variant before proceeding!", MessageType.Error);
		}

		EditorGUILayout.EndScrollView();
	}
}

#endif