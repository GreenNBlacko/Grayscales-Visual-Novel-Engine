using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(CGManager))]
[RequireComponent(typeof(ChapterManager))]
[RequireComponent(typeof(CharacterManager))]
public class DialogueManager : MonoBehaviour {

	public GameObjects gameObjects;

	public ButtonArrays buttonArrays;

	public ButtonPrefabs buttonPrefabs;

	public AudioSources audioSources;

	public Scripts scripts;

	public TextBox textBox;

	[HideInInspector]
	public int BacklogID = 0;

	[HideInInspector]
	public bool ShowTextRun = false;

	private bool ignoreSentenceActions;

	private Sentence currentSentence = new Sentence();

	[HideInInspector]
	public bool choiceNext = false;

	[HideInInspector]
	public bool skipmode = false;

	[HideInInspector]
	public bool disableInput = false;

	[HideInInspector]
	public bool automode = false;

	private bool WaitUntilMotionIsfinishedTemp = false;
	private bool WaitUntilMotionIsfinished = false;

	private Character currentCharacter = new Character();

	private bool cubeAnim = false;

	[HideInInspector]
	public bool automodeRunning = false;
	[Range(0, 6)]
	[Tooltip("How many seconds should automode wait before showing next line?")]
	[Header("Automode")]
	public float AutomodeDelay = 1.5F;

	void Start() {
		gameObjects.BacklogGameobject.SetActive(false);
		gameObjects.ChoiceList.SetActive(false);
		gameObjects.DisplayGameobject.SetActive(true);
		choiceNext = false;

		CalculateCharacterData();

		StartCoroutine(CubeAnimation());
		NextSentence();

		foreach (Transform temp in buttonArrays.BacklogButtonsArray) {
			Destroy(temp.gameObject);
		}
	}

	void Update() {
		if (!disableInput) {
			/*if (Input.GetKeyDown("space") && !gameObjects.BacklogGameobject.activeInHierarchy && !gameObjects.ChoiceList.activeInHierarchy || Input.mouseScrollDelta.y < 0 && !gameObjects.BacklogGameobject.activeInHierarchy && !gameObjects.ChoiceList.activeInHierarchy) {
				NextSentence();
				skipmode = false;
				automode = false;
			}
			if (Input.mouseScrollDelta.y > 0 || Input.GetKeyDown(KeyCode.PageUp)) {
				if (!gameObjects.BacklogGameobject.activeInHierarchy) {
					gameObjects.BacklogGameobject.SetActive(true);
					gameObjects.DisplayGameobject.SetActive(false);
				}
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
				automode = false;
				textBox.TypeText = false;
			}
			if (Input.GetButtonUp("Skip")) {
				skipmode = false;
				textBox.TypeText = true;
			}*/

			if (skipmode && !gameObjects.ChoiceList.activeInHierarchy && !gameObjects.BacklogGameobject.activeInHierarchy && gameObjects.DisplayGameobject.activeInHierarchy) { StartCoroutine(Skiptext()); }

			/*if (Input.GetMouseButtonDown(1) || Input.GetKeyDown("escape")) {
				if (gameObjects.BacklogGameobject.activeInHierarchy) {
					gameObjects.BacklogGameobject.SetActive(false);
					gameObjects.DisplayGameobject.SetActive(true);
				}
			}*/

			if (automode && !automodeRunning) {
				StartCoroutine(AutoMode());
			}
		}

		foreach (Character ch in scripts.sentenceManager.Characters) {
			if (ch.CharacterName == currentCharacter.CharacterName) {
				currentCharacter = ch;
			}
		}
		UpdateColor(currentCharacter);
	}

	public void ToggleAutoMode() {
		automode = !automode;
	}

	public void NextSentence() {
		NextSentence(new CallbackContext());
	}

