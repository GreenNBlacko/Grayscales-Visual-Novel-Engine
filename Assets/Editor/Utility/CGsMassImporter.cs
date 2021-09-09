#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ArtworkMassImporter : EditorWindow {

	public CGManager cgManager;

	public int selectedIndex;

	public List<Sprite> Artworks;

	public Vector2 scrollPos;

	[MenuItem("Utility/Artwork Mass Importer")]
	public static void ShowWindow() {
		GetWindow(typeof(ArtworkMassImporter), false, "Artwork Mass Importer");
	}

	public void OnGUI() {
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		if(cgManager == null) {
			cgManager = FindObjectOfType<CGManager>();
		}

		selectedIndex = EditorGUILayout.Popup("Type of artwork", selectedIndex, new string[2] { "BG", "CG" });

		ScriptableObject target = this;
		SerializedObject so = new SerializedObject(target);
		SerializedProperty stringsProperty = so.FindProperty("Artworks");

		EditorGUILayout.PropertyField(stringsProperty, true);
		so.ApplyModifiedProperties();

		if(GUILayout.Button("Import Artwork")) {
			List<Artwork> artworks = new List<Artwork>();

			if(selectedIndex == 0) {
				artworks.AddRange(cgManager.BGList);
			} else {
				artworks.AddRange(cgManager.CGList);
			}

			foreach(Sprite sprite in Artworks) {
				artworks.Add(new Artwork { Name = sprite.texture.name, artworkImage = sprite });
			}

			if(selectedIndex == 0) {
				cgManager.BGList = artworks.ToArray();
			} else {
				cgManager.CGList = artworks.ToArray();
			}

			Close();
		}

		EditorGUILayout.EndScrollView();
	}
}

#endif