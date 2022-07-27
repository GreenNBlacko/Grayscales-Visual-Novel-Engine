/*#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Option))]
public class OptionPropertyDrawer : PropertyDrawer {
	private CustomInspectorVariables InspectorVariables = ScriptableObject.CreateInstance<CustomInspectorVariables>();
	
	const string InspectorVariablesPath = "Assets/Editor/InspectorVariables/Option_InspectorVariables.asset";
	
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		InspectorVariables = CustomInspectorUtility.GetVariables(InspectorVariablesPath);

		EditorGUI.BeginProperty(position, label, property);

		Rect labelPosition = new Rect { x = position.x, y = position.y, width = position.width, height = 16 };
		
		property.isExpanded = EditorGUI.Foldout(labelPosition, property.isExpanded, label);
		
		if(property.isExpanded) {
			EditorGUI.indentLevel++;
			
			CustomInspectorUtility.ShowVariables(position, property, InspectorVariables);
			
			EditorGUI.indentLevel--;
		}

		EditorGUI.EndProperty();
	}
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return 18 + CustomInspectorUtility.GetPropertyHeight(property, InspectorVariables);
	}
}
#endif*/