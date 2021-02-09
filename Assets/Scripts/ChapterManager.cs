using UnityEngine;

public class ChapterManager : MonoBehaviour {
	public int CurrentChapterIndex = 0;

	public DialogueManager dialogueManager;

	public void LoadChapter(int ChapterIndex, int backlogID = -1) {
		foreach (Transform tr in dialogueManager.buttonArrays.BacklogButtonsArray) {
			if (tr.GetComponent<BacklogListItem>().ChapterIndex > ChapterIndex) { tr.GetComponent<BacklogListItem>().Remove(); }
		}

		foreach (Transform tr in dialogueManager.buttonArrays.ChapterJumpArray) {
			if (tr.GetComponent<ChapterJumpItem>().ChapterIndex > ChapterIndex) { Destroy(tr.gameObject); Debug.Log(tr.GetComponent<ChapterJumpItem>().ChapterIndex); }
			if (tr.GetComponent<ChapterJumpItem>().ChapterIndex == ChapterIndex) { tr.GetComponent<ChapterJumpItem>().IsCurrentChapter = true; }
		}

		if (CurrentChapterIndex > ChapterIndex) {
			if (backlogID == -1) { dialogueManager.BacklogID = backlogID = 0; } else { dialogueManager.BacklogID = backlogID; }

			foreach (Transform tr in dialogueManager.buttonArrays.BacklogButtonsArray) {
				if (tr.GetComponent<BacklogListItem>().ChapterIndex >= ChapterIndex) {
					tr.GetComponent<BacklogListItem>().Remove();
				}
			}
		} else {
			if (backlogID == -1) { dialogueManager.BacklogID = backlogID = 0; } else { dialogueManager.BacklogID = backlogID; }
		}


		CurrentChapterIndex = ChapterIndex;

		foreach (Transform button in dialogueManager.buttonArrays.ChoiceButtonsArray) {
			button.GetComponent<ChoiceListItem>().Remove();
		}

		dialogueManager.gameObjects.ChoiceList.SetActive(false);
		dialogueManager.gameObjects.BacklogGameobject.SetActive(false);
		dialogueManager.choiceNext = false;
		dialogueManager.NextSentence();
	}

}
