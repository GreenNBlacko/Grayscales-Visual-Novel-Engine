﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

	public GameObjects gameObjects;

	public ButtonArrays buttonArrays;

	public ButtonPrefabs buttonPrefabs;

	public AudioSources audioSources;

	public Scripts scripts;

	public TextBox textBox;

	[HideInInspector]
	public int BacklogID = 0;

	private bool ShowTextRun = false;

	private bool ignoreSentenceActions;

	[HideInInspector]
	public bool choiceNext = false;

	[HideInInspector]
	public bool skipmode = false;

	[HideInInspector]
	public bool disableInput = false;

	private bool automode = false;

	private Character currentCharacter = new Character();

	[HideInInspector]
	public bool automodeRunning = false;
	[Range(0, 6)]
	[Tooltip("How many seconds should automode wait before showing next line?")]
	[Header("Automode")]
	public float AutomodeDelay = 1.5F;

	void Start() {
		gameObjects.BacklogGameobject.SetActive(false);
		gameObjects.ChoiceList.SetActive(false);
		choiceNext = false;

		CalculateCharacterData();
	}

	void Update() {
		if (!disableInput) {
			if (Input.GetKeyDown("space") && !gameObjects.BacklogGameobject.activeInHierarchy && !gameObjects.ChoiceList.activeInHierarchy || Input.mouseScrollDelta.y < 0 && !gameObjects.BacklogGameobject.activeInHierarchy && !gameObjects.ChoiceList.activeInHierarchy) {
				NextSentence();
				skipmode = false;
				automode = false;
			}
			if (Input.mouseScrollDelta.y > 0 || Input.GetKeyDown(KeyCode.PageUp)) {
				if (!gameObjects.BacklogGameobject.activeInHierarchy)
					gameObjects.BacklogGameobject.SetActive(true);
			}

			if (Input.GetMouseButtonDown(0)) {
				skipmode = false;
				automode = false;
			}

			if (Input.GetMouseButtonDown(1)) {
				if (!gameObjects.BacklogGameobject.activeInHierarchy) {
					if (gameObjects.DisplayGameobject.activeInHierarchy)
						gameObjects.DisplayGameobject.SetActive(false);
					else
						gameObjects.DisplayGameobject.SetActive(true);
				}
			}

			if (Input.GetButtonDown("Skip")) {
				skipmode = true;
				textBox.TypeText = false;
			}
			if (Input.GetButtonUp("Skip")) {
				skipmode = false;
				textBox.TypeText = true;
			}

			if (skipmode && !gameObjects.ChoiceList.activeInHierarchy && !gameObjects.BacklogGameobject.activeInHierarchy && gameObjects.DisplayGameobject.activeInHierarchy) { StartCoroutine(Skiptext()); }

			if (Input.GetMouseButtonDown(1) || Input.GetKeyDown("escape")) {
				if (gameObjects.BacklogGameobject.activeInHierarchy)
					gameObjects.BacklogGameobject.SetActive(false);
			}

			if (automode && !automodeRunning) {
				StartCoroutine(AutoMode());
			}
			if (!automode && automodeRunning) {
				StopCoroutine(AutoMode());
				automodeRunning = false;
			}
		}

		foreach (Character ch in scripts.CharacterInfoScript.Characters) {
			if (ch.CharacterName == currentCharacter.CharacterName) {
				currentCharacter = ch;
			}
		}
		UpdateColor(currentCharacter);
	}

	public void ToggleAutoMode() {
		if (automode) { automode = false; } else { automode = true; }
	}

	public void NextSentence() {
		if (!ShowTextRun) {
			if (BacklogID != scripts.sentenceManager.Chapters[scripts.ChapterManagerScript.CurrentChapterIndex].Sentences.Length || scripts.ChapterManagerScript.CurrentChapterIndex != scripts.sentenceManager.Chapters.Length - 1 && choiceNext == true) {
				if (!choiceNext) {
					Sentence sentence = scripts.sentenceManager.Chapters[scripts.ChapterManagerScript.CurrentChapterIndex].Sentences[BacklogID];

					foreach (Character character in scripts.CharacterInfoScript.Characters) {
						if (character.CharacterName == sentence.Name)
							currentCharacter = character;
					}

					textBox.NameText.text = sentence.Name;

					PlayVoice(sentence.VoiceClip);

					textBox.SentenceText.text = null;

					UpdateColor(currentCharacter);

					if (textBox.TypeText) {
						StartCoroutine(ShowText(sentence.Text));
						ShowTextRun = true;
					} else {
						textBox.SentenceText.text = sentence.Text;
					}

					sentence.Viewed = true;
					switch (sentence.artworkType) {
						case Sentence.ArtworkType.BackgroundImage: {
								scripts.CGManagerScript.ShowBG(sentence.BG_ID);
								break;
							}
						case Sentence.ArtworkType.CGImage: {
								scripts.CGManagerScript.ShowCG(sentence.CG_ID);
								break;
							}
					}

					if (sentence.Choice) {
						choiceNext = true;
						Debug.Log("Choice detected");
					}

					if(!ignoreSentenceActions) {
						StartCoroutine(CallOnSentenceInit(sentence));
					}

					ignoreSentenceActions = false;

					AddItemToBacklog(BacklogID, scripts.ChapterManagerScript.CurrentChapterIndex, sentence.Name, sentence.Text, currentCharacter, sentence.Voiced, sentence.VoiceClip);
					BacklogID += 1;
				} else {
					Sentence sentence = scripts.sentenceManager.Chapters[scripts.ChapterManagerScript.CurrentChapterIndex].Sentences[BacklogID - 1];
					Debug.Log("Sentence: " + sentence.Text);
					gameObjects.ChoiceList.SetActive(true);
					foreach (Option choice in sentence.choiceOptions) {
						Debug.Log("Found " + choice.OptionText);
						AddChoiceItem(choice.OptionID, choice.OptionText);
					}
				}
			} else {
				Chapter chapter = scripts.sentenceManager.Chapters[scripts.ChapterManagerScript.CurrentChapterIndex];
				int nextChapterIndex = chapter.NextChapter;

				if(scripts.sentenceManager.Chapters.Length <= nextChapterIndex) {
					nextChapterIndex = -1;
				}

				if (nextChapterIndex != -1) {
					if (buttonArrays.ChapterJumpArray != null) {
						if (buttonArrays.ChapterJumpArray.childCount != 0) {
							buttonArrays.ChapterJumpArray.GetChild(buttonArrays.ChapterJumpArray.childCount - 1).GetComponent<ChapterJumpItem>().IsCurrentChapter = false;
						}

						Chapter nextChapter = scripts.sentenceManager.Chapters[nextChapterIndex];

						GameObject chapterJump = Instantiate(buttonPrefabs.ChapterJumpItemPreset, buttonArrays.ChapterJumpArray);
						chapterJump.name = nextChapter.ChapterName;

						ChapterJumpItem item = chapterJump.GetComponent<ChapterJumpItem>();

						item.ChapterName = nextChapter.ChapterName;
						item.ChapterIndex = chapter.NextChapter;
						item.IsCurrentChapter = true;
					}

					scripts.ChapterManagerScript.LoadChapter(nextChapterIndex);
				} else
					Debug.Log("End of sentences.");
			}
		} else {
			Sentence sentence = scripts.sentenceManager.Chapters[scripts.ChapterManagerScript.CurrentChapterIndex].Sentences[BacklogID - 1];
			StopCoroutine(ShowText(sentence.Text));
			textBox.SentenceText.maxVisibleCharacters = sentence.Text.Length;
			ShowTextRun = false;
		}
	}

	public IEnumerator CallOnSentenceInit(Sentence sentence) {
		if (scripts.characterManagerScript == null) {
			scripts.characterManagerScript = (CharacterManager)FindObjectOfType(typeof(CharacterManager));
		}

		foreach (OnSentenceInit action in sentence.onSentenceInit) {
			switch (action.actionType) {
				case OnSentenceInit.Actions.AddCharacterToScene: {
						Vector2 startingPosition = new Vector2(0, 0);

						switch (action.startingPosition) {
							case OnSentenceInit.StartingPlace.Left: {
									startingPosition = new Vector2(-150, 0);
									break;
								}
							case OnSentenceInit.StartingPlace.Right: {
									startingPosition = new Vector2(2800, 0);
									break;
								}
							case OnSentenceInit.StartingPlace.Custom: {
									startingPosition = action.customStartingPosition;
									break;
								}
						}

						scripts.characterManagerScript.AddCharacterToScene(action.CharacterName, GetCharacterState(action.CharacterName, action.CharacterState), action.Position, action.EnterScene, startingPosition, action.transitionSpeed, action.FadeIn, action.FadeSpeed);
						continue;
					}
				case OnSentenceInit.Actions.MoveCharacter: {
						scripts.characterManagerScript.MoveCharacter(action.CharacterName, action.Position, action.transitionSpeed);
						continue;
					}
				case OnSentenceInit.Actions.RemoveCharacterFromScene: {
						scripts.characterManagerScript.RemoveCharacterFromScene(action.CharacterName, action.Position, action.ExitScene, action.transitionSpeed, action.FadeOut, action.FadeSpeed);
						continue;
					}
				case OnSentenceInit.Actions.ChangeCharacterState: {
						scripts.characterManagerScript.ChangeCharacterState(action.CharacterName, GetCharacterState(action.CharacterName, action.CharacterState), action.Transition, action.FadeSpeed);
						continue;
					}
				case OnSentenceInit.Actions.Delay: {
						yield return new WaitForSeconds(action.Delay);
						continue;
					}
			}
		}
	}

	public void QuickLoadChapter(SaveData quickSaveData) {
		StopAllCoroutines();
		ShowTextRun = false;
		foreach (Transform backlogEntry in buttonArrays.BacklogButtonsArray) { backlogEntry.GetComponent<BacklogListItem>().Remove(); }
		for (int i = 0; i < quickSaveData.Choices.Count; i++) {
			BacklogEntry choice = quickSaveData.Choices[i];
			Sentence sentence = scripts.sentenceManager.Chapters[choice.chapterID].Sentences[choice.sentenceID];

			AddItemToBacklog(choice.sentenceID, choice.chapterID, sentence.Name, sentence.Text, currentCharacter, sentence.Voiced, sentence.VoiceClip);

		}
		BacklogID = quickSaveData.SentenceID;
		scripts.ChapterManagerScript.CurrentChapterIndex = quickSaveData.ChapterID;
		buttonArrays.BacklogButtonsArray.GetChild(buttonArrays.BacklogButtonsArray.childCount - 1).GetComponent<BacklogListItem>().Remove();
		NextSentence();
	}

	public CharacterState GetCharacterState(string CharacterName, string stateName) {
		CharacterState characterState = new CharacterState();
		foreach (Character character in scripts.CharacterInfoScript.Characters) {
			if (character.CharacterName == CharacterName) {
				foreach (CharacterState state in character.CharacterStates) {
					if (state.StateName == stateName) {
						characterState = state;
					}
				}
			}
		}
		return characterState;
	}

	public void AddItemToBacklog(int ID, int chapterIndex, string name, string sentence, Character currentCharacter, bool voiced, AudioClip voiceClip) {
		GameObject ListItem = (GameObject)Instantiate(buttonPrefabs.BacklogButtonPreset, transform.position, transform.rotation);
		ListItem.transform.SetParent(buttonArrays.BacklogButtonsArray);
		ListItem.GetComponent<BacklogListItem>().Setup(ID, chapterIndex, name, sentence, currentCharacter, voiced, voiceClip);
	}

	public void AddChoiceItem(int ID, string Text) {
		GameObject ListChoice = (GameObject)Instantiate(buttonPrefabs.ChoiceButtonPreset, transform.position, transform.rotation);
		ListChoice.transform.SetParent(buttonArrays.ChoiceButtonsArray);
		ListChoice.GetComponent<ChoiceListItem>().Setup(ID, Text);
	}

	public void PlayVoice(AudioClip voiceClip) {

	}

	public IEnumerator ShowText(string text) {
		textBox.SentenceText.maxVisibleCharacters = 1;
		textBox.SentenceText.text = text;
		yield return new WaitForSeconds(textBox.TextSpeed);

		for (int i = 1; i < text.Length; i++) {
			textBox.SentenceText.maxVisibleCharacters += 1;
			yield return new WaitForSeconds(textBox.TextSpeed);
		}

		ShowTextRun = false;
	}

	public IEnumerator Skiptext() {
		yield return new WaitForSeconds(textBox.TextSpeed * Time.deltaTime);
		if (!choiceNext) { NextSentence(); }
	}

	public IEnumerator AutoMode() {
		automodeRunning = true;
		while (ShowTextRun) {
			yield return null;
		}
		yield return new WaitForSeconds(AutomodeDelay);
		if (!gameObjects.ChoiceList.activeInHierarchy) { NextSentence(); }
		automodeRunning = false;
	}

	public void JumpTo(int ID, int chapterIndex) {
		scripts.characterManagerScript.RemoveAllCharactersFromScene();

		foreach(CharacterSaveData data in scripts.sentenceManager.Chapters[chapterIndex].characterSaves[ID]) {
			if(data.CharacterOnScene && !scripts.characterManagerScript.CharacterInScene(data.CharacterName)) {
				scripts.characterManagerScript.AddCharacterToScene(data.CharacterName, GetCharacterState(data.CharacterName, data.stateName), new Vector2(data.CharacterPosition[0], data.CharacterPosition[1]));
				Debug.Log(data.CharacterName + " added");
			}
		}

		ignoreSentenceActions = true;

		if (chapterIndex == scripts.ChapterManagerScript.CurrentChapterIndex) {
			BacklogID = ID;
			for (int i = ID; i < GetChildCountInArray(buttonArrays.BacklogButtonsArray); i++) {
				if (buttonArrays.BacklogButtonsArray.GetChild(i).GetComponent<BacklogListItem>().ChapterIndex >= scripts.ChapterManagerScript.CurrentChapterIndex && buttonArrays.BacklogButtonsArray.GetChild(i).GetComponent<BacklogListItem>().ID >= ID) {
					buttonArrays.BacklogButtonsArray.GetChild(i).GetComponent<BacklogListItem>().Remove();
				}
			}

			gameObjects.BacklogGameobject.SetActive(false);
			gameObjects.ChoiceList.SetActive(false);

			if (choiceNext) {
				foreach (Transform choiceButton in buttonArrays.ChoiceButtonsArray) {
					choiceButton.GetComponent<ChoiceListItem>().Remove();
				}
			}
			choiceNext = false;
			NextSentence();
		} else {
			if (chapterIndex < scripts.ChapterManagerScript.CurrentChapterIndex) {
				foreach (Transform backlogListItem in buttonArrays.BacklogButtonsArray) {
					if (backlogListItem.GetComponent<BacklogListItem>().ChapterIndex > chapterIndex) {
						backlogListItem.GetComponent<BacklogListItem>().Remove();
					}
				}
			}
			scripts.ChapterManagerScript.LoadChapter(chapterIndex, ID);
		}
	}

	public static int GetChildCountInArray(Transform Array) {
		int i = 0;
		foreach (Transform tr in Array) { ++i; }
		return i;
	}

	public List<BacklogEntry> Returnbacklog() {
		List<BacklogEntry> backlog = new List<BacklogEntry>();
		foreach (Transform backlogEntry in buttonArrays.BacklogButtonsArray) {
			BacklogEntry be = new BacklogEntry();
			BacklogListItem backlogListItem = backlogEntry.GetComponent<BacklogListItem>();

			be.sentenceID = backlogListItem.ID;
			be.chapterID = backlogListItem.ChapterIndex;
			backlog.Add(be);
		}
		return backlog;
	}

	public void ShowUI() {
		gameObjects.UI.SetActive(true);
		disableInput = false;
	}

	public void HideUI() {
		gameObjects.UI.SetActive(false);
		disableInput = true;
	}

	public void ToggleBacklog() {
		if (gameObjects.BacklogGameobject.activeInHierarchy)
			gameObjects.BacklogGameobject.SetActive(false);
		else
			gameObjects.BacklogGameobject.SetActive(true);
	}

	public void UpdateColor(Character character) {
		Color32 nameColor = character.NameColor;
		Color32 textColor = character.TextColor;

		if (!character.UseSeperateColors) {
			nameColor = character.Color;
			textColor = character.Color;
		}

		switch (character.gradientType) {
			case Character.GradientType.None: {
					textBox.NameText.colorGradient = new VertexGradient(nameColor);
					textBox.SentenceText.colorGradient = new VertexGradient(textColor);
					break;
				}
			case Character.GradientType.Name: {
					textBox.NameText.colorGradient = new VertexGradient(nameColor, nameColor, character.NameGradientColor, character.NameGradientColor);
					textBox.SentenceText.colorGradient = new VertexGradient(textColor, textColor, textColor, textColor);
					break;
				}
			case Character.GradientType.Text: {
					textBox.NameText.colorGradient = new VertexGradient(nameColor, nameColor, nameColor, nameColor);
					textBox.SentenceText.colorGradient = new VertexGradient(textColor, textColor, character.TextGradientColor, character.TextGradientColor);
					break;
				}
			case Character.GradientType.Both: {
					if (character.UseSeperateGradientColors) {
						textBox.NameText.colorGradient = new VertexGradient(nameColor, nameColor, character.NameGradientColor, character.NameGradientColor);
						textBox.SentenceText.colorGradient = new VertexGradient(textColor, textColor, character.TextGradientColor, character.TextGradientColor);
					} else {
						textBox.NameText.colorGradient = new VertexGradient(nameColor, nameColor, character.GradientColor, character.GradientColor);
						textBox.SentenceText.colorGradient = new VertexGradient(textColor, textColor, character.GradientColor, character.GradientColor);
					}
					break;
				}
		}
	}

	public void CalculateCharacterData() {
		Chapter nextChapter = scripts.sentenceManager.Chapters[scripts.ChapterManagerScript.CurrentChapterIndex];

		GameObject chapterJump = Instantiate(buttonPrefabs.ChapterJumpItemPreset, buttonArrays.ChapterJumpArray);
		chapterJump.name = nextChapter.ChapterName;

		ChapterJumpItem item = chapterJump.GetComponent<ChapterJumpItem>();

		item.ChapterName = nextChapter.ChapterName;
		item.ChapterIndex = scripts.ChapterManagerScript.CurrentChapterIndex;
		item.IsCurrentChapter = true;

		List<List<CharacterSaveData>> characterData = new List<List<CharacterSaveData>>();

		List<CharacterSaveData> chapterData = new List<CharacterSaveData>();

		foreach (Chapter chapter in scripts.sentenceManager.Chapters) {

			if (characterData.Count > 0) {
				chapterData = characterData[characterData.Count - 1];
			}

			Debug.Log(chapter.ChapterName);

			foreach (Sentence sentence in chapter.Sentences) {

				foreach (Character character in scripts.CharacterInfoScript.Characters) {
					CharacterSaveData characterSaveData = new CharacterSaveData();

					foreach (CharacterSaveData data in chapterData) {
						if (data.CharacterName == character.CharacterName) {
							characterSaveData = data;
						}
					}

					characterSaveData.CharacterName = character.CharacterName;


					foreach (OnSentenceInit sentenceInit in sentence.onSentenceInit) {
						if (sentenceInit.CharacterName == characterSaveData.CharacterName) {
							switch (sentenceInit.actionType) {
								case OnSentenceInit.Actions.AddCharacterToScene: {
										characterSaveData.CharacterOnScene = true;
										characterSaveData.CharacterPosition = new float[2] { sentenceInit.Position.x, sentenceInit.Position.y };
										characterSaveData.stateName = sentenceInit.CharacterState;
										break;
									}
								case OnSentenceInit.Actions.MoveCharacter: {
										characterSaveData.CharacterOnScene = true;
										characterSaveData.CharacterPosition = new float[2] { sentenceInit.Position.x, sentenceInit.Position.y };
										break;
									}
								case OnSentenceInit.Actions.ChangeCharacterState: {
										characterSaveData.CharacterOnScene = true;
										characterSaveData.stateName = sentenceInit.CharacterState;
										break;
									}
								case OnSentenceInit.Actions.RemoveCharacterFromScene: {
										characterSaveData.CharacterOnScene = false;
										break;
									}
							}
						}
					}
					chapterData.Add(characterSaveData);

					string nostate = "None";
					string state = (characterSaveData.stateName != null) ? characterSaveData.stateName : nostate;

					Debug.Log("Name: " + characterSaveData.CharacterName + ", On scene: " + characterSaveData.CharacterOnScene + ", Position: X" + characterSaveData.CharacterPosition[0] + ", Y" + characterSaveData.CharacterPosition[1] + ", State: " + state);
				}
			}

			characterData.Add(chapterData);

			chapter.characterSaves = characterData;
		}
	}
}

