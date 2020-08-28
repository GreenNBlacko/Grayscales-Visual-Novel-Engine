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
		foreach(Character character in MainScript.scripts.CharacterInfoScript.Characters) {
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
		NameText.text = Name;
		SentenceText.text = sentence;
		Character = character;

		UpdateColor(character);
		if(AudioButton != null) {
			if (Voiced) {
				AudioButton.interactable = true;
			} else {
				AudioButton.interactable = false;
			}
		}

		this.transform.localScale = new Vector3(1,1,1);
	}

	public void Remove() {
		Destroy(this.gameObject);
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

		switch (character.gradientType) {
			case Character.GradientType.None: {
					NameText.colorGradient = new VertexGradient(nameColor);
					SentenceText.colorGradient = new VertexGradient(textColor);
					break;
				}
			case Character.GradientType.Name: {
					NameText.colorGradient = new VertexGradient(nameColor, nameColor, character.NameGradientColor, character.NameGradientColor);
					SentenceText.colorGradient = new VertexGradient(textColor, textColor, textColor, textColor);
					break;
				}
			case Character.GradientType.Text: {
					NameText.colorGradient = new VertexGradient(nameColor, nameColor, nameColor, nameColor);
					SentenceText.colorGradient = new VertexGradient(textColor, textColor, character.TextGradientColor, character.TextGradientColor);
					break;
				}
			case Character.GradientType.Both: {
					if (character.UseSeperateGradientColors) { 
						NameText.colorGradient = new VertexGradient(nameColor, nameColor, character.NameGradientColor, character.NameGradientColor);
						SentenceText.colorGradient = new VertexGradient(textColor, textColor, character.TextGradientColor, character.TextGradientColor);
					} else {
						NameText.colorGradient = new VertexGradient(nameColor, nameColor, character.GradientColor, character.GradientColor);
						SentenceText.colorGradient = new VertexGradient(textColor, textColor, character.GradientColor, character.GradientColor);
					}     
					break;
				}
		}
	}
}
