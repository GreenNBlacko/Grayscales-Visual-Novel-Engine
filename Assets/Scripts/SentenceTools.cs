using Localisation.Languages;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SentenceTools {
	public static SentenceManager sentenceManager = ScriptableObject.CreateInstance<SentenceManager>();

	public static void AddChapter(string ChapterName, int NextChapter = -1) {

		Chapter chapter = new Chapter();

		chapter.Sentences = new List<Sentence>();
		chapter.Sentences.Add(new Sentence());

		sentenceManager.Chapters.Add(chapter);
	}

	public static void AddSentence(int ChapterName, int Name, bool OverrideName, string displayName, string Sentence, bool Choice = false, bool Voiced = false, string VoiceClip = "") {
		int senIndex = sentenceManager.Chapters[ChapterName].Sentences.Count;

		sentenceManager.Chapters[ChapterName].Sentences.Add(new Sentence());

		sentenceManager.Chapters[ChapterName].Sentences[senIndex].Name = Name;
		if (OverrideName) {
			sentenceManager.Chapters[ChapterName].Sentences[senIndex].OverrideName = OverrideName;
			GetSelectedLanguagePack().chapters[ChapterName].sentences[senIndex].DisplayName = displayName;
		}

		GetSelectedLanguagePack().chapters[ChapterName].sentences[senIndex].Text = Sentence;
		sentenceManager.Chapters[ChapterName].Sentences[senIndex].Choice = Choice;
		sentenceManager.Chapters[ChapterName].Sentences[senIndex].Voiced = Voiced;

		if (Voiced) {
			sentenceManager.Chapters[ChapterName].Sentences[senIndex].VoiceClip = AssetDatabase.LoadAssetAtPath<AudioClip>(VoiceClip);
		}
	}

	public static Color32 RGBToColor32(byte[] rgb, byte a = 255) {
		return new Color32(rgb[0], rgb[1], rgb[2], a);
	}

	public static Color32 HexToColor32(string hex) {
		if (hex.Contains("0x"))
			hex = hex.Replace("0x", "");
		if (hex.Contains("#"))
			hex = hex.Replace("#", "");

		byte a = 255;
		byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

		if (hex.Length == 8) {
			a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
		}
		return new Color32(r, g, b, a);
	}

	public static Color32 SetColorAlpha(Color32 color, byte alpha) {
		return SetColorAlpha(color.r, color.g, color.b, alpha);
	}

	public static Color32 SetColorAlpha(Color color, byte alpha) {
		byte r = (byte)(color.r * 255);
		byte g = (byte)(color.g * 255);
		byte b = (byte)(color.b * 255);

		return SetColorAlpha(r, g, b, alpha);
	}

	public static Color32 SetColorAlpha(byte r, byte g, byte b, byte a) {
		return new Color32(r, g, b, a);
	}

	public static void AddCharacter(int characterName, Color32 nameColor = default, Color32 textColor = default, Character.GradientType gradientType = Character.GradientType.Name, Color32 nameColorGradient = default, Color32 textColorGradient = default, Color32 colorGradient = default) {
		if (nameColor.a < 255)
			nameColor = new Color32(255, 255, 255, 255);

		if (textColor.a < 255)
			textColor = new Color32(255, 255, 255, 255);

		if (nameColorGradient.a < 255)
			nameColorGradient = new Color32(255, 255, 255, 255);

		if (textColorGradient.a < 255)
			textColorGradient = new Color32(255, 255, 255, 255);

		if (colorGradient.a < 255)
			colorGradient = new Color32(255, 255, 255, 255);

		Character character = new Character();

		character.CharacterName = characterName;
		character.NameColor = nameColor;
		character.TextColor = textColor;
		character.gradientType = gradientType;
		character.NameGradientColor = nameColorGradient;
		character.TextGradientColor = textColorGradient;
		character.GradientColor = colorGradient;

		sentenceManager.Characters.Add(character);
	}

	public static string[] GetChapterNames() {
		List<string> list = new List<string> { "None" };

		foreach (var chapter in GetSelectedLanguagePack().chapters) {
			list.Add(chapter.ChapterName);
		}

		return list.ToArray();
	}

	public static string[] GetCharacterNames() {
		string[] list = new string[0];
		foreach (var character in GetSelectedLanguagePack().characters) {
			Array.Resize(ref list, list.Length + 1);
			list[list.Length - 1] = character.CharacterName;
		}
		return list;
	}

	public static string[] GetBGList() {
		CGManager cgManager = GameObject.Find("ScriptManager").GetComponent<CGManager>();
		List<string> list = new List<string> { "None" };
		foreach (Artwork artwork in cgManager.BGList) {
			list.Add(artwork.Name);
		}
		return list.ToArray();
	}

	public static string[] GetCGList() {
		CGManager cgManager = GameObject.Find("ScriptManager").GetComponent<CGManager>();
		List<string> list = new List<string> { "None" };
		foreach (Artwork artwork in cgManager.CGList) {
			list.Add(artwork.Name);
		}
		return list.ToArray();
	}

	public static string[] GetCharacterVariants(int CharacterName) {
		List<string> list = new List<string>();
		foreach (Character character in sentenceManager.Characters) {
			if (character.CharacterName == CharacterName) {
				foreach (CharacterVariant variant in character.characterVariants) {
					list.Add(variant.VariantName);
				}
			}
		}
		return list.ToArray();
	}

	public static string[] GetCharacterStates(int CharacterName, string variantName) {
		List<string> list = new List<string>();
		foreach (CharacterVariant variant in sentenceManager.Characters[CharacterName].characterVariants) {
			if (variant.VariantName == variantName) {
				foreach (CharacterState state in variant.variantStates) {
					list.Add(state.StateName);
				}
			}
		}
		return list.ToArray();
	}

	public static Sprite GetStateSprite(string characterName, string spriteName) {
		Sprite sprite = Sprite.Create(new Texture2D(0, 0), new Rect(), Vector2.zero);

		if (PathExists(Application.dataPath + "/Sprites/Characters/" + characterName + "/" + spriteName)) {
			byte[] fileData = File.ReadAllBytes(Application.dataPath + "/Sprites/Characters/" + characterName + "/" + spriteName);
			Texture2D tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
			tex.LoadImage(fileData);

			tex.Apply();

			sprite = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), Vector2.zero);
			sprite.name = spriteName;
		} else {
			Debug.LogError("Path " + Application.dataPath + "/Sprites/Characters/" + characterName + "/" + spriteName + " doesn't exist!");
		}

		return sprite;
	}

	public static LanguagePack GetSelectedLanguagePack() {
		return sentenceManager.LanguagePacks[sentenceManager.SelectedLanguagePack];
	}

	public static bool PathExists(string path) {
		return File.Exists(path);
	}
}
