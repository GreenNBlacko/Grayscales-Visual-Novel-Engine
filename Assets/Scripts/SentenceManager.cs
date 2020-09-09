using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class SentenceManager : MonoBehaviour {

	[HideInInspector]
	public bool UseJSONFile;

	[HideInInspector]
	public bool OverrideSentenceValues;

	public Chapter[] Chapters;

	public void Start() {

		DialogueManager scriptmanager = (DialogueManager)FindObjectOfType(typeof(DialogueManager));
		scriptmanager.scripts.sentenceManager = this;
		scriptmanager.NextSentence();
	}

	public void ImportSentenceDataJSON () {
		JSONSentenceImporter JsonSentenceImporter = (JSONSentenceImporter)FindObjectOfType(typeof(JSONSentenceImporter));
		CharacterInfo characterInfo = (CharacterInfo)FindObjectOfType(typeof(CharacterInfo));

		JsonSentenceImporter.LoadFromJSON();
	}

	public void ImportSentenceDataCS () {
		SentenceTools.AddChapter(this, "ChapterName");
		SentenceTools.AddSentence(this, "ChapterName", "Test Character 3", "Testing if choice options are working", Sentence.ArtworkType.None);
	}
}

#region SentenceData
[Serializable]
public class Chapter {

	public string ChapterName = "";

	[Tooltip("What chapter should be played after all the sentences in this chapter?")]
	public int NextChapter = -1;

	public Sentence[] Sentences;

}  

[Serializable]
public class Sentence {
		

	[Tooltip("Character's name")]
	[ArrayToList("GetCharacterNames")]
	public string Name;

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

	[ArrayToList("GetCharacterStates", "CharacterName", "actionType", new object[2] { Actions.AddCharacterToScene, Actions.ChangeCharacterState })]
	public string CharacterState;

	[DrawIf("actionType", Actions.AddCharacterToScene)]
	[Tooltip("Should the character enter the scene or just be spawned in?")]
	public bool EnterScene = true;

	[ConditionalHide("EnterScene", true)]
	public StartingPlace startingPosition;

	[DrawIf("startingPosition", StartingPlace.Custom, StartingPlace.Custom, "EnterScene", true)]
	public Vector2 customStartingPosition;

	[Range(2, 0)]
	public float transitionSpeed = 0.1f;

	[DrawIfAny("actionType", new object[2] { Actions.AddCharacterToScene, Actions.MoveCharacter })]
	[Tooltip("Position that the character is going to be placed in(2650x1440 base resolution, downscaled bsaed on the screen resolution)")]
	public Vector2 Position;

	public enum Actions { AddCharacterToScene, MoveCharacter, RemoveCharacterFromScene, ChangeCharacterState };
	public enum StartingPlace { Left, Right, Custom };
}
#endregion

