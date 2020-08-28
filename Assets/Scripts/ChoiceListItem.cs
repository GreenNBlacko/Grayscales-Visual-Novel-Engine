using UnityEngine;
using TMPro;

public class ChoiceListItem : MonoBehaviour
{

    public int ChoiceID;
    public string ChoiceText;

    private DialogueManager MainScript;
    private ChapterManager LevelLoader;

    public TextMeshProUGUI ButtonText;

    void Start() {
        MainScript = (DialogueManager)FindObjectOfType(typeof(DialogueManager));
        LevelLoader = (ChapterManager)FindObjectOfType(typeof(ChapterManager));
    }

    public void Setup (int ID, string Text) {
        ChoiceID = ID;
        ChoiceText = Text;

        this.gameObject.name = "" + ChoiceText;
        ButtonText.text = Text;

        this.transform.localScale = new Vector3(1,1,1);
    }

    public void OnButtonClick() {
        LevelLoader.LoadChapter(ChoiceID);
    }

    public void Remove() {
        Destroy(this.gameObject);
    }
}
