using UnityEngine;

public class ArrayToListAttribute : PropertyAttribute {
	public string MethodName { get; private set; }
	public string AdditionalVariable { get; private set; }
	public string SecondAdditionalVariable { get; private set; }
	public string comparedPropertyName { get; private set; }
	public object[] comparedValueArray { get; private set; }

	public ArrayToListAttribute(string MethodName, string AdditionalVariable = default, string SecondAdditionalVariable = default, string comparedPropertyName = default, object[] comparedValueArray = default) {
		this.MethodName = MethodName;
		this.AdditionalVariable = AdditionalVariable;
		this.SecondAdditionalVariable = SecondAdditionalVariable;
		this.comparedPropertyName = comparedPropertyName;
		this.comparedValueArray = comparedValueArray;
	}
}
