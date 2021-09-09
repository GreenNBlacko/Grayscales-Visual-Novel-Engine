using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LanguageName_LanguagePack", menuName = "Utility/Language Pack", order = 1)]
public class LanguagePack : ScriptableObject {
	public List<LanguagePackChapter> chapters = new List<LanguagePackChapter>();
	public List<LanguagePackCharacter> characters = new List<LanguagePackCharacter>();
}

[System.Serializable]
public class LanguagePackChapter {
	public string chapterName;

	public List<LanguagePackSentence> sentences = new List<LanguagePackSentence>();
}

[System.Serializable]
public class LanguagePackSentence {
	public string overrideName;
	public string sentence;

	public List<LanguagePackChoiceOption> choices = new List<LanguagePackChoiceOption>();
}

[System.Serializable]
public class LanguagePackChoiceOption {
	public string choiceName;
}

[System.Serializable]
public class LanguagePackCharacter {
	public string characterName;
}
