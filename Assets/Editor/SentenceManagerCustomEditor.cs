using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#region CustomEditor
#if UNITY_EDITOR

[CustomEditor(typeof(SentenceManager))]
public class SentenceManagerCustomEditor : Editor {
	public override void OnInspectorGUI() {

		DrawDefaultInspector();

		SentenceManager myScript = (SentenceManager)target;

		myScript.OverrideSentenceValues = GUILayout.Toggle(myScript.OverrideSentenceValues, "Replace sentence if duplicate found?");
		myScript.UseJSONFile = GUILayout.Toggle(myScript.UseJSONFile, "Use JSON file?");

		if (myScript.UseJSONFile) {
			if (GUILayout.Button("Load sentences from a JSON file")) {
				myScript.ImportSentenceDataJSON();
			}
		}

		if (GUILayout.Button("Load sentences from using sentence tools")) {
			myScript.ImportSentenceDataCS();
		}

		if (GUILayout.Button("Save sentences to a JSON file")) {
			myScript.ExportSentenceDataJson();
		}
	}
}

#endif
#endregion