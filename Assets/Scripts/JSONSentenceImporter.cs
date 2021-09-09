using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JSONSentenceImporter : MonoBehaviour {
	[HideInInspector]
	public SentenceData sentenceData;

	public void LoadFromJSON() {
		string filePath = UnityEditor.EditorUtility.OpenFilePanel("Select file to load", "/Assets/Sentences/", "json");

		if (File.Exists(filePath)) {
			StreamReader reader = new StreamReader(filePath);
			string JsonData = "" + reader.ReadToEnd();

			Debug.Log(JsonData);

			sentenceData = JsonUtility.FromJson<SentenceData>(JsonData);

			reader.Close();
			#region SentenceImport

			SentenceManager sentenceManager = (SentenceManager)FindObjectOfType(typeof(SentenceManager));

			SentenceTools.sentenceManager = sentenceManager;

			foreach (chap ch in sentenceData.chapters) {
				SentenceTools.AddChapter(ch.ChapterName, ch.NextChapter);
				foreach (sent sen in ch.sentences) {
					SentenceTools.AddSentence(ch.ChapterName, sen.Name, sen.NameOverride, sen.DisplayName, sen.Text, sen.Choice, sen.choiceOptions, sen.Voiced, sen.VoiceClip);
				}
			}
			#endregion

			#region CharacterDataImport

			CharacterInfo characterInfo = (CharacterInfo)FindObjectOfType(typeof(CharacterInfo));

			SentenceTools.characterInfo = characterInfo;

			if (sentenceData.characterData == null) return;

			foreach (CharData charData in sentenceData.characterData) {
				Color32 NameColor = default;
				Color32 TextColor = default;
				Color32 NameColorGradient = default;
				Color32 TextColorGradient = default;
				Color32 ColorGradient = default;

				if (charData.nameColorRGB.Length != 0)
					NameColor = SentenceTools.RGBToColor32(charData.nameColorRGB);
				if (charData.nameColorHex != "")
					NameColor = SentenceTools.HexToColor32(charData.nameColorHex);

				if (charData.textColorRGB.Length != 0)
					TextColor = SentenceTools.RGBToColor32(charData.textColorRGB);
				if (charData.textColorHex != "")
					TextColor = SentenceTools.HexToColor32(charData.textColorHex);

				if (charData.nameColorGradientRGB.Length != 0)
					NameColorGradient = SentenceTools.RGBToColor32(charData.nameColorGradientRGB);
				if (charData.nameColorGradientHex != "")
					NameColorGradient = SentenceTools.HexToColor32(charData.nameColorGradientHex);

				if (charData.textColorGradientRGB.Length != 0)
					TextColorGradient = SentenceTools.RGBToColor32(charData.textColorGradientRGB);
				if (charData.textColorGradientHex != "")
					TextColorGradient = SentenceTools.HexToColor32(charData.textColorGradientHex);

				if (charData.colorGradientRGB.Length != 0)
					ColorGradient = SentenceTools.RGBToColor32(charData.colorGradientRGB);
				if (charData.colorGradientHex != "")
					ColorGradient = SentenceTools.HexToColor32(charData.colorGradientHex);

				SentenceTools.AddCharacter(charData.characterName, NameColor, TextColor, charData.gradientType, NameColorGradient, TextColorGradient, ColorGradient);
			}
			#endregion
		} else {
			SentenceData data = new SentenceData();

			data.chapters = new chap[1];
			data.chapters[0] = new chap();
			data.chapters[0].sentences = new sent[1];
			data.chapters[0].sentences[0] = new sent();
			data.chapters[0].sentences[0].choiceOptions = new Option[1];
			data.chapters[0].sentences[0].choiceOptions[0] = new Option();

			data.characterData = new CharData[1];
			data.characterData[0] = new CharData();

			string tmp = JsonUtility.ToJson(data, true);
			File.WriteAllText("Assets/Sentences/SentenceData.json", tmp);
		}
	}
	public void ExportFromJSON() {
		SentenceManager sentenceManager = FindObjectOfType<SentenceManager>();

		SentenceData data = new SentenceData();

		List<chap> chapters = new List<chap>();

		foreach(Chapter chapter in sentenceManager.Chapters) {
			chap Chap = new chap();

			Chap.ChapterName = chapter.ChapterName;
			Chap.NextChapter = chapter.NextChapter;

			List<sent> sentences = new List<sent>();

			foreach(Sentence sentence in chapter.Sentences) {
				sent Sent = new sent();

				Sent.Name = sentence.Name;
				Sent.NameOverride = sentence.OverrideName;
				Sent.DisplayName = sentence.DisplayName;
				Sent.Text = sentence.Text;
				Sent.Choice = sentence.Choice;
				Sent.choiceOptions = sentence.choiceOptions;
				Sent.Viewed = sentence.Viewed;
				Sent.Voiced = sentence.Voiced;
				Sent.VoiceClip = UnityEditor.AssetDatabase.GetAssetPath(sentence.VoiceClip);
				Sent.sentenceActions = sentence.onSentenceInit;

				sentences.Add(Sent);
			}

			Chap.sentences = sentences.ToArray();
			chapters.Add(Chap);
		}

		data.chapters = chapters.ToArray();

		string JsonData = JsonUtility.ToJson(data, true);
		File.WriteAllText("Assets/Sentences/SentenceDataV2.json", JsonData);
	}
}

[System.Serializable]
public class SentenceData {
	public chap[] chapters;
	public CharData[] characterData;
}

[System.Serializable]
public class chap {
	public string ChapterName;
	public int NextChapter = -1;
	public sent[] sentences;
}

[System.Serializable]
public class sent {
	public string Name;

	public bool NameOverride = false;

	public string DisplayName = "";

	public string Text;

	public bool Choice;

	public Option[] choiceOptions;

	public bool Viewed;

	public bool Voiced;

	public string VoiceClip;

	public OnSentenceInit[] sentenceActions;
}

[Serializable]
public class SentenceActionData {
	public OnSentenceInit.Actions ActionType;

	public bool FadeIn;
	public bool FadeOut;
	public bool Transition;

	public float FadeSpeed;
	public float TransitionSpeed;

	public int startingPosition;
	public Vector2 customStartingPosition;

	public Vector2 position;

	public string CharacterName;
	public string StateName;

	public string BGMName;

	public float Delay;
}

[Serializable]
public class CharData {
	public string characterName;

	public CharacterStateData[] characterStates;

	public bool useSeperateColors;

	public string colorHex;
	public byte[] colorRGB;
	public string nameColorHex;
	public byte[] nameColorRGB;
	public string textColorHex;
	public byte[] textColorRGB;

	public Character.GradientType gradientType = Character.GradientType.None;

	public bool useSeperateGradientColors;

	public string colorGradientHex;
	public byte[] colorGradientRGB;
	public string nameColorGradientHex;
	public byte[] nameColorGradientRGB;
	public string textColorGradientHex;
	public byte[] textColorGradientRGB;
}

[Serializable]
public class CharacterStateData {
	public string StateName;
	public int StateType;
	public string BaseLayerImagePath;
	public string ExpressionLayerImagePath;
}

[Serializable]
public class ArtworkData {
	public string ArtworkName;
	public int ArtworkType;
	public string ArtworkPath;
}

[Serializable]
public class BGMData {
	public string BGMName;
	public string BGMPath;
}

