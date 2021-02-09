using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterJumpItem : MonoBehaviour {

	public string ChapterName;
	public int ChapterIndex;

	[HideInInspector]
	public bool IsCurrentChapter = false;

	public Button button;
	public TMP_Text text;

	private DialogueManager manager;

	void Start() {
		manager = FindObjectOfType<DialogueManager>();

		name = ChapterName;
		text.text = ChapterName;
	}

	void Update() {
		if (IsCurrentChapter) {
			if (!button.interactable) { return; }
			button.interactable = false;
		} else {
			if (button.interactable) { return; }
			button.interactable = true;
		}
	}

	public void LoadChapter() {
		manager.JumpTo(0, ChapterIndex);
	}
}
