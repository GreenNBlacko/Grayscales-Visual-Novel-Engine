using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DrawIfAnyAttribute : PropertyAttribute {

	public string comparedPropertyName { get; private set; }
	public object[] comparedValueArray { get; private set; }

	public DrawIfAnyAttribute(string comparedPropertyName, object[] comparedValueArray) {
		this.comparedPropertyName = comparedPropertyName;
		this.comparedValueArray = comparedValueArray;
	}
}
