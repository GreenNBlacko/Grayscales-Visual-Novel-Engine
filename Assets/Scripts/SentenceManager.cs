using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class SentenceManager : MonoBehaviour {

	[HideInInspector]
	public bool UseJSONFile;

	[HideInInspector]
	public bool Translating;

	[HideInInspector]
	public bool OverrideSentenceValues;

	public Chapter[] Chapters;

	public void Start() {
		if(Translating) {
			return;
		}

		DialogueManager scriptmanager = (DialogueManager)FindObjectOfType(typeof(DialogueManager));
		scriptmanager.scripts.sentenceManager = this;
		scriptmanager.NextSentence();
	}

	public void ImportSentenceDataJSON () {
		JSONSentenceImporter JsonSentenceImporter = (JSONSentenceImporter)FindObjectOfType(typeof(JSONSentenceImporter));
		CharacterInfo characterInfo = (CharacterInfo)FindObjectOfType(typeof(CharacterInfo));

		SentenceTools.sentenceManager = this;
		SentenceTools.characterInfo = characterInfo;

		JsonSentenceImporter.LoadFromJSON();
	}

	public void ImportSentenceDataCS () {
		CharacterInfo characterInfo = (CharacterInfo)FindObjectOfType(typeof(CharacterInfo));

		SentenceTools.sentenceManager = this;
		SentenceTools.characterInfo = characterInfo;

		//SentenceTools.AddChapter("ChapterName");
		//SentenceTools.AddSentence("ChapterName", "Test Character 3", "Testing if choice options are working", Sentence.ArtworkType.None);
		SentenceTools.AddCharacterState("Test Character 2", "state", CharacterState.StateType.SingleLayer, "mashs_0.png");
	}
}

#region SentenceData
[Serializable]
public class Chapter {

	public string ChapterName = "";

	[Tooltip("What chapter should be played after all the sentences in this chapter?")]
	public int NextChapter = -1;

	public Sentence[] Sentences;

	public List<List<CharacterSaveData>> characterSaves = new List<List<CharacterSaveData>>();
}  

[Serializable]
public class Sentence {
		

	[Tooltip("Character's name")]
	[ArrayToList("GetCharacterNames")]
	public string Name;

	public bool OverrideName;

	[ConditionalHide("OverrideName", true)]
	public string DisplayName;

	[Tooltip("Character's sentence")]
	[TextArea]
	public string Text;

	[Tooltip("Transition to the text")]
	public Transition transition;

	public ArtworkType artworkType = ArtworkType.None;

	[DrawIf("artworkType", ArtworkType.BackgroundImage)]
	[Tooltip("Background used ID (for more info look at CG Manager)")]
	public int BG_ID = -1;

	[DrawIf("artworkType", ArtworkType.CGImage)]
	[Tooltip("CG used ID (for more info look at CG Manager)")]
	public int CG_ID = -1;

	[Tooltip("Is this sentence a choice?")]
	public bool Choice = true;

	[Tooltip("Choice Options (will be used if Choice option is ticked above)")]
	public Option[] choiceOptions;

	[Tooltip("Was the text line viewed already?")]
	public bool Viewed;

	[Tooltip("Is the sentence voiced?")]    
	public bool Voiced;

	[Tooltip("VA clip (will be used if Voiced is is ticked above)")]
	[DrawIf("Voiced", true)]
	public AudioClip VoiceClip;

	[Tooltip("Actions taken when the sentence is played")]
	public OnSentenceInit[] onSentenceInit;

	//not important
	[SerializeField]
	public enum Transition { None, FadeOut };
	public enum ArtworkType { None, BackgroundImage, CGImage };
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

	[ArrayToList("GetCharacterStates", "CharacterName")]
	public string CharacterState;

	[Tooltip("Wait x seconds before executing the next action")]
	public float Delay;

	public bool FadeIn;

	public bool FadeOut;

	public bool Transition;

	[Range(1,0)]
	public float FadeSpeed;

	[Tooltip("Should the character enter the scene or just be spawned in?")]
	public bool EnterScene = true;

	[Tooltip("Should the character exit the scene or just be removed?")]
	public bool ExitScene = true;

	public StartingPlace startingPosition;

	public Vector2 customStartingPosition;

	[Range(2, 0)]
	public float transitionSpeed = 0.1f;

	[Tooltip("Position that the character is going to be placed in(2650x1440 base resolution, downscaled bsaed on the screen resolution)")]
	public Vector2 Position;

	public enum Actions { AddCharacterToScene, MoveCharacter, RemoveCharacterFromScene, ChangeCharacterState, Delay };
	public enum StartingPlace { Left, Right, Custom };
}

#endregion