	bool IsMouseOverGameWindow { get { return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y); } }

	public bool MouseInNoTouchZone() {
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = Input.mousePosition;

		List<RaycastResult> raycastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, raycastResults);

		foreach (RaycastResult result in raycastResults) {
			if (result.gameObject.tag == "NoTouchZone")
				return true;
		}

		return false;
	}

	public bool ActionPerformed(CallbackContext ctx) {
		if (MouseInNoTouchZone() && BacklogID > 0 ||
			ctx.canceled ||
			ctx.started ||
			ctx.performed && !IsMouseOverGameWindow && (ctx.action.activeControl.device.name == "Keyboard" ||
			ctx.action.activeControl.device.name == "Mouse"))
			return false;

		return true;
	}

	public async void NextSentence(CallbackContext ctx) {
		if (!ActionPerformed(ctx))
			return;

		if (ctx.control != null) {
			skipmode = false;
			automode = false;
		}

		if (!ShowTextRun) {
			if (BacklogID != scripts.sentenceManager.Chapters[scripts.ChapterManagerScript.CurrentChapterIndex].Sentences.Count || scripts.ChapterManagerScript.CurrentChapterIndex != scripts.sentenceManager.Chapters.Count - 1 && choiceNext == true) {
				if (!choiceNext) {
					currentSentence = scripts.sentenceManager.Chapters[scripts.ChapterManagerScript.CurrentChapterIndex].Sentences[BacklogID];
					currentCharacter = scripts.sentenceManager.Characters[currentSentence.Name];

					var lang = SentenceTools.GetSelectedLanguagePack();
					var langSent = lang.chapters[scripts.ChapterManagerScript.CurrentChapterIndex].sentences[BacklogID];
					var langChar = lang.characters[currentSentence.Name];

					if (currentSentence.OverrideName)
						textBox.NameText.text = langSent.DisplayName;
					else
						textBox.NameText.text = langChar.CharacterName;

					if(currentSentence.Voiced && (currentSentence.VoiceClip != null || langSent.OverrideVoiceline)) PlayVoice(currentSentence.VoiceClip);

					textBox.SentenceText.text = null;

					UpdateColor(currentCharacter);

					if (textBox.TypeText) {
						StartCoroutine(ShowText());
					} else {
						textBox.SentenceText.text = langSent.Text;
					}

					if (currentSentence.Choice) {
						choiceNext = true;
						Debug.Log("Choice detected");
					}

					if (!ignoreSentenceActions) {
						await CallOnSentenceInit();
					}

					ignoreSentenceActions = false;
					BacklogID += 1;
				} else {
					
				}
			} else {
				Debug.Log("End of sentences.");
			}
		} else {
			ShowTextRun = false;
			var lang = SentenceTools.GetSelectedLanguagePack();
			var langSent = lang.chapters[scripts.ChapterManagerScript.CurrentChapterIndex].sentences[BacklogID];


			textBox.SentenceText.maxVisibleCharacters = -1;
		}
	}

	public void LastSentence() {
		if (BacklogID < 1) {
			if (scripts.ChapterManagerScript.CurrentChapterIndex < 1)
				JumpTo(scripts.sentenceManager.Chapters[scripts.ChapterManagerScript.CurrentChapterIndex - 1].Sentences.Count - 1, scripts.ChapterManagerScript.CurrentChapterIndex - 1);
		} else {
			JumpTo(BacklogID - 1, scripts.ChapterManagerScript.CurrentChapterIndex);
		}
	}

	public async Task CallOnSentenceInit() {
		if (scripts.characterManagerScript == null) {
			scripts.characterManagerScript = (CharacterManager)FindObjectOfType(typeof(CharacterManager));
		}

		foreach (OnSentenceInit action in currentSentence.onSentenceInit) {
			switch (action.actionType) {
				case OnSentenceInit.Actions.AddCharacterToScene: {
					if(WaitUntilMotionIsfinishedTemp || WaitUntilMotionIsfinished)
						await scripts.characterManagerScript.AddCharacterToScene(action.CharacterName, action.startingPosition, action.customStartingPosition, GetCharacterState(action.CharacterName, action.CharacterVariant, action.CharacterState), action.Position, action.EnterScene, action.transitionSpeed, action.FadeIn, action.FadeSpeed);
					else
						_ = scripts.characterManagerScript.AddCharacterToScene(action.CharacterName, action.startingPosition, action.customStartingPosition, GetCharacterState(action.CharacterName, action.CharacterVariant, action.CharacterState), action.Position, action.EnterScene, action.transitionSpeed, action.FadeIn, action.FadeSpeed);

					WaitUntilMotionIsfinishedTemp = false;

					Debug.Log("Add finished");
					continue;
				}
				case OnSentenceInit.Actions.MoveCharacter: {
					if (WaitUntilMotionIsfinished || WaitUntilMotionIsfinishedTemp) 
						await scripts.characterManagerScript.MoveCharacter(action.CharacterName, action.Position, action.transitionSpeed);
					else
						_ = scripts.characterManagerScript.MoveCharacter(action.CharacterName, action.Position, action.transitionSpeed);

					WaitUntilMotionIsfinishedTemp = false;
					continue;
				}
				case OnSentenceInit.Actions.RemoveCharacterFromScene: { 
					if (WaitUntilMotionIsfinished || WaitUntilMotionIsfinishedTemp)
						await scripts.characterManagerScript.RemoveCharacterFromScene(action.CharacterName, action.Position, action.ExitScene, action.transitionSpeed, action.FadeOut, action.FadeSpeed);
					else
						_ = scripts.characterManagerScript.RemoveCharacterFromScene(action.CharacterName, action.Position, action.ExitScene, action.transitionSpeed, action.FadeOut, action.FadeSpeed);

					WaitUntilMotionIsfinishedTemp = false;
					continue;
				}
				case OnSentenceInit.Actions.ChangeCharacterState: {
					if (WaitUntilMotionIsfinished || WaitUntilMotionIsfinishedTemp)
						await scripts.characterManagerScript.ChangeCharacterState(action.CharacterName, GetCharacterState(action.CharacterName, action.CharacterVariant, action.CharacterState), action.Transition, action.transitionSpeed);
					else
						_ = scripts.characterManagerScript.ChangeCharacterState(action.CharacterName, GetCharacterState(action.CharacterName, action.CharacterVariant, action.CharacterState), action.Transition, action.transitionSpeed);

					WaitUntilMotionIsfinishedTemp = false;
					continue;
				}
				case OnSentenceInit.Actions.Delay: {
					await Task.Delay(Mathf.RoundToInt(action.Delay * 1000));
					continue;
				}
				case OnSentenceInit.Actions.ShowBG: {
					if (WaitUntilMotionIsfinished || WaitUntilMotionIsfinishedTemp)
						await scripts.CGManagerScript.ShowBG(action.BG, action.Transition, action.transitionSpeed);
					else
						_ = scripts.CGManagerScript.ShowBG(action.BG, action.Transition, action.transitionSpeed);

					WaitUntilMotionIsfinishedTemp = false;
					continue;
				}
				case OnSentenceInit.Actions.ShowCG: {
					if (WaitUntilMotionIsfinished || WaitUntilMotionIsfinishedTemp)
						await scripts.CGManagerScript.ShowCG(action.CG, action.Transition, action.transitionSpeed);
					else
						_ = scripts.CGManagerScript.ShowCG(action.CG, action.Transition, action.transitionSpeed);

					WaitUntilMotionIsfinishedTemp = false;
					continue;
				}
				case OnSentenceInit.Actions.WaitUntilActionIsFinished: {
					if (action.RunOnlyOnce) {
						WaitUntilMotionIsfinishedTemp = true;
						WaitUntilMotionIsfinished = false;
					} else {
						WaitUntilMotionIsfinished = !WaitUntilMotionIsfinished;
					}
					continue;
				}
				case OnSentenceInit.Actions.SetCharacterIndex: {
					scripts.characterManagerScript.SetCharacterIndex(action.CharacterName, action.CharacterIndex);
					continue;
				}
			}
		}
	}

	public void QuickLoadChapter(SaveData quickSaveData) {
		StopAllCoroutines();
		ShowTextRun = false;
		foreach (Transform backlogEntry in buttonArrays.BacklogButtonsArray) { backlogEntry.GetComponent<BacklogListItem>().Remove(); }

		//Update Backlog

		BacklogID = quickSaveData.SentenceID;
		scripts.ChapterManagerScript.CurrentChapterIndex = quickSaveData.ChapterID;
		buttonArrays.BacklogButtonsArray.GetChild(buttonArrays.BacklogButtonsArray.childCount - 1).GetComponent<BacklogListItem>().Remove();
		NextSentence();
	}

	public CharacterState GetCharacterState(int CharacterName, int variantName, int stateName) {
		return scripts.sentenceManager.Characters[CharacterName].characterVariants[variantName].variantStates[stateName];
	}

	public void AddItemToBacklog(int ID, int chapterIndex, string name, string sentence, Character currentCharacter, bool voiced, AudioClip voiceClip) {
		GameObject ListItem = Instantiate(buttonPrefabs.BacklogButtonPreset, transform.position, transform.rotation);
		ListItem.transform.SetParent(buttonArrays.BacklogButtonsArray);
		ListItem.GetComponent<BacklogListItem>().Setup(ID, chapterIndex, name, sentence, currentCharacter, voiced, voiceClip);
	}

	public void AddChoiceItem(int ID, string Text) {
		GameObject ListChoice = Instantiate(buttonPrefabs.ChoiceButtonPreset, transform.position, transform.rotation);
		ListChoice.transform.SetParent(buttonArrays.ChoiceButtonsArray);
		ListChoice.GetComponent<ChoiceListItem>().Setup(ID, Text);
	}

	public void PlayVoice(AudioClip voiceClip) {

	}

	public IEnumerator ShowText() {
		ShowTextRun = true;

		cubeAnim = false;

		textBox.SentenceText.maxVisibleCharacters = 1;
		textBox.SentenceText.text = langSent.Text;
		yield return new WaitForSeconds(textBox.TextSpeed);

		while (textBox.SentenceText.maxVisibleCharacters < textBox.SentenceText.text.Length && ShowTextRun) {
			textBox.SentenceText.maxVisibleCharacters += 1;
			yield return new WaitForSeconds(textBox.TextSpeed);
		}

		textBox.SentenceText.maxVisibleCharacters += 15;

		ShowTextRun = false;

		cubeAnim = true;
		StartCoroutine(CubeAnimation());
	}

	public IEnumerator CubeAnimation() {
		while (cubeAnim) {
			if (!cubeAnim)
				break;

			for (int i = 0; i < 32 && cubeAnim; i++) {
				textBox.SentenceText.text = currentSentence.Text + "<sprite=" + i + ">";
				yield return new WaitForSeconds(0.07f);
			}

			yield return new WaitForSeconds(0.1f);
		}
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
		yield return new WaitForSeconds(scripts.sentenceManager.Chapters[scripts.ChapterManagerScript.CurrentChapterIndex].Sentences[BacklogID - 1].Text.Length * textBox.TextSpeed + AutomodeDelay);

		if (!automode) { automodeRunning = false; yield break; }

		if (!gameObjects.ChoiceList.activeInHierarchy) { NextSentence(); } else
			automode = false;

		automodeRunning = false;
	}

	public void JumpTo(int ID, int chapterIndex) {
		Debug.Log(ID);
		Debug.Log(chapterIndex);

		scripts.characterManagerScript.RemoveAllCharactersFromScene();


		foreach (CharacterSaveData data in scripts.sentenceManager.Chapters[chapterIndex].characterSaves[ID]) {
			if (data.CharacterOnScene && !scripts.characterManagerScript.CharacterInScene(data.CharacterName)) {
				_ = scripts.characterManagerScript.AddCharacterToScene(data.CharacterName, GetCharacterState(data.CharacterName, data.variantName, data.stateName), new Vector2(data.CharacterPosition[0], data.CharacterPosition[1]));
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

		if (buttonArrays.ChapterJumpArray != null) {
			GameObject chapterJump = Instantiate(buttonPrefabs.ChapterJumpItemPreset, buttonArrays.ChapterJumpArray);
			chapterJump.name = nextChapter.ChapterName;

			ChapterJumpItem item = chapterJump.GetComponent<ChapterJumpItem>();

			item.ChapterName = nextChapter.ChapterName;
			item.ChapterIndex = scripts.ChapterManagerScript.CurrentChapterIndex;
			item.IsCurrentChapter = true;
		}

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
									characterSaveData.variantName = sentenceInit.CharacterVariant;
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
									characterSaveData.variantName = sentenceInit.CharacterVariant;
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
					string state = characterSaveData.stateName ?? nostate;

					//Debug.Log("Name: " + characterSaveData.CharacterName + ", On scene: " + characterSaveData.CharacterOnScene + ", Position: X" + characterSaveData.CharacterPosition[0] + ", Y" + characterSaveData.CharacterPosition[1] + ", State: " + state);
				}

				characterData.Add(chapterData);
			}

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

	[Tooltip("Dialoge lines")]
	public SentenceManager sentenceManager;

	[Tooltip("Manages chapters")]
	public ChapterManager ChapterManagerScript;

	[Tooltip("Character in scene manager")]
	public CharacterManager characterManagerScript;
}

[System.Serializable]
public class MusicTrigger {

	public AudioClip musicClip;
}
