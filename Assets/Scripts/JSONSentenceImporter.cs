using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using UnityEngine;

public class JSONSentenceImporter : MonoBehaviour {
	[HideInInspector]
	public SentenceData sentenceData;

	public void LoadFromJSON() {
		if (File.Exists("Assets/Sentences/SentenceData.json")) {
			string JsonData = "";
			StreamReader reader = new StreamReader("Assets/Sentences/SentenceData.json");
			JsonData = reader.ReadToEnd();
			sentenceData = JsonUtility.FromJson<SentenceData>(JsonData);
			#region SentenceImport

			SentenceManager sentenceManager = (SentenceManager)FindObjectOfType(typeof(SentenceManager));

			foreach (chap ch in sentenceData.chapters) {
				SentenceTools.AddChapter(sentenceManager, ch.ChapterName, ch.NextChapter);
				foreach (sent sen in ch.sentences) {
					SentenceTools.AddSentence(sentenceManager, ch.ChapterName, sen.Name, sen.Text, sen.artworkType, sen.BG_ID, sen.CG_ID, sen.Choice, sen.choiceOptions, sen.Voiced, sen.VoiceClip);
				}
			}
			#endregion

			#region CharacterDataImport

			CharacterInfo characterInfo = (CharacterInfo)FindObjectOfType(typeof(CharacterInfo));

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

				SentenceTools.AddCharacter(characterInfo, charData.characterName, NameColor, TextColor, charData.gradientType, NameColorGradient, TextColorGradient, ColorGradient);
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

			string tmp = JsonUtility.ToJson(data);
			File.WriteAllText("Assets/Sentences/SentenceData.json", tmp);
		}
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

	public string Text;

	[JsonConverter(typeof(StringEnumConverter))]
	public Sentence.ArtworkType artworkType = Sentence.ArtworkType.None;

	public int BG_ID;
	public int CG_ID;

	public bool Choice;

	public Option[] choiceOptions;

	public bool Viewed;

	public bool Voiced;

	public string VoiceClip;
}

[System.Serializable]
public class CharData {
	public string characterName;

	public bool useSeperateColors = false;

	public byte[] colorRGB = new byte[0];
	public string colorHex = "";

	public byte[] nameColorRGB = new byte[0];
	public string nameColorHex = "";

	public byte[] textColorRGB = new byte[0];
	public string textColorHex = "";

	[JsonConverter(typeof(StringEnumConverter))]
	public Character.GradientType gradientType = Character.GradientType.None;

	public bool useSeperateGradientColors = false;

	public byte[] nameColorGradientRGB = new byte[0];
	public string nameColorGradientHex = "";

	public byte[] textColorGradientRGB = new byte[0];
	public string textColorGradientHex = "";

	public byte[] colorGradientRGB = new byte[0];
	public string colorGradientHex = "";
}

