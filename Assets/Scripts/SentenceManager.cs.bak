using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sentence Manager", menuName = "Sentence Manager", order = 1)]
public class SentenceManager : ScriptableObject {

	[HideInInspector]
	public bool UseJSONFile;

	[HideInInspector]
	public bool Translating;

	[HideInInspector]
	public bool OverrideSentenceValues;

	public Chapter[] Chapters;

	public void Start() {
		if (Translating) {
			return;
		}

		DialogueManager scriptmanager = (DialogueManager)FindObjectOfType(typeof(DialogueManager));
		scriptmanager.scripts.sentenceManager = this;
		scriptmanager.NextSentence();
	}

	public void ImportSentenceDataJSON() {
		JSONSentenceImporter JsonSentenceImporter = (JSONSentenceImporter)FindObjectOfType(typeof(JSONSentenceImporter));
		CharacterInfo characterInfo = (CharacterInfo)FindObjectOfType(typeof(CharacterInfo));

		SentenceTools.sentenceManager = this;
		SentenceTools.characterInfo = characterInfo;

		JsonSentenceImporter.LoadFromJSON();
	}

	public void ImportSentenceDataCS() {
		CharacterInfo characterInfo = (CharacterInfo)FindObjectOfType(typeof(CharacterInfo));

		SentenceTools.sentenceManager = this;
		SentenceTools.characterInfo = characterInfo;
	}

	public void ExportSentenceDataJson() {
		JSONSentenceImporter JsonSentenceImporter = (JSONSentenceImporter)FindObjectOfType(typeof(JSONSentenceImporter));

		JsonSentenceImporter.ExportFromJSON();
	}
}

#region SentenceData
[Serializable]
public class Chapter {

	public string ChapterName = "";

	[ArrayToList("GetChapterNames")]
	[Tooltip("What chapter should be played after all the sentences in this chapter?")]
	public int NextChapter = -1;

	public List<Sentence> Sentences;

	public List<List<CharacterSaveData>> characterSaves = new List<List<CharacterSaveData>>();
}

[Serializable]
public class Sentence {

	[HideInInspector]
	public bool ChoiceActive;

	[HideInInspector]
	public bool onSentenceInitActive;

	[Tooltip("Character's name")]
	[ArrayToList("GetCharacterNames")]
	public string Name;

	public bool OverrideName;

	[DrawIf("OverrideName", true)]
	public string DisplayName;

	[Tooltip("Character's sentence")]
	[TextArea(3,15)]
	public string Text;

	[Tooltip("Transition to the text")]
	public Transition transition;

	[Tooltip("Is this sentence a choice?")]
	public bool Choice = true;

	public Option[] choiceOptions;

	[Tooltip("Was the text line viewed already?")]
	public bool Viewed;

	[Tooltip("Is the sentence voiced?")]
	public bool Voiced;

	[DrawIf("Voiced", true)]
	[Tooltip("VA clip (will be used if Voiced is is ticked above)")]
	public AudioClip VoiceClip;

	[Tooltip("Actions taken when the sentence is played")]
	public OnSentenceInit[] onSentenceInit;

	//not important
	[SerializeField]
	public enum Transition { None, FadeOut };
}

[Serializable]
public class Option {
	public int OptionID;
	public string OptionText;
}

[Serializable]
public class OnSentenceInit {

	public Actions actionType;

	[ArrayToList("GetCharacterNames")]
	public string CharacterName;

	[ArrayToList("GetCharacterVariants", "CharacterName")]
	public string CharacterVariant;

	[ArrayToList("GetCharacterStates", "CharacterName", "CharacterVariant")]
	public string CharacterState;

	[Tooltip("Wait x seconds before executing the next action")]
	public float Delay;

	public bool FadeIn;

	public bool FadeOut;

	public bool Transition;

	[Range(2, 0)]
	public float FadeSpeed = 0.5f;

	public bool advanced = false;

	public Vector2 CharacterOffset;

	[ArrayToList("GetBGList")]
	[Tooltip("Background used")]
	public int BG;

	[ArrayToList("GetCGList")]
	[Tooltip("CG used")]
	public int CG;

	[Tooltip("Higher index means that the character will appear in front of other characters, lower - behind them")]
	public int CharacterIndex;

	[Tooltip("If checked, the code will return to regular operation after the action is finished, if not, the code will run until this action is called again.")]
	public bool RunOnlyOnce;

	[Tooltip("Should the character enter the scene or just be spawned in?")]
	public bool EnterScene = true;

	[Tooltip("Should the character exit the scene or just be removed?")]
	public bool ExitScene = true;

	public StartingPlace startingPosition;

	public Vector2 customStartingPosition;

	[Range(2, 0)]
	public float transitionSpeed = 1.43f;

	[Tooltip("Position that the character is going to be placed in(2650x1440 base resolution, downscaled bsaed on the screen resolution)")]
	public Vector2 Position;

	public enum Actions { AddCharacterToScene, MoveCharacter, ShowTransition, ShowBG, ShowCG, RemoveCharacterFromScene, ChangeCharacterState, Delay, WaitUntilActionIsFinished, SetCharacterIndex, PlayBGM };
	public enum StartingPlace { Left, Right, Custom };

	public OnSentenceInit() {
		FadeSpeed = 0.7f;
		transitionSpeed = 1.43f;
	}

	public OnSentenceInit(Actions actionType = 0, string characterName = "", string characterState = "", float delay = 0f, bool fadeIn = false, bool fadeOut = false, bool transition = false, float fadeSpeed = 0.7f, int bg = 0, int cg = 0, int characterIndex = 0, bool runOnlyOnce = false, bool enterScene = false, bool exitScene = false, StartingPlace startingPosition = StartingPlace.Left, Vector2 customStartingPosition = default, float transitionSpeed = 1.43f, Vector2 position = default) {
		this.actionType = actionType;
		CharacterName = characterName;
		CharacterState = characterState;
		Delay = delay;
		FadeIn = fadeIn;
		FadeOut = fadeOut;
		Transition = transition;
		FadeSpeed = fadeSpeed;
		BG = bg;
		CG = cg;
		CharacterIndex = characterIndex;
		RunOnlyOnce = runOnlyOnce;
		EnterScene = enterScene;
		ExitScene = exitScene;
		this.startingPosition = startingPosition;
		this.customStartingPosition = customStartingPosition;
		this.transitionSpeed = transitionSpeed;
		Position = position;
	}
}

#endregion