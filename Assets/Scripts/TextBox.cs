using TMPro;
using UnityEngine;

public class TextBox : MonoBehaviour {
	public TMP_Text NameText;
	public TMP_Text SentenceText;

	[HideInInspector] [Tooltip("Text will progressively be displayed in the text box")] public bool TypeText = true;
	[HideInInspector] [Tooltip("The time betweeen displaying each letter")] public float TextSpeed = 0.1f;
}
