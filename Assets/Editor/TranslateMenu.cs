using System.Collections.Generic;
using RestSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class TranslateMenu : EditorWindow {

	public int MenuNumber = 0;
	public int ItemID = 0;
	public int SubItemID = 0;

	public static bool overrideSettings;

	public TranslateOptions options;

	public AutoTranslateAPIs autoTranslateAPIs;

	public static string AutoTranslateAPIKey = "";

	public string[] MTLLanguages = new string[9] { "auto", "de", "en", "fr", "es", "it", "nl", "pl", "ja" };

	public static object reference;

	public static object output;

	private static Vector2 scrollPos;

	public static bool MTLOptions;
	public static int MTLSourceLanguage;
	public static int MTLOutputLanguage;

	public List<bool> chapterFoldout = new List<bool>();
	public List<List<bool>> sentenceFoldout = new List<List<bool>>();
	public List<List<bool>> choiceFoldout = new List<List<bool>>();

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

					options = (TranslateOptions)EditorGUILayout.EnumPopup("Source file", options);

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

						chapterFoldout.Clear();
						sentenceFoldout.Clear();
						choiceFoldout.Clear();

						SentenceManager manager = (SentenceManager)reference;

						foreach (Chapter chapter in manager.Chapters) {
							chapterFoldout.Add(true);

							List<bool> sentences = new List<bool>();

							List<bool> choices = new List<bool>();

							foreach (Sentence sentence in chapter.Sentences) {
								sentences.Add(true);

								if (sentence.Choice) {
									choices.Add(true);
								}
							}

							sentenceFoldout.Add(sentences);

							choiceFoldout.Add(choices);
						}

						MTLSourceLanguage = 0;
						MTLOutputLanguage = 1;
					}

					break;
				}
			case 1: {
					if (reference == null || reference.GetType() != typeof(SentenceManager)) { MenuNumber = 0; return; }

					overrideSettings = EditorGUILayout.Toggle("Override values", overrideSettings);

					SentenceManager manager = (SentenceManager)reference;

					if(manager == null) { MenuNumber = 0; reference = null; return; }

					if (output == null) {
						AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(manager), "Assets/Sentences/Translation.asset");
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();

						output = AssetDatabase.LoadAssetAtPath<SentenceManager>("Assets/Sentences/Translation.asset");

						overrideSettings = false;
						ItemID = 0;
						SubItemID = 0;

						scrollPos = Vector2.zero;
					}

					MTLOptions = EditorGUILayout.BeginFoldoutHeaderGroup(MTLOptions, "Auto translate options");

					if(MTLOptions) {
						autoTranslateAPIs = (AutoTranslateAPIs)EditorGUILayout.EnumPopup("API Used", autoTranslateAPIs);

						AutoTranslateAPIKey = EditorGUILayout.TextField("API Key", AutoTranslateAPIKey);

						MTLSourceLanguage = EditorGUILayout.Popup("Source Language", MTLSourceLanguage, new string[9] { "Auto select", "German", "English", "French", "Spanish", "Italian", "Dutch", "Polish", "Japanese" });

						MTLOutputLanguage = EditorGUILayout.Popup("Target Language", MTLOutputLanguage - 1, new string[8] { "German", "English", "French", "Spanish", "Italian", "Dutch", "Polish", "Japanese" }) + 1;
					}

					EditorGUILayout.EndFoldoutHeaderGroup();

					SentenceManager sentenceManager = (SentenceManager)output;

					sentenceManager.Translating = true;

					scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

					EditorGUI.indentLevel++;

					chapterFoldout[ItemID] = EditorGUILayout.BeginFoldoutHeaderGroup(chapterFoldout[ItemID], "Chapter translation <" + (ItemID + 1)  + "/" + manager.Chapters.Length + ">");
					if(chapterFoldout[ItemID]) {
						EditorGUILayout.SelectableLabel(manager.Chapters[ItemID].ChapterName);
						sentenceManager.Chapters[ItemID].ChapterName = EditorGUILayout.TextArea(sentenceManager.Chapters[ItemID].ChapterName);

						if (GUILayout.Button("Auto translate chapter")) {
							sentenceManager.Chapters[ItemID].ChapterName = TranslateText(manager.Chapters[ItemID].ChapterName);
						}
					}

					EditorGUILayout.EndFoldoutHeaderGroup();

					EditorGUILayout.Space(15);

					sentenceFoldout[ItemID][SubItemID] = EditorGUILayout.BeginFoldoutHeaderGroup(sentenceFoldout[ItemID][SubItemID], "Sentence translation <" + (SubItemID + 1) + "/" + manager.Chapters[ItemID].Sentences.Count + ">");

					if (sentenceFoldout[ItemID][SubItemID]) {
						EditorGUILayout.SelectableLabel(manager.Chapters[ItemID].Sentences[SubItemID].Text);
						sentenceManager.Chapters[ItemID].Sentences[SubItemID].Text = EditorGUILayout.TextArea(sentenceManager.Chapters[ItemID].Sentences[SubItemID].Text);

						if (GUILayout.Button("Auto translate sentence")) {
							sentenceManager.Chapters[ItemID].Sentences[SubItemID].Text = TranslateText(manager.Chapters[ItemID].Sentences[SubItemID].Text);
						}
					}

					EditorGUILayout.EndFoldoutHeaderGroup();

					EditorGUILayout.Space(5);

					if (sentenceManager.Chapters[ItemID].Sentences[SubItemID].Choice) {
						EditorGUI.indentLevel++;

						choiceFoldout[ItemID][SubItemID] = EditorGUILayout.BeginFoldoutHeaderGroup(choiceFoldout[ItemID][SubItemID], "Choice translation");

						if(choiceFoldout[ItemID][SubItemID]) {
							for (int i = 0; i < sentenceManager.Chapters[ItemID].Sentences[SubItemID].choiceOptions.Length; i++) {
								EditorGUILayout.SelectableLabel(manager.Chapters[ItemID].Sentences[SubItemID].choiceOptions[i].OptionText);
								sentenceManager.Chapters[ItemID].Sentences[SubItemID].choiceOptions[i].OptionText = EditorGUILayout.TextArea(sentenceManager.Chapters[ItemID].Sentences[SubItemID].choiceOptions[i].OptionText);
							}

							if (GUILayout.Button("Auto translate choices")) {
								sentenceManager.Chapters[ItemID].ChapterName = TranslateText(manager.Chapters[ItemID].ChapterName);

								string choicesText = "";

								for (int i = 0; i < sentenceManager.Chapters[ItemID].Sentences[SubItemID].choiceOptions.Length; i++) {
									choicesText += manager.Chapters[ItemID].Sentences[SubItemID].choiceOptions[i].OptionText + "¤";
								}

								choicesText = TranslateText(choicesText);

								for (int i = 0; i < sentenceManager.Chapters[ItemID].Sentences[SubItemID].choiceOptions.Length; i++) {
									sentenceManager.Chapters[ItemID].Sentences[SubItemID].choiceOptions[i].OptionText = choicesText.Split('¤')[i].Trim('¤');
								}
							}
						}

						EditorGUILayout.EndFoldoutHeaderGroup();

						EditorGUI.indentLevel--;

						EditorGUILayout.Space(5);
					}

					EditorGUI.indentLevel--;

					if (overrideSettings) {
						
					}

					EditorGUILayout.Space(12);

					EditorGUILayout.EndScrollView();

					EditorGUILayout.Space(5);

					if (GUILayout.Button("Auto translate everything")) {
						string chapterTranslationData = "";

						List<string> sentencesTranslationData = new List<string>();

						List<List<string>> choicesTranslationData = new List<List<string>>();

						for(int i = 0; i < manager.Chapters.Length; i++) {
							Chapter chapter = manager.Chapters[i];

							chapterTranslationData += chapter.ChapterName + "¤";

							string sentenceTranslationData = "";

							List<string> choiceTranslationData = new List<string>();

							for (int o = 0; o < chapter.Sentences.Count; o++) {
								Sentence sentence = chapter.Sentences[o];

								sentenceTranslationData += sentence.Text + "¤";

								if(sentence.Choice) {
									string choiceOptionsTranslationData = "";

									foreach (Option choice in sentence.choiceOptions) {
										choiceOptionsTranslationData += choice.OptionText + "¤";
									}

									choiceTranslationData.Add(choiceOptionsTranslationData);
								}
							}

							choicesTranslationData.Add(choiceTranslationData);

							sentencesTranslationData.Add(sentenceTranslationData);
						}

						if(autoTranslateAPIs == AutoTranslateAPIs.Manual) {

						} else {
							if (manager.Chapters.Length > 0) {
								chapterTranslationData = TranslateText(chapterTranslationData);

								for (int i = 0; i < sentenceManager.Chapters.Length; i++) {
									Chapter chapter = sentenceManager.Chapters[i];

									chapter.ChapterName = chapterTranslationData.Split('¤')[i].Trim('¤');

									if (chapter.Sentences.Count > 0) {
										string sentenceDataTranslation = TranslateText(sentencesTranslationData[i]);

										for (int o = 0; o < chapter.Sentences.Count; o++) {
											Sentence sentence = chapter.Sentences[o];

											sentence.Text = sentenceDataTranslation.Split('¤')[o].Trim('¤');

											if (sentence.Choice && sentence.choiceOptions.Length > 0) {
												string choicesText = TranslateText(choicesTranslationData[i][o]);

												for (int p = 0; p < sentence.choiceOptions.Length; p++) {
													Option choice = sentence.choiceOptions[p];

													choice.OptionText = choicesText.Split('¤')[p].Trim('¤');
												}
											}
										}
									}
								}
							}
						}
					}

					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Last chapter") && ItemID > 0) { ItemID--; SubItemID = 0; scrollPos = Vector2.zero; }
					if (GUILayout.Button("Next chapter") && ItemID < sentenceManager.Chapters.Length - 1) { ItemID++; SubItemID = 0; scrollPos = Vector2.zero; }
					EditorGUILayout.EndHorizontal();	

					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Last sentence") && SubItemID > 0) { SubItemID--; scrollPos = Vector2.zero; }
					if (GUILayout.Button("Next sentence") && SubItemID < sentenceManager.Chapters[ItemID].Sentences.Count) { SubItemID++; scrollPos = Vector2.zero; }
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Done  ")) { MenuNumber = 0; output = null; sentenceManager.Translating = false; }
					if (GUILayout.Button("Cancel")) { MenuNumber = 0; output = null; if(sentenceManager != null) AssetDatabase.DeleteAsset("Assets/Sentences/Translation.asset"); }
					EditorGUILayout.EndHorizontal();

					break;
				}
		}
	}

	public string TranslateText(string text) {
		if(AutoTranslateAPIKey == "") { Debug.LogError("Enter the API key to use with your translator choice. If you don't have one, create it at: https://english.api.rakuten.net"); return text; }

		switch(autoTranslateAPIs) {
			case AutoTranslateAPIs.GoogleTranslate: {
					var client = new RestClient("https://google-translate1.p.rapidapi.com/language/translate/v2");
					var request = new RestRequest(Method.POST);
					request.AddHeader("content-type", "application/x-www-form-urlencoded");
					request.AddHeader("accept-encoding", "application/gzip");
					request.AddHeader("x-rapidapi-key", AutoTranslateAPIKey);
					request.AddHeader("x-rapidapi-host", "google-translate1.p.rapidapi.com");
					request.AddParameter("application/x-www-form-urlencoded", "q=" + UnityWebRequest.EscapeURL(text).Replace("+", "%20") + "&target=" + MTLLanguages[MTLOutputLanguage] + (MTLSourceLanguage != 0 ? "&source=" + MTLLanguages[MTLSourceLanguage] + "" : ""), ParameterType.RequestBody);
					IRestResponse response = client.Execute(request);

					GoogleTranslateAnswer answer = JsonUtility.FromJson<GoogleTranslateAnswer>(response.Content);

					if (!response.IsSuccessful) { Debug.LogError(answer.message); return text; }

					return answer.data.translations[0].translatedText;
				}
			case AutoTranslateAPIs.DeepTranslate: {
					DeepTranslateRequest translateRequest = new DeepTranslateRequest {
						q = text,
						source = MTLLanguages[MTLSourceLanguage],
						target = MTLLanguages[MTLOutputLanguage]
					};

					var client = new RestClient("https://deep-translate1.p.rapidapi.com/language/translate/v2");
					var request = new RestRequest(Method.POST);
					request.AddHeader("content-type", "application/json");
					request.AddHeader("x-rapidapi-key", AutoTranslateAPIKey);
					request.AddHeader("x-rapidapi-host", "deep-translate1.p.rapidapi.com");
					request.AddParameter("application/json", JsonUtility.ToJson(translateRequest, true), ParameterType.RequestBody);
					IRestResponse response = client.Execute(request);

					Debug.Log(response.Content);

					DeepTranslateAnswer answer = JsonUtility.FromJson<DeepTranslateAnswer>(response.Content);

					if (!response.IsSuccessful) { Debug.LogError(answer.message); return text; }

					return UnityWebRequest.UnEscapeURL(answer.data.translations.translatedText);
				}
			default: {
					Debug.LogError("API not supported");
					return text;
				}
		}
	}

	public enum TranslateOptions { SentenceData, CharacterData, LanguagePack };
	public enum AutoTranslateAPIs { GoogleTranslate, DeepTranslate, Manual };
}

#region GoogleTranslate
[System.Serializable]
public class GoogleTranslateAnswer {
	public string message;
	public GoogleTranslateAnswerData data;
}

[System.Serializable]
public class GoogleTranslateAnswerData {
	public GoogleTranslateAnswerDataTranslation[] translations;
}

[System.Serializable]
public class GoogleTranslateAnswerDataTranslation {
	public string translatedText;
}
#endregion

#region DeepTranslate
[System.Serializable]
public class DeepTranslateAnswer {
	public string message;
	public DeepTranslateAnswerData data;
}

[System.Serializable]
public class DeepTranslateAnswerData {
	public DeepTranslateAnswerrDataTranslation translations;
}

[System.Serializable]
public class DeepTranslateAnswerrDataTranslation {
	public string translatedText;
}

[System.Serializable]
public class DeepTranslateRequest {
	public string q;
	public string source = "auto";
	public string target = "en";
}
#endregion
