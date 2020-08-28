using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

public class SaveLoadSystem : MonoBehaviour
{
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

	void Start()
	{
		if (File.Exists(Application.persistentDataPath + "/QuickSave.sav")) { loadButton.interactable = true; }
		else { loadButton.interactable = false; }
		quickSaveData = new SaveData();
		SavedataList = new SaveData[0];
		for (int i = 0; i < SavedataList.Length; i++)
		{
			Load(i);
		}
	}

	void Update()
	{

	}

	public void AddChoiceData(List<BacklogEntry> backlog)
	{
		quickSaveData.Choices = backlog;

		UnityEngine.Debug.Log("Choice ID: " + quickSaveData.Choices[0]);
	}

	public void QuickSaveStart()
	{
		StartCoroutine(readPixels());
	}

	public void QuickSave()
	{
		quickSaveData.SentenceID = dialogueManager.BacklogID - 1;
		quickSaveData.ChapterID = dialogueManager.scripts.ChapterManagerScript.CurrentChapterIndex;

		AddChoiceData(dialogueManager.Returnbacklog());

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/QuickSave.sav");

		bf.Serialize(file, quickSaveData);

		file.Close();

		loadButton.interactable = true;
		UnityEngine.Debug.Log("Save succesful!");
	}

	public void QuickLoad()
	{
		if (File.Exists(Application.persistentDataPath + "/QuickSave.sav"))
		{
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

	public void Save()
	{
		int slot = 0;
		if (slot + 1 >= SavedataList.Length)
		{
			Array.Resize(ref SavedataList, slot + 1);
			SavedataList[slot] = new SaveData();
		}
		quickSaveData.SentenceID = dialogueManager.BacklogID - 1;
		quickSaveData.ChapterID = dialogueManager.scripts.ChapterManagerScript.CurrentChapterIndex;

		AddChoiceData(dialogueManager.Returnbacklog());

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/Save_" + slot + ".sav");

		bf.Serialize(file, SavedataList[slot]);

		file.Close();

		loadButton.interactable = true;
		UnityEngine.Debug.Log("Save succesful!");
	}

	public void Load(int slot)
	{
		if (File.Exists(Application.persistentDataPath + "/Save_" + slot + ".sav"))
		{
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

	public void LoadSave()
	{
		int slot = 0;
		dialogueManager.QuickLoadChapter(SavedataList[slot]);
	}

	public void QuickLoadSave() {
		dialogueManager.QuickLoadChapter(quickSaveData);
	}

	WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
	public IEnumerator readPixels()
	{
		bool temp = dialogueManager.gameObjects.UI.activeInHierarchy;
		dialogueManager.HideUI();
		yield return frameEnd;

		RenderTexture imageIN = new RenderTexture(Screen.width, Screen.height, 24);
		imageIN.Create();

		mainCamera.targetTexture = imageIN;

		image = new Texture2D(Screen.width, Screen.height);
		image.ReadPixels(new Rect(0, 0, imageIN.width, imageIN.height), 0, 0);
		image.Apply();

		quickSaveData.ImageSave = image.EncodeToPNG();

		mainCamera.targetTexture = null;

		if(temp)
			dialogueManager.ShowUI();

		QuickSave();
	}
}

[System.Serializable]
public class SaveData
{
	public int SentenceID;
	public int ChapterID;
	public byte[] ImageSave;
	public List<BacklogEntry> Choices = new List<BacklogEntry>();
}

[System.Serializable]
public class BacklogEntry
{
	public int sentenceID;
	public int chapterID;
}
