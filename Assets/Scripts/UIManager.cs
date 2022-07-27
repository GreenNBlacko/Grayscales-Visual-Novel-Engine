using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Localisation.Languages;

public class UIManager : MonoBehaviour {
	public static UIManager instance;

	private MainManager main;

	public TextBox textBox;

	private bool isDisplayingText = false;

	// ----------------------------------------- EASY --- ACCESS --- VARIABLES ----------------------------------------- \\
	private SentenceManager sentenceData => MainManager.sentenceData;

	private int currentChapter => main.currentChapter;
	private int currentSentence => main.currentSentence;
	private int currentCharacter => sentenceData.Chapters[currentChapter].Sentences[currentSentence].Name;

	private int selectedLanguagePack => sentenceData.SelectedLanguagePack;
	private List<LanguagePack> languagePacks => sentenceData.LanguagePacks;

	private string characterName => sentenceData.Chapters[currentChapter].Sentences[currentSentence].OverrideName ? languagePacks[selectedLanguagePack].chapters[currentChapter].sentences[currentSentence].DisplayName : languagePacks[selectedLanguagePack].characters[currentCharacter].CharacterName;
	private string text => languagePacks[selectedLanguagePack].chapters[currentChapter].sentences[currentSentence].Text;

	private TMP_Text nameText => textBox.NameText;
	private TMP_Text sentenceText => textBox.SentenceText;

	public bool typeText => textBox.TypeText;
	public float typeSpeed => textBox.TextSpeed;
	// ----------------------------------------- EASY --- ACCESS --- VARIABLES ----------------------------------------- \\

	private void Awake() {
		if (instance != null)
			Destroy(instance.gameObject);

		instance = this;
	}

	private void Start() {
		// ------------------------------------- PRELOAD ------------------------------------- \\
		if (MainManager.instance == null) {
			Util.SendError(0);
		}

		main = MainManager.instance;

		if(textBox == null) {
			Util.SendError(3);
		}
		// ------------------------------------- PRELOAD ------------------------------------- \\
	}

	public void RunUpdate() {
		nameText.text = characterName;
	}

	public async void DisplayText() {
		if(typeText) {
			if (sentenceText.maxVisibleCharacters != -1)
				sentenceText.maxVisibleCharacters = -1;

			sentenceText.maxVisibleCharacters = 0;

			sentenceText.text = text;

			while(sentenceText.maxVisibleCharacters != -1) {
				textBox.SentenceText.maxVisibleCharacters += 1;

				await Task.Delay(Mathf.RoundToInt(typeSpeed * 1000));

				if (sentenceText.maxVisibleCharacters == sentenceText.text.Length) {
					sentenceText.maxVisibleCharacters = -1;
				}
			}
		}
	}

	public void UpdateColor(int characterIndex) {
		Character character = sentenceData.Characters[currentCharacter];

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
}