[System.Serializable]
public class GameObjects {
	public GameObject BacklogGameobject;
	public GameObject DisplayGameobject;
	public GameObject ChoiceList;
	public GameObject UI;
}

[System.Serializable]
public class ButtonArrays {
	public Transform BacklogButtonsArray;
	public Transform ChoiceButtonsArray;
	public Transform ChapterJumpArray;
}

[System.Serializable]
public class ButtonPrefabs {
	public GameObject BacklogButtonPreset;
	public GameObject ChoiceButtonPreset;
	public GameObject ChapterJumpItemPreset;
}

[System.Serializable]
public class AudioSources {
	public AudioSource VoiceAudioSource;
	public AudioSource MusicAudioSource;
}

[System.Serializable]
public class Scripts {
	[Tooltip("List of CGs and info about them")]
	public CGManager CGManagerScript;

	[Tooltip("currentCharacter Dialoge lines")]
	public SentenceManager sentenceManager;

	[Tooltip("Shared info about the currentCharacters (e.g. Text color)")]
	public CharacterInfo CharacterInfoScript;

	[Tooltip("Manages chapters")]
	public ChapterManager ChapterManagerScript;

	[Tooltip("Character in scene manager")]
	public CharacterManager characterManagerScript;
}

[System.Serializable]
public class TextBox {
	public TextMeshProUGUI NameText;
	public TextMeshProUGUI SentenceText;

	public bool TypeText = true;
	[ConditionalHide("TypeText", true)]
	public float TextSpeed = 0.1f;
}

[System.Serializable]
public class MusicTrigger {

	public AudioClip musicClip;
}
