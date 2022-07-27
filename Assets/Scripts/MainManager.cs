using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour {
	public static MainManager instance;

	private UIManager ui;
	private CurrentState currentState = new CurrentState();

	public static SentenceManager sentenceData;

	// ----------------------------------------- EASY --- ACCESS --- VARIABLES ----------------------------------------- \\
	[HideInInspector] public int currentChapter => currentState.currentChapter;
	[HideInInspector] public int currentSentence => currentState.currentSentence;

	[HideInInspector] public List<int> currentChoices => currentState.currentChoices;
	[HideInInspector] public List<CurrentCharacterState> currentCharacterStates => currentState.currentCharacterStates;
	// ----------------------------------------- EASY --- ACCESS --- VARIABLES ----------------------------------------- \\

	private void Awake() {
		if (instance != null)
			Destroy(instance.gameObject);

		instance = this;
	}

	private void Start() {
		// ------------------------------------- PRELOAD ------------------------------------- \\
		if (UIManager.instance == null) {
			Util.SendError(1);
		}

		ui = UIManager.instance;

		if(sentenceData == null) {
			Util.SendError(2);
		}
		// ------------------------------------- PRELOAD ------------------------------------- \\


		ui.RunUpdate();
	}
}

public static class Util {
	public static void SendError(int errorCode) {
		string ErrorText = " Error code: " + errorCode;

		switch (errorCode) {
			default: {
				ErrorText = "Unknown error occured!" + ErrorText;
				break;
			}
			case 0: {
				ErrorText = "Fatal error occured! Missing Main Manager instance. Ensure you have added the script 'MainManager' into your scene." + ErrorText;
				break;
			}
			case 1: {
				ErrorText = "Fatal error occured! Missing UI Manager instance. Ensure you have added the script 'UIManager' into your scene." + ErrorText;
				break;
			}
			case 2: {
				ErrorText = "Fatal error occured! Missing Sentence Manager instance. Ensure you have added a 'SentenceManager' object in the 'MainManager' script." + ErrorText;
				break;
			}
			case 3: {
				ErrorText = "Fatal error occured! Missing Text Box. Ensure you have added a 'TextBox' object in the 'UIManager' script." + ErrorText;
				break;
			}
		}

		Debug.LogError(ErrorText);

		Debug.Break();
	}
}

public class CurrentState {
	public int currentChapter;
	public int currentSentence;

	public List<int> currentChoices;

	public List<CurrentCharacterState> currentCharacterStates;
}

public class CurrentCharacterState {
	public bool CharacterInScene;

	public Vector2 CharacterPosition;
	public int CharacterVariant;
	public int CharacterState;

}
