using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadSystem : MonoBehaviour {
	#region Fields
	public DialogueManager dialogueManager;

	public int sentenceID;
	public int chapterID;

	public Button loadButton;

	public Texture2D image;

	private SaveData quickSaveData;

	private SaveData[] SavedataList;

	public Camera mainCamera;
	#endregion

	void Start() {
		if (File.Exists(Application.persistentDataPath + "/QuickSave.sav")) { loadButton.interactable = true; } else { loadButton.interactable = false; }
		quickSaveData = new SaveData();
		SavedataList = new SaveData[0];
		QuickLoad();
		for (int i = 0; i < SavedataList.Length; i++) {
			Load(i);
		}
	}

	void Update() {

	}

	public void AddChoiceData(List<BacklogEntry> backlog, int index = -1) {
		if (index == -1)
			quickSaveData.Choices = backlog;
		else
			SavedataList[index].Choices = backlog;

		UnityEngine.Debug.Log("Choice ID: " + quickSaveData.Choices[0]);
	}

	public void QuickSaveStart() {
		StartCoroutine(readPixels());
	}

	public void SaveStart(int slot) {
		StartCoroutine(readPixels(slot));
	}

	public void QuickSave() {
		quickSaveData.SentenceID = dialogueManager.BacklogID - 1;
		quickSaveData.ChapterID = dialogueManager.scripts.ChapterManagerScript.CurrentChapterIndex;
		quickSaveData.characters = GetCharacters();

		foreach (CharacterSaveData character in quickSaveData.characters) {
			character.CharacterPosition[0] /= Screen.width;
			character.CharacterPosition[1] /= Screen.height;

			character.CharacterPosition[0] *= 2650;
			character.CharacterPosition[1] *= 1440;
		}

		AddChoiceData(dialogueManager.Returnbacklog());

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/QuickSave.sav");

		bf.Serialize(file, quickSaveData);

		file.Close();

		loadButton.interactable = true;
		UnityEngine.Debug.Log("Save succesful!");
	}

	public void QuickLoad() {
		if (File.Exists(Application.persistentDataPath + "/QuickSave.sav")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/QuickSave.sav", FileMode.Open);

			SaveData save = (SaveData)bf.Deserialize(file);

			file.Close();

			sentenceID = save.SentenceID;
			chapterID = save.ChapterID;

			quickSaveData = save;

			image = new Texture2D(Screen.width, Screen.height);
			image.LoadImage(save.ImageSave);
		}
	}

	public void Save(int slot) {
		if (slot + 1 >= SavedataList.Length) {
			Array.Resize(ref SavedataList, slot + 1);
			SavedataList[slot] = new SaveData();
		}
		SavedataList[slot].SentenceID = dialogueManager.BacklogID - 1;
		SavedataList[slot].ChapterID = dialogueManager.scripts.ChapterManagerScript.CurrentChapterIndex;
		SavedataList[slot].characters = GetCharacters();

		foreach(CharacterSaveData character in SavedataList[slot].characters) {
			character.CharacterPosition[0] /= Screen.width;
			character.CharacterPosition[1] /= Screen.height;

			character.CharacterPosition[0] *= 2650;
			character.CharacterPosition[1] *= 1440;
		}

		AddChoiceData(dialogueManager.Returnbacklog(), slot);

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/Save_" + slot + ".sav");

		bf.Serialize(file, SavedataList[slot]);

		file.Close();

		loadButton.interactable = true;
		UnityEngine.Debug.Log("Save succesful!");
	}

	public void Load(int slot) {
		if (File.Exists(Application.persistentDataPath + "/Save_" + slot + ".sav")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/Save_" + slot + ".sav", FileMode.Open);

			SaveData save = (SaveData)bf.Deserialize(file);

			file.Close();

			sentenceID = save.SentenceID;
			chapterID = save.ChapterID;

			SavedataList[slot] = save;

			image = new Texture2D(Screen.width, Screen.height);
			image.LoadImage(save.ImageSave);
		}
	}

	public void LoadSave(int slot) {
		ManageCharacterInfo(SavedataList[slot].characters);
		dialogueManager.QuickLoadChapter(SavedataList[slot]);
	}

	public void QuickLoadSave() {
		ManageCharacterInfo(quickSaveData.characters);
		dialogueManager.QuickLoadChapter(quickSaveData);
	}

	WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
	public IEnumerator readPixels(int slot = -1) {
		bool temp = dialogueManager.gameObjects.UI.activeInHierarchy;
		dialogueManager.HideUI();
		yield return frameEnd;

		RenderTexture imageIN = new RenderTexture(Screen.width, Screen.height, 24);
		imageIN.Create();

		mainCamera.targetTexture = imageIN;

		image = new Texture2D(Screen.width, Screen.height);
		image.ReadPixels(new Rect(0, 0, imageIN.width, imageIN.height), 0, 0);
		image.Apply();

		if (slot == -1) {
			quickSaveData.ImageSave = image.EncodeToPNG();
			QuickSave();
		} else {
			SavedataList[slot].ImageSave = image.EncodeToPNG();
			Save(slot);
		}

		mainCamera.targetTexture = null;

		if (temp)
			dialogueManager.ShowUI();
	}

	public void ManageCharacterInfo(List<CharacterSaveData> characters) {
		foreach(Character character in FindObjectOfType<CharacterInfo>().Characters) {
			if(character.CharacterOnScene) {
				FindObjectOfType<CharacterManager>().RemoveCharacterFromScene(character.CharacterName, character.CharacterPosition);
			}
		}

		foreach(CharacterSaveData character in characters) {
			if(character.CharacterOnScene) {
				FindObjectOfType<CharacterManager>().AddCharacterToScene(character.CharacterName, dialogueManager.GetCharacterState(character.CharacterName, character.stateName), new Vector2(character.CharacterPosition[0], character.CharacterPosition[1]));
			}
		}
	}

	public List<CharacterSaveData> GetCharacters() {
		List<CharacterSaveData> saveDatas = new List<CharacterSaveData>();
		List<Character> characters = FindObjectOfType<CharacterInfo>().Characters.ToList();

		foreach(Character character in characters) {
			saveDatas.Add(new CharacterSaveData() { CharacterName = character.CharacterName, CharacterOnScene = character.CharacterOnScene, CharacterPosition = new float[2] { character.CharacterPosition.x / character.CharacterScale.x, character.CharacterPosition.y / character.CharacterScale.y }, stateName = character.CurrentState.StateName});
		}

		return saveDatas;
	}
}

[System.Serializable]
public class SaveData {
	public int SentenceID;
	public int ChapterID;
	public byte[] ImageSave;
	public List<BacklogEntry> Choices = new List<BacklogEntry>();
	[SerializeField]
	public List<CharacterSaveData> characters = new List<CharacterSaveData>();
}

[System.Serializable]
public class BacklogEntry {
	public int sentenceID;
	public int chapterID;
}

[Serializable]
public class CharacterSaveData {
	public string CharacterName;
	public bool CharacterOnScene = false;
	public float[] CharacterPosition = new float[2];
	public string stateName;
}
