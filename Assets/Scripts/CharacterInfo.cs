using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterInfo : MonoBehaviour
{
    public Sprite DefaultCharacterPic;

    public Character[] Characters;
}

[Serializable]
public class Character {
    public string CharacterName;

    [Tooltip("character's emotions and appearance")]
    public CharacterState[] CharacterStates;

    public Sprite CharacterPic;

    [Tooltip("Use seperate colors for name and text?")]
    public bool UseSeperateColors = false;

    [DrawIf("UseSeperateColors", false)]
    public Color32 Color = new Color32(255, 255, 255, 255);

    [Tooltip("Character's name color")]
    [ConditionalHide("UseSeperateColors", true)]
    public Color32 NameColor = new Color32(255, 255, 255, 255);

    [Tooltip("Text color")]
    [ConditionalHide("UseSeperateColors", true)]
    public Color32 TextColor = new Color32(255, 255, 255, 255);

    [Tooltip("Use gradient for text?")]
    public GradientType gradientType = GradientType.None;

    [DisableIf("gradientType", GradientType.Both,"tmpValue", "tmpBool")]
    public bool UseSeperateGradientColors = false;

    [HideInInspector]
    public bool tmpBool;
    [HideInInspector]
    public bool tmpValue;

    [Tooltip("Name text gradient color")]
    [DrawIf("gradientType", GradientType.Name, GradientType.Both, "UseSeperateGradientColors", true)]
    public Color32 NameGradientColor = new Color32(255, 255, 255, 255);

    [Tooltip("Sentence text gradient color")]
    [DrawIf("gradientType", GradientType.Text, GradientType.Both, "UseSeperateGradientColors", true)]
    public Color TextGradientColor = new Color(255, 255, 255);

    [Tooltip("Name and sentence text gradient color")]
    [DrawIf("UseSeperateGradientColors", false)]
    public Color32 GradientColor = new Color32(255, 255, 255, 255);

    [HideInInspector]
    public bool CharacterOnScene = false;
    [HideInInspector]
    public Vector2 CharacterPosition = new Vector2(0,0);
    [HideInInspector]
    public CharacterState CurrentState = new CharacterState();

    public enum GradientType { None, Name, Text, Both };
}

[Serializable]
public class CharacterState {
    public string StateName;
    public StateType stateType;
    [DrawIf("stateType", StateType.MultiLayer)]
    public Sprite BaseImage;
    public Sprite StateImage;

    public enum StateType { SingleLayer, MultiLayer };
}
