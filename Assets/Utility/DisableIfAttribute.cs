using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DisableIfAttribute : PropertyAttribute {
	public string comparedPropertyName { get; private set; }
	public object comparedValue { get; private set; }
	public string tmpValueName { get; set; }
	public string tmpBoolName { get; set; }

	public DisableIfAttribute(string comparedPropertyName, object comparedValue, string tmpValueName, string tmpBoolName) {
		this.comparedPropertyName = comparedPropertyName;
		this.comparedValue = comparedValue;
		this.tmpValueName = tmpValueName;
		this.tmpBoolName = tmpBoolName;
	}
}
