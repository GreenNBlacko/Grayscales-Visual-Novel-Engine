using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class SentenceTools {
	public static SentenceManager sentenceManager = ScriptableObject.CreateInstance<SentenceManager>();
	public static CharacterInfo characterInfo;

	public static void AddChapter(string ChapterName, int NextChapter = -1) {
		sentenceManager = AssetDatabase.LoadAssetAtPath<SentenceManager>("Assets/Sentences/Sentence Manager.asset");
		if (sentenceManager.Chapters.Length > 0) {
			foreach (Chapter chap in sentenceManager.Chapters) {
				if (chap.ChapterName == ChapterName) {
					EditChapter(ChapterName, ChapterName, NextChapter);
					return;
				}
			}
		}

		int i = sentenceManager.Chapters.Length;

		Array.Resize(ref sentenceManager.Chapters, i + 1);
		sentenceManager.Chapters[i] = new Chapter();
		sentenceManager.Chapters[i].ChapterName = ChapterName;
		sentenceManager.Chapters[i].NextChapter = NextChapter;
		sentenceManager.Chapters[i].Sentences = new List<Sentence>();
		sentenceManager.Chapters[i].Sentences.Add(new Sentence());
	}

	public static void EditChapter(string ChapterName, string ChapterNameNew = "", int NextChapter = -1) {
		int ChapterIndex = 0;
		foreach (Chapter chap in sentenceManager.Chapters) {
			if (chap.ChapterName == ChapterName) {
				ChapterIndex = Array.IndexOf(sentenceManager.Chapters, chap);
			}
		}
		if (ChapterNameNew != "") {
			sentenceManager.Chapters[ChapterIndex].ChapterName = ChapterNameNew;
		}
		sentenceManager.Chapters[ChapterIndex].NextChapter = NextChapter;
	}

	public static void AddSentence(string ChapterName, string Name, string Sentence, bool Choice = false, Option[] choiceOptions = null, bool Voiced = false, string VoiceClip = "") {
		AddSentence(ChapterName, Name, false, "", Sentence, Choice, choiceOptions, Voiced, VoiceClip);
	}

	public static void AddSentence(string ChapterName, string Name, bool OverrideName, string displayName, string Sentence, bool Choice = false, Option[] choiceOptions = null, bool Voiced = false, string VoiceClip = "") {
		int ChapterIndex = -1;
		foreach (Chapter chap in sentenceManager.Chapters) {
			if (chap.ChapterName == ChapterName) {
				ChapterIndex = Array.IndexOf(sentenceManager.Chapters, chap);
			}
		}
		if (ChapterIndex == -1) {
			Debug.LogError("Chapter '" + ChapterName + "' doesn't exist");
			return;
		}
		if (sentenceManager.OverrideSentenceValues) {
			foreach (Sentence sent in sentenceManager.Chapters[ChapterIndex].Sentences) {
				if (sent.Name == Name && sent.Text == Sentence) {
					EditSentence(ChapterName, Name, Sentence, Name, Sentence, Choice, choiceOptions);
					return;
				}
			}
		}

		int senIndex = sentenceManager.Chapters[ChapterIndex].Sentences.Count;

		sentenceManager.Chapters[ChapterIndex].Sentences.Add(new Sentence());

		sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].Name = Name;
		if (OverrideName) {
			sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].OverrideName = OverrideName;
			sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].DisplayName = displayName;
		}
		sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].Text = Sentence;
		sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].Choice = Choice;
		sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].choiceOptions = choiceOptions;
		sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].Viewed = false;
		sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].Voiced = Voiced;
		if (Voiced) {
			sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].VoiceClip = AssetDatabase.LoadAssetAtPath<AudioClip>(VoiceClip);
		}
	}

	public static void EditSentence(string ChapterName, string SentenceName, string SentenceText, string Name = "", string Text = "", bool Choice = false, Option[] choiceOptions = null, bool Voiced = false, string VoiceClip = "") {
		int ChapterIndex = -1;
		foreach (Chapter chap in sentenceManager.Chapters) {
			if (chap.ChapterName == ChapterName) {
				ChapterIndex = Array.IndexOf(sentenceManager.Chapters, chap);
			}
		}
		if (ChapterIndex == -1) {
			Debug.LogError("Chapter '" + ChapterName + "' doesn't exist");
			return;
		}
		int senIndex = -1;
		foreach (Sentence sent in sentenceManager.Chapters[ChapterIndex].Sentences) {
			if (sent.Name == SentenceName && sent.Text == SentenceText) {
				senIndex = sentenceManager.Chapters[ChapterIndex].Sentences.IndexOf(sent);
			}
		}
		if (senIndex == -1) {
			Debug.LogError("Sentence '" + SentenceText + "' doesn't exist");
			return;
		}
		if (Name != "")
			sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].Name = Name;
		if (Text != "")
			sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].Text = Text;
		sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].Choice = Choice;
		sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].choiceOptions = choiceOptions;
		sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].Viewed = false;
		sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].Voiced = Voiced;
		if (Voiced) {
			UriBuilder uri = new UriBuilder(Application.dataPath + "/Audio/" + VoiceClip);
			uri.Scheme = "file";

			UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri.ToString(), AudioType.OGGVORBIS);

			sentenceManager.Chapters[ChapterIndex].Sentences[senIndex].VoiceClip = DownloadHandlerAudioClip.GetContent(www);
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

	public static void AddCharacter(string characterName, Color32 nameColor = default, Color32 textColor = default, Character.GradientType gradientType = Character.GradientType.Name, Color32 nameColorGradient = default, Color32 textColorGradient = default, Color32 colorGradient = default) {
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

		int index = characterInfo.Characters.Length;
		Array.Resize(ref characterInfo.Characters, index + 1);
		characterInfo.Characters[index] = new Character();
		characterInfo.Characters[index].CharacterName = characterName;
		characterInfo.Characters[index].NameColor = nameColor;
		characterInfo.Characters[index].TextColor = textColor;
		characterInfo.Characters[index].gradientType = gradientType;
		characterInfo.Characters[index].NameGradientColor = nameColorGradient;
		characterInfo.Characters[index].TextGradientColor = textColorGradient;
	}

	public static void AddCharacterState(string characterName, string characterVariant, string stateName, CharacterState.StateType stateType = CharacterState.StateType.SingleLayer, string BaseImagePath = "") {
		AddCharacterState(characterName, characterVariant, stateName, stateType, BaseImagePath, "", false, Vector2.zero);
	}

	public static void AddCharacterState(string characterName, string characterVariant, string stateName, CharacterState.StateType stateType, string BaseImagePath, string stateImagePath, bool advanced, Vector2 offset) {
		foreach (Character character in characterInfo.Characters) {
			if (character.CharacterName == characterName) {
				foreach (CharacterVariant variant in character.characterVariants) {
					int index = variant.variantStates.Length;
					Array.Resize(ref variant.variantStates, index + 1);

					Sprite baseImage = GetStateSprite(characterName, BaseImagePath), expressionImage = default;

					if (stateImagePath != "") { expressionImage = GetStateSprite(characterName, stateImagePath); }

					variant.variantStates[index] = new CharacterState(stateName, stateType, baseImage, expressionImage, advanced, offset);
				}
			}
		}
	}

	public static string[] GetChapterNames() {
		sentenceManager = AssetDatabase.LoadAssetAtPath<SentenceManager>("Assets/Sentences/Sentence Manager.asset");

		List<string> list = new List<string> { "None" };

		foreach (Chapter chapter in sentenceManager.Chapters) {
			list.Add(chapter.ChapterName);
		}

		return list.ToArray();
	}

	public static string[] GetCharacterNames() {
		CharacterInfo characterInfo = GameObject.Find("ScriptManager").GetComponent<CharacterInfo>();
		string[] list = new string[0];
		foreach (Character character in characterInfo.Characters) {
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

	public static string[] GetCharacterVariants(string CharacterName) {
		CharacterInfo characterInfo = GameObject.Find("ScriptManager").GetComponent<CharacterInfo>();
		List<string> list = new List<string>();
		foreach (Character character in characterInfo.Characters) {
			if (character.CharacterName == CharacterName) {
				foreach (CharacterVariant variant in character.characterVariants) {
					list.Add(variant.VariantName);
				}
			}
		}
		return list.ToArray();
	}

	public static string[] GetCharacterStates(string CharacterName, string variantName) {
		CharacterInfo characterInfo = GameObject.Find("ScriptManager").GetComponent<CharacterInfo>();
		List<string> list = new List<string>();
		foreach (Character character in characterInfo.Characters) {
			if (character.CharacterName == CharacterName) {
				foreach (CharacterVariant variant in character.characterVariants) {
					if (variant.VariantName == variantName) {
						foreach (CharacterState state in variant.variantStates) {
							list.Add(state.StateName);
						}
					}
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

	public static bool PathExists(string path) {
		return File.Exists(path);
	}
}
