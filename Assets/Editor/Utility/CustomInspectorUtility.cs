#if UNITY_EDITOR
using B83.LogicExpressionParser;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CustomInspectorUtility {
	public static void ShowVariables(Rect position, SerializedProperty property, CustomInspectorVariables inspectorVariables) {
		Rect contentPosition = new Rect(position);

		int i = 0;

		foreach (CustomInspectorVariable variable in inspectorVariables.inspectorVariables) {
			if (CheckDependencies(property, variable)) {
				contentPosition = new Rect(contentPosition.x, contentPosition.y + (i > 0 ? inspectorVariables.inspectorVariables[i-1].height : 16f) + variable.offset, (variable.witdh <= -1f) ? contentPosition.width : variable.witdh, variable.height);

				//if(variable.variableName == "startingPosition")Debug.Log("Name: " + variable.variableName + " Show: " + CheckDependencies(property, variable));

				if (variable.displayType == VariableDisplayType.Default)
					EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative(variable.variableName), true);
				else {
					switch (variable.variableType) {
						case VariableType.Int: {
								switch (variable.customVariableDisplay) {
									case CustomVariableDisplay.IntField: {
											property.FindPropertyRelative(variable.variableName).intValue = EditorGUI.IntField(contentPosition, property.FindPropertyRelative(variable.variableName).displayName, property.FindPropertyRelative(variable.variableName).intValue);
											break;
										}
									case CustomVariableDisplay.IntRange: {
											if (variable.ModifierOptions == RangeModifierOptions.MinMaxField) {
												property.FindPropertyRelative(variable.variableName).intValue = Mathf.Clamp(EditorGUI.IntField(contentPosition, property.FindPropertyRelative(variable.variableName).displayName, property.FindPropertyRelative(variable.variableName).intValue), Mathf.RoundToInt(variable.Bounds.x), Mathf.RoundToInt(variable.Bounds.y));
											} else {
												property.FindPropertyRelative(variable.variableName).intValue = EditorGUI.IntSlider(contentPosition, property.FindPropertyRelative(variable.variableName).displayName, property.FindPropertyRelative(variable.variableName).intValue, Mathf.RoundToInt(variable.Bounds.x), Mathf.RoundToInt(variable.Bounds.y));
											}
											break;
										}
									case CustomVariableDisplay.IntPopup: {
											List<int> items = new List<int>();

											foreach (string s in variable.PopupOptions.Split(',')) {
												items.Add(int.Parse(s));
											}

											property.FindPropertyRelative(variable.variableName).intValue = EditorGUI.IntPopup(contentPosition, property.FindPropertyRelative(variable.variableName).displayName, property.FindPropertyRelative(variable.variableName).intValue, variable.PopupOptions.Split(','), items.ToArray());
											break;
										}
								}
								break;
							}
					}
				}
			}
		}
		i++;
	}

	public static string GetInspectorVariablesPath(string fileName) {
		return "Assets/Editor/InspectorVariables/" + fileName + "_InspectorVariables.asset";
	}

	public static bool CheckDependencies(SerializedProperty property, CustomInspectorVariable variable) {
		if (variable.variableDependencies.Count > 0) {
			string dependencyCheck = "";

			

			for (int i = 0; i < variable.variableDependencies.Count; i++) {
				CustomInspectorVariableDependency variableDependency = variable.variableDependencies[i];
				var dependency = property.FindPropertyRelative(variableDependency.dependencyName);

				if(i > 0) {
					if(variable.variableDependencies[i - 1].verificationModifier == DependencyVerificationModifier.NAND || variable.variableDependencies[i - 1].verificationModifier == DependencyVerificationModifier.NOR || variable.variableDependencies[i - 1].verificationModifier == DependencyVerificationModifier.XNOR) {
						dependencyCheck += "!";
					}
				}

				switch (variableDependency.dependencyVerification) {
					case DependencyVerification.Equals: {
							switch (variableDependency.dependencyType) {
								case DependencyType.Int: {
										dependencyCheck += dependency.intValue + " == " + int.Parse(variableDependency.comparedValue);
										break;
									}
								case DependencyType.Bool: {
										dependencyCheck += (dependency.boolValue ? 1 : 0) + " == " + (bool.Parse(variableDependency.comparedValue) ? 1 : 0);
										break;
									}
								case DependencyType.Enum: {
										dependencyCheck += dependency.enumValueIndex + " == " + int.Parse(variableDependency.comparedValue);
										break;
									}
								case DependencyType.String: {
										dependencyCheck += dependency.stringValue + " == " + variableDependency.comparedValue;
										break;
									}
							}
							break;
						}
					case DependencyVerification.EqualsOpposite: {
							switch (variableDependency.dependencyType) {
								case DependencyType.Int: {
										dependencyCheck += dependency.intValue + " != " + int.Parse(variableDependency.comparedValue);
										break;
									}
								case DependencyType.Bool: {
										dependencyCheck += (dependency.boolValue ? 1 : 0) + " != " + (bool.Parse(variableDependency.comparedValue) ? 1 : 0);
										break;
									}
								case DependencyType.Enum: {
										dependencyCheck += dependency.enumValueIndex + " != " + int.Parse(variableDependency.comparedValue);
										break;
									}
								case DependencyType.String: {
										dependencyCheck += dependency.stringValue + " != " + variableDependency.comparedValue;
										break;
									}
							}
							break;
						}
					case DependencyVerification.Contains: {
							switch (variableDependency.dependencyType) {
								case DependencyType.Int: {
										dependencyCheck += (dependency.intValue.ToString().Contains(variableDependency.comparedValue) ? 1 : 0) + " == 1 ";
										break;
									}
								case DependencyType.Bool: {
										dependencyCheck += (dependency.boolValue ? 1 : 0) + " == " + (bool.Parse(variableDependency.comparedValue) ? 1 : 0);
										break;
									}
								case DependencyType.Enum: {
										dependencyCheck += (dependency.enumValueIndex.ToString().Contains(variableDependency.comparedValue) ? 1 : 0) + " == 1 ";
										break;
									}
								case DependencyType.String: {
										dependencyCheck += (dependency.stringValue.Contains(variableDependency.comparedValue) ? 1 : 0) + " == 1 ";
										break;
									}
							}
							break;
						}
					case DependencyVerification.ContainsOpposite: {
							switch (variableDependency.dependencyType) {
								case DependencyType.Int: {
										dependencyCheck += "" + (!dependency.intValue.ToString().Contains(variableDependency.comparedValue) ? 1 : 0) + " == 1 ";
										break;
									}
								case DependencyType.Bool: {
										dependencyCheck += "" + (!dependency.boolValue ? 1 : 0) + " == " + (!bool.Parse(variableDependency.comparedValue) ? 1 : 0);
										break;
									}
								case DependencyType.Enum: {
										dependencyCheck += (!dependency.enumValueIndex.ToString().Contains(variableDependency.comparedValue) ? 1 : 0) + " == 1 ";
										break;
									}
								case DependencyType.String: {
										dependencyCheck += (!dependency.stringValue.Contains(variableDependency.comparedValue) ? 1 : 0) + " == 1 ";
										break;
									}
							}
							break;
						}
					case DependencyVerification.MoreThan: {
							switch (variableDependency.dependencyType) {
								case DependencyType.Int: {
										dependencyCheck += dependency.intValue + " > " + int.Parse(variableDependency.comparedValue);
										break;
									}
								case DependencyType.Bool: {
										dependencyCheck += dependency.boolValue + " > " + (bool.Parse(variableDependency.comparedValue) ? 1 : 0);
										break;
									}
								case DependencyType.Enum: {
										dependencyCheck += dependency.enumValueIndex + " > " + int.Parse(variableDependency.comparedValue);
										break;
									}
								case DependencyType.String: {
										dependencyCheck += (dependency.stringValue.Contains(variableDependency.comparedValue) ? 1 : 0);
										break;
									}
							}
							break;
						}
					case DependencyVerification.LessThan: {
							switch (variableDependency.dependencyType) {
								case DependencyType.Int: {
										dependencyCheck += dependency.intValue + " < " + int.Parse(variableDependency.comparedValue);
										break;
									}
								case DependencyType.Bool: {
										dependencyCheck += dependency.boolValue + " < " + (bool.Parse(variableDependency.comparedValue) ? 1 : 0);
										break;
									}
								case DependencyType.Enum: {
										dependencyCheck += dependency.enumValueIndex + " < " + int.Parse(variableDependency.comparedValue);
										break;
									}
								case DependencyType.String: {
										dependencyCheck += (!dependency.stringValue.Contains(variableDependency.comparedValue) ? 1 : 0);
										break;
									}
							}
							break;
						}
				}

				if (i >= variable.variableDependencies.Count - 1) break;

				switch (variableDependency.verificationModifier) {
					case DependencyVerificationModifier.AND: {
							dependencyCheck += " && ";
							break;
						}
					case DependencyVerificationModifier.NAND: {
							dependencyCheck += " && !";
							break;
						}
					case DependencyVerificationModifier.OR: {
							dependencyCheck += " || ";
							break;
						}
					case DependencyVerificationModifier.NOR: {
							dependencyCheck += " || !";
							break;
						}
					case DependencyVerificationModifier.XOR: {
							dependencyCheck += " xor ";
							break;
						}
					case DependencyVerificationModifier.XNOR: {
							dependencyCheck += " xor !"; 
							break;
						}
				}
			}

			//if (dependencyCheck.Contains("0 == 0")) Debug.Log(dependencyCheck);

			Parser parser = new Parser();

			LogicExpression exp = parser.Parse(dependencyCheck);

			return exp.GetResult();
		} else {
			return true;
		}
	}

	public static CustomInspectorVariables CreateVariables(string Path, string fileName) {
		string AssemblyToken = GetAssemblyName(fileName);

		CustomInspectorVariables inspectorVariables = ScriptableObject.CreateInstance<CustomInspectorVariables>();

		AssetDatabase.CreateAsset(inspectorVariables, Path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		inspectorVariables = AssetDatabase.LoadAssetAtPath<CustomInspectorVariables>(Path);

		List<FieldInfo> variables = new List<FieldInfo>();
		variables.AddRange(System.Type.GetType(AssemblyToken).GetFields());

		foreach (FieldInfo variable in variables) {
			CustomInspectorVariable inspectorVariable = new CustomInspectorVariable();

			inspectorVariable.variableName = variable.Name;

			if (variable.FieldType.BaseType.Name.Contains("Enum")) {
				inspectorVariable.variableType = VariableType.Enum;
			}

			if (variable.FieldType.IsClass) {
				inspectorVariable.variableType = VariableType.Class;
			}

			if (variable.FieldType.Name.Contains("[]") || variable.FieldType.Name.Contains("List")) {
				inspectorVariable.variableType = VariableType.Array;
			}

			switch (variable.FieldType.Name) {
				case "Int32": {
						inspectorVariable.variableType = VariableType.Int;
						break;
					}
				case "String": {
						inspectorVariable.variableType = VariableType.String;
						break;
					}
				case "Boolean": {
						inspectorVariable.variableType = VariableType.Bool;
						break;
					}
				case "Vector2": {
						inspectorVariable.variableType = VariableType.Vector;
						break;
					}
				case "Vector3": {
						inspectorVariable.variableType = VariableType.Vector;
						break;
					}
				case "Single": {
						inspectorVariable.variableType = VariableType.Float;
						break;
					}
			}

			if(inspectorVariable.variableType == VariableType.Class || inspectorVariable.variableType == VariableType.Array) {
				inspectorVariable.ChildClassVariables = CreateVariables(GetInspectorVariablesPath(variable.FieldType.Name), variable.FieldType.Name);
			}

			inspectorVariable.offset = 2;

			inspectorVariable.witdh = -1;
			inspectorVariable.height = 16;

			inspectorVariables.inspectorVariables.Add(inspectorVariable);

		}

		return inspectorVariables;
	}

	public static CustomInspectorVariables GetVariables(string Path) {
		if (!File.Exists(Path)) {
			string fileName = Path.Replace("Assets/Editor/InspectorVariables/", "").Replace("_InspectorVariables.asset", "");

			return CreateVariables(Path, fileName);
		}

		CustomInspectorVariables inspectorVariables;

		inspectorVariables = AssetDatabase.LoadAssetAtPath<CustomInspectorVariables>(Path);

		return inspectorVariables;
	}

	public static string GetAssemblyName(string fileName, bool editor = false) {
		if(editor) return fileName + ", Assembly-CSharp-Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
		return fileName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	}

	public static float GetPropertyHeight(SerializedProperty property, CustomInspectorVariables inspectorVariables) {
		float propertyHeight = 0;

		if (!property.isExpanded) { return 0; }

		foreach (CustomInspectorVariable inspectorVariable in inspectorVariables.inspectorVariables) {
			if (CheckDependencies(property, inspectorVariable)) {
				switch (inspectorVariable.variableType) {
					case VariableType.Int: {
							propertyHeight += inspectorVariable.height + inspectorVariable.offset;
							break;
						}
					default: {
							propertyHeight += inspectorVariable.height + inspectorVariable.offset;
							break;
						}
				}
			}
		}

		return propertyHeight;
	}
}

#endif