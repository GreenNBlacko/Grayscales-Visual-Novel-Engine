using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BacklogListItem : MonoBehaviour
{
	public int ID;
	public int ChapterIndex;

	public string Name;
	public string sentence;

	public Button AudioButton;

	public AudioClip VoiceClip;

	public TextMeshProUGUI NameText;
	public TextMeshProUGUI SentenceText;

	private DialogueManager MainScript;
	private Character Character;

	void Start() {
		MainScript = (DialogueManager)FindObjectOfType(typeof(DialogueManager));
	}

	void Update() {
		if(MainScript == null) MainScript = (DialogueManager)FindObjectOfType(typeof(DialogueManager));

		foreach (Character character in MainScript.scripts.CharacterInfoScript.Characters) {
			if (Character.CharacterName == character.CharacterName)
				Character = character;
		}
		UpdateColor(Character);
	}

	public void Setup(int itemID, int chapterIndex, string itemName, string itemSentence, Character character, bool Voiced, AudioClip itemVoiceClip) {
		ID = itemID;
		ChapterIndex = chapterIndex;
		Name = itemName;
		sentence = itemSentence;
		VoiceClip = itemVoiceClip;
		this.gameObject.name = "" + ID;
		if (NameText != null) NameText.text = Name;
		if (SentenceText != null) SentenceText.text = sentence;
		Character = character;

		UpdateColor(character);
		if(AudioButton != null) {
			if (Voiced) {
				AudioButton.interactable = true;
			} else {
				AudioButton.interactable = false;
			}
		}

		transform.localScale = new Vector3(1,1,1);
	}

	public void Remove() {
		Destroy(gameObject);
	}

	public void Jumpto() {
		MainScript.JumpTo(ID, ChapterIndex);
	}

	public void ReplayVoice() {
		if(VoiceClip != null) {
			MainScript.PlayVoice(VoiceClip);
		}
	}

	public void UpdateColor(Character character) {
		Color32 nameColor = character.NameColor;
		Color32 textColor = character.TextColor;

		if (!character.UseSeperateColors) {
			nameColor = character.Color;
			textColor = character.Color;
		}

		Color32 nameGradientColor = character.NameGradientColor;
		Color32 textGradientColor = character.TextGradientColor;

		if (!character.UseSeperateColors) {
			nameGradientColor = character.GradientColor;
			textGradientColor = character.GradientColor;
		}

		switch (character.gradientType) {
			case Character.GradientType.None: {
					if (NameText != null) NameText.colorGradient = new VertexGradient(nameColor);
					if (SentenceText != null) SentenceText.colorGradient = new VertexGradient(textColor);
					break;
				}
			case Character.GradientType.Name: {
					if (NameText != null) NameText.colorGradient = new VertexGradient(nameColor, nameColor, character.NameGradientColor, character.NameGradientColor);
					if (SentenceText != null) SentenceText.colorGradient = new VertexGradient(textColor, textColor, textColor, textColor);
					break;
				}
			case Character.GradientType.Text: {
					if (NameText != null) NameText.colorGradient = new VertexGradient(nameColor, nameColor, nameColor, nameColor);
					if (SentenceText != null) SentenceText.colorGradient = new VertexGradient(textColor, textColor, character.TextGradientColor, character.TextGradientColor);
					break;
				}
			case Character.GradientType.Both: {
					if (NameText != null) NameText.colorGradient = new VertexGradient(nameColor, nameColor, nameGradientColor, nameGradientColor);
					if (SentenceText != null) SentenceText.colorGradient = new VertexGradient(textColor, textColor, textGradientColor, textGradientColor);    
					break;
				}
		}
	}
}
