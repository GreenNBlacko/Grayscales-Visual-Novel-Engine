using UnityEngine;

public class ArrayToListAttribute : PropertyAttribute {
	public string MethodName { get; private set; }
	public string AdditionalVariables { get; private set; }
	public string comparedPropertyName { get; private set; }
	public object[] comparedValueArray { get; private set; }

	public ArrayToListAttribute(string MethodName, string AdditionalVariables = default, string comparedPropertyName = default, object[] comparedValueArray = default) {
		this.MethodName = MethodName;
		this.AdditionalVariables = AdditionalVariables;
		this.comparedPropertyName = comparedPropertyName;
		this.comparedValueArray = comparedValueArray;
	}
}
