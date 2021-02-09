using UnityEditor;
using UnityEngine;

public class TranslateMenu : EditorWindow {

	private int MenuNumber = 0;
	public int ItemID = 0;
	public int SubItemID = 0;

	public bool overrideSettings;

	public TranslateOptions options;

	public object reference;

	public object output;

	private Vector2 scrollPos; 


	[MenuItem("Utility/Translate sentence data")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(TranslateMenu), false, "Translate Sentence data");
	}

	public void OnGUI() {
		ShowMenu(MenuNumber);
	}

	public void ShowMenu(int ID) {
		switch (ID) {
			case 0: {
					int temp = 0;

					options = (TranslateOptions)EditorGUILayout.EnumPopup("Select which file to edit", options);

					switch (options) {
						case TranslateOptions.SentenceData: {
								if(reference != null && reference.GetType() != typeof(SentenceManager)) { reference = null; }
								reference = (SentenceManager)EditorGUILayout.ObjectField((Object)reference, typeof(SentenceManager), true);
								temp = 1;
								break;
							}
						case TranslateOptions.CharacterData: {
								if (reference != null && reference.GetType() != typeof(CharacterInfo)) { reference = null; }
								reference = (CharacterInfo)EditorGUILayout.ObjectField((Object)reference, typeof(CharacterInfo), true);
								temp = 2;
								break;
							}
						case TranslateOptions.LanguagePack: {
								return;
							}
					}

					if (GUILayout.Button("Translate")) {
						if (reference != null) MenuNumber = temp;
					}

					break;
				}
			case 1: {
					if (reference == null && reference.GetType() != typeof(SentenceManager)) { MenuNumber = 0; return; }

					overrideSettings = EditorGUILayout.Toggle("Override values", overrideSettings);

					SentenceManager manager = (SentenceManager)reference;

					if(manager == null) { MenuNumber = 0; reference = null; return; }

					if (output == null) {
						GameObject gameObject = Instantiate(manager.gameObject);
						gameObject.name = "Sentence Manager(Translation)";
						output = gameObject.GetComponent<SentenceManager>();

						overrideSettings = false;
						ItemID = 0;
						SubItemID = 0;

						scrollPos = Vector2.zero;
					}

					SentenceManager sentenceManager = (SentenceManager)output;

					sentenceManager.Translating = true;

					scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

					EditorGUI.indentLevel++;

					GUILayout.Label("Chapter translation <" + (ItemID + 1)  + "/" + manager.Chapters.Length + ">", EditorStyles.boldLabel);
					EditorGUILayout.SelectableLabel(manager.Chapters[ItemID].ChapterName);
					sentenceManager.Chapters[ItemID].ChapterName = EditorGUILayout.TextArea(sentenceManager.Chapters[ItemID].ChapterName);

					EditorGUILayout.Space(15);

					GUILayout.Label("Sentence translation <" + (SubItemID + 1) + "/" + manager.Chapters[ItemID].Sentences.Length + ">", EditorStyles.boldLabel);
					EditorGUILayout.SelectableLabel(manager.Chapters[ItemID].Sentences[SubItemID].Text);
					sentenceManager.Chapters[ItemID].Sentences[SubItemID].Text = EditorGUILayout.TextArea(sentenceManager.Chapters[ItemID].Sentences[SubItemID].Text);

					EditorGUILayout.Space(5);

					if (sentenceManager.Chapters[ItemID].Sentences[SubItemID].Choice) {
						EditorGUI.indentLevel++;

						GUILayout.Label("Choice translation", EditorStyles.boldLabel);

						for (int i = 0; i < sentenceManager.Chapters[ItemID].Sentences[SubItemID].choiceOptions.Length; i++) {
							EditorGUILayout.SelectableLabel(manager.Chapters[ItemID].Sentences[SubItemID].choiceOptions[i].OptionText);
							sentenceManager.Chapters[ItemID].Sentences[SubItemID].choiceOptions[i].OptionText = EditorGUILayout.TextArea(sentenceManager.Chapters[ItemID].Sentences[SubItemID].choiceOptions[i].OptionText);
						}

						EditorGUI.indentLevel--;

						EditorGUILayout.Space(5);
					}

					EditorGUI.indentLevel--;

					if (overrideSettings) {
						
					}

					EditorGUILayout.Space(12);

					EditorGUILayout.EndScrollView();

					EditorGUILayout.Space(5);

					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Last chapter") && ItemID > 0) { ItemID--; SubItemID = 0; scrollPos = Vector2.zero; }
					if (GUILayout.Button("Next chapter") && ItemID < sentenceManager.Chapters.Length - 1) { ItemID++; SubItemID = 0; scrollPos = Vector2.zero; }
					EditorGUILayout.EndHorizontal();	

					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Last sentence") && SubItemID > 0) { SubItemID--; scrollPos = Vector2.zero; }
					if (GUILayout.Button("Next sentence") && SubItemID < sentenceManager.Chapters[ItemID].Sentences.Length) { SubItemID++; scrollPos = Vector2.zero; }
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Done  ")) { MenuNumber = 0; output = null; sentenceManager.Translating = false; }
					if (GUILayout.Button("Cancel")) { MenuNumber = 0; output = null; if(sentenceManager != null) DestroyImmediate(sentenceManager.gameObject); }
					EditorGUILayout.EndHorizontal();

					break;
				}
		}
	}

	public enum TranslateOptions { SentenceData, CharacterData, LanguagePack };
}
