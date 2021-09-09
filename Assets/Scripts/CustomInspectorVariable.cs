using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomInspectorVariable {
	public string variableName;

	public VariableType variableType;
	public VariableDisplayType displayType;

	public CustomVariableDisplay customVariableDisplay;

	public RangeModifierOptions ModifierOptions;
	public Vector2 Bounds;

	public string PopupOptions;

	public CustomInspectorVariables ChildClassVariables;

	public float offset;

	public float witdh;
	public float height;

	public List<CustomInspectorVariableDependency> variableDependencies = new List<CustomInspectorVariableDependency>();
}

[System.Serializable]
public class CustomInspectorVariableDependency {
	public string dependencyName;
	public DependencyType dependencyType;

	public string comparedValue;

	public DependencyVerification dependencyVerification;
	public DependencyVerificationModifier verificationModifier;
}

public enum VariableType { Int, String, Bool, Enum, Vector, Float, Class, Array };
public enum VariableDisplayType { Default, Custom };
public enum CustomVariableDisplay { IntField, IntRange, IntPopup };
public enum RangeModifierOptions { MinMaxField, Slider }
public enum DependencyType { Int, Bool, Enum, String };
public enum DependencyVerification { Equals, EqualsOpposite, Contains, ContainsOpposite, MoreThan, LessThan };
public enum DependencyVerificationModifier { AND, NAND, OR, NOR, XOR, XNOR };