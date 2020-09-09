using UnityEngine;
using System;

/// <summary>
/// Draws the field/property ONLY if the compared property compared by the comparison type with the value of comparedValue returns true.
/// Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DrawIfAttribute : PropertyAttribute
{
	#region Fields

	public string comparedPropertyName { get; private set; }
	public object comparedValue { get; private set; }
	public object secondComparedValue { get; private set; }
	public string secondComparedPropertyName { get; private set; }
	public object thirdComparedValue { get; private set; }
	public bool slider { get; private set; }
	public DisablingType disablingType { get; private set; }

	/// <summary>
	/// Types of comperisons.
	/// </summary>
	public enum DisablingType
	{
		ReadOnly = 2,
		DontDraw = 3
	}

	#endregion

	/// <summary>
	/// Only draws the field only if a condition is met. Supports enum and bools.
	/// </summary>
	/// <param name="comparedPropertyName">The name of the property that is being compared (case sensitive).</param>
	/// <param name="comparedValue">The value the property is being compared to.</param>
	/// <param name="disablingType">The type of disabling that should happen if the condition is NOT met. Defaulted to DisablingType.DontDraw.</param>
	public DrawIfAttribute(string comparedPropertyName, object comparedValue, bool slider, DisablingType disablingType = DisablingType.DontDraw) {
		this.comparedPropertyName = comparedPropertyName;
		this.comparedValue = comparedValue;
		this.slider = slider;
		this.disablingType = disablingType;
	}

	public DrawIfAttribute(string comparedPropertyName, object comparedValue, string secondComparedPropertyName, object secondComparedValue) {
		this.comparedPropertyName = comparedPropertyName;
		this.comparedValue = comparedValue;
		this.secondComparedValue = secondComparedValue;
		this.secondComparedPropertyName = secondComparedPropertyName;
	}

	public DrawIfAttribute(string comparedPropertyName, object comparedValue, object secondComparedValue = null, string secondComparedPropertyName = null, object thirdComparedValue = null, bool slider = false, DisablingType disablingType = DisablingType.DontDraw)
	{
		this.comparedPropertyName = comparedPropertyName;
		this.comparedValue = comparedValue;
		this.secondComparedValue = secondComparedValue;
		this.secondComparedPropertyName = secondComparedPropertyName;
		this.thirdComparedValue = thirdComparedValue;
		this.slider = slider;
		this.disablingType = disablingType;
	}
}