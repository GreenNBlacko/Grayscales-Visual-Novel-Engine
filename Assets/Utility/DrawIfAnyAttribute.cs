using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DrawIfAnyAttribute : PropertyAttribute {

	public string comparedPropertyName { get; private set; }
	public object[] comparedValueArray { get; private set; }
	public bool slider { get; private set; }

	public DrawIfAnyAttribute(string comparedPropertyName, object[] comparedValueArray, bool slider = false) {
		this.comparedPropertyName = comparedPropertyName;
		this.comparedValueArray = comparedValueArray;
		this.slider = slider;
	}
}
