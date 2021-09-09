using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Script_CustomInspectorVariables", menuName = "Utility/Custom Inspector Variables", order = 1)]
public class CustomInspectorVariables : ScriptableObject {
	public List<CustomInspectorVariable> inspectorVariables = new List<CustomInspectorVariable>();
}

public class RenameAttribute : PropertyAttribute {
	public string NewName { get; set; }
	public string[] comparedPropertyName { get; set; }
	public object[] comparedValue { get; set; }
	public int checkType { get; set; }
	public bool hideVariable { get; set; }

	public RenameAttribute(string name, bool HideVariable = false) {
		NewName = name;
		comparedPropertyName = new string[1] { "" };
		comparedValue = null;
		hideVariable = HideVariable;
	}

	public RenameAttribute(string name, string comparedProperty, object ComparedValue, bool HideVariable = false) {
		NewName = name;
		comparedPropertyName = new string[1] { comparedProperty };
		comparedValue = new object[1] { ComparedValue };
		checkType = 0;
		hideVariable = HideVariable;
	}

	public RenameAttribute(string name, string[] comparedProperty, object[] ComparedValue, int CheckType, bool HideVariable = false) {
		NewName = name;
		comparedPropertyName = comparedProperty;
		comparedValue = ComparedValue;
		checkType = CheckType;
		hideVariable = HideVariable;
	}
}
