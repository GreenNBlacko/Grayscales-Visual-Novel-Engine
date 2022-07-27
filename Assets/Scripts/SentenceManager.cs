using System;
using System.Collections.Generic;
using UnityEngine;
using Localisation.Languages;

[CreateAssetMenu(fileName = "Sentence Manager", menuName = "Sentence Manager", order = 1)]
public class SentenceManager : ScriptableObject {
	[Header("Sentence Manager")]
	public List<Chapter> Chapters = new List<Chapter>();

	public int DefaultLanguagePack;
	public int SelectedLanguagePack;
	
	[Space(5)]

	public List<LanguagePack> LanguagePacks = new List<LanguagePack>();

	[Header("Character Info")]
	public List<Character> Characters = new List<Character>();

	[Header("CG Manager")]
	public List<Artwork> BGList = new List<Artwork>();
	public List<Artwork> CGList = new List<Artwork>();
}

#region SentenceData
[Serializable]
public class Chapter {
	public List<ChapterPlayRequirements> playRequirements;

	public Sprite CoverImage;

	public List<Sentence> Sentences;

	public List<List<CharacterSaveData>> characterSaves = new List<List<CharacterSaveData>>();

	
}


[Serializable]
public class ChapterPlayRequirements {
	public int choiceIndex;

	public List<ChapterPlayRequirement> ChoiceOptions;

	public enum ChapterPlayRequirement { Required, Optional, NotRequired };
}

[Serializable]
public class Sentence {
	[Tooltip("Character's name")]
	public int Name;

	public bool OverrideName;

	[Tooltip("Is this sentence a choice?")]
	public bool Choice = true;
	public int ChoiceOptionsCount;

	[Tooltip("Is the sentence voiced?")]
	public bool Voiced;

	[Tooltip("VA clip (will be used if Voiced is is ticked above)")]
	public AudioClip VoiceClip;

	[Tooltip("Actions taken when the sentence is played")]
	public List<OnSentenceInit> onSentenceInit = new List<OnSentenceInit>();

	public Sentence() {
		Name = 0;
		OverrideName = false;
		Choice = false;
		Voiced = false;
		VoiceClip = null;
		onSentenceInit = new List<OnSentenceInit>();
	}

	public Sentence(int characterName, bool overrideName, bool choice, bool voiced, AudioClip voiceClip, List<OnSentenceInit> sentenceActions) {
		Name = characterName;
		OverrideName = overrideName;
		Choice = choice;
		Voiced = voiced;
		VoiceClip = voiceClip;
		onSentenceInit = sentenceActions;
	}
}

[Serializable]
public class OnSentenceInit {

	public Actions actionType;
	public int CharacterName;

	public int CharacterVariant;

	public int CharacterState;

	[Tooltip("Wait x seconds before executing the next action")]
	public float Delay;

	public bool FadeIn;

	public bool FadeOut;

	public bool Transition;

	[Range(2, 0)]
	public float FadeSpeed = 0.5f;

	public bool advanced = false;

	public Vector2 CharacterOffset;

	[Tooltip("Background used")]
	public int BG;

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

	public OnSentenceInit(Actions actionType = 0, int characterName = 0, int characterVariant = 0, int characterState = 0, float delay = 0f, bool fadeIn = false, bool fadeOut = false, bool transition = false, float fadeSpeed = 0.7f, int bg = 0, int cg = 0, int characterIndex = 0, bool runOnlyOnce = false, bool enterScene = false, bool exitScene = false, StartingPlace startingPosition = StartingPlace.Left, Vector2 customStartingPosition = default, float transitionSpeed = 1.43f, Vector2 position = default) {
		this.actionType = actionType;
		CharacterName = characterName;
		CharacterVariant = characterVariant;
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

[Serializable]
public class Character {
	public int CharacterName;

	[Tooltip("The scale of the character's sprites(1 is default)")]
	public Vector2 CharacterScale = new Vector2(1, 1);

	[Tooltip("character's emotions and appearance")]
	public List<CharacterVariant> characterVariants = new List<CharacterVariant>();

	[Tooltip("Use seperate colors for name and text?")]
	public bool UseSeperateColors = false;

	public Color32 Color = new Color32(255, 255, 255, 255);

	[Tooltip("Character's name color")]
	public Color32 NameColor = new Color32(255, 255, 255, 255);

	[Tooltip("Text color")]
	public Color32 TextColor = new Color32(255, 255, 255, 255);

	[Tooltip("Use gradient for text?")]
	public GradientType gradientType = GradientType.None;

	public bool UseSeperateGradientColors = false;

	[Tooltip("Name text gradient color")]
	public Color32 NameGradientColor = new Color32(255, 255, 255, 255);

	[Tooltip("Sentence text gradient color")]
	public Color32 TextGradientColor = new Color(255, 255, 255);

	[Tooltip("Name and sentence text gradient color")]
	public Color32 GradientColor = new Color32(255, 255, 255, 255);

	public enum GradientType { None, Name, Text, Both };
}

[Serializable]
public class CharacterVariant {
	public string VariantName;

	public List<CharacterState> variantStates = new List<CharacterState>();
}

[Serializable]
public class CharacterState {
	public string StateName;
	public StateType stateType;

	public Sprite BaseLayer;
	public Sprite ExpressionLayer;

	public bool Advanced;

	public Vector2 BaseLayerPosition;
	public Vector2 ExpressionLayerPosition;

	public enum StateType { SingleLayer, MultiLayer };

	public CharacterState() { }

	public CharacterState(string stateName, StateType type, Sprite baseLayer, Sprite expressionLayer = default, bool advanced = false, Vector2 baseLayerPosition = default, Vector2 expressionLayerPosition = default) {
		StateName = stateName;
		stateType = type;

		BaseLayer = baseLayer;
		ExpressionLayer = expressionLayer;
		
		Advanced = advanced;
		if (advanced) {
			BaseLayerPosition = baseLayerPosition;
			if (type == StateType.MultiLayer) ExpressionLayerPosition = expressionLayerPosition;
		}
	}
}

[Serializable]
public class Artwork {
	public string Name;
	public Sprite artworkImage;
}

namespace Localisation.Languages {
	using LanguagePackData;

	[Serializable]
	public class LanguagePack {
		public string languageName;

		public List<Chapter> chapters = new List<Chapter>();

		public List<Character> characters = new List<Character>();

		public LanguagePack() { }

		public LanguagePack(LanguagePack reference) {
			chapters = new List<Chapter>(reference.chapters);
			characters = new List<Character>(reference.characters);
		}
	}

	namespace LanguagePackData {
		[Serializable]
		public class Chapter {
			public string ChapterName;
			public string ChapterSummary;

			public Sprite OverrideCoverImage;

			public List<Sentence> sentences = new List<Sentence>();
		}

		[Serializable]
		public class Sentence {
			public string DisplayName;

			public string Text;

			public List<string> ChoiceData;

			public AudioClip OverrideVoiceline;
		}

		[Serializable]
		public class Character {
			public string CharacterName;
		}
	}
}
#endregion