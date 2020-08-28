using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    public int CurrentChapterIndex;

    public DialogueManager dialogueManager;

    public void LoadChapter (int ChapterIndex, int backlogID = -1) {
        foreach(Transform tr in dialogueManager.buttonArrays.BacklogButtonsArray) {
            if(tr.GetComponent<BacklogListItem>().ChapterIndex > ChapterIndex) {tr.GetComponent<BacklogListItem>().Remove();}
        }
        if(CurrentChapterIndex > ChapterIndex) {
            if (backlogID == -1) {dialogueManager.BacklogID = backlogID = 0;}
            else {dialogueManager.BacklogID = backlogID;}
            foreach (Transform tr in dialogueManager.buttonArrays.BacklogButtonsArray) {
                if(tr.GetComponent<BacklogListItem>().ChapterIndex >= ChapterIndex) {
                    tr.GetComponent<BacklogListItem>().Remove();
                }
            }
        } else {
            if (backlogID == -1) {dialogueManager.BacklogID = backlogID = 0;}
            else {dialogueManager.BacklogID = backlogID;}
        }

            
        CurrentChapterIndex = ChapterIndex;
        foreach(Transform button in dialogueManager.buttonArrays.ChoiceButtonsArray) {
            button.GetComponent<ChoiceListItem>().Remove();
        }
        dialogueManager.gameObjects.ChoiceList.SetActive(false);
        dialogueManager.gameObjects.BacklogGameobject.SetActive(false);
        dialogueManager.choiceNext = false;
        dialogueManager.NextSentence();
    }

}
