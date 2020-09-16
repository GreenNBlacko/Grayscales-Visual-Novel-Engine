using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour {
	public GameObject CharacterArray = null;
	private Dictionary<string, CharacterData> CharactersInScene = new Dictionary<string, CharacterData>();
	private CharacterInfo characterInfo = null;
	private Vector2 resolution = new Vector2(Screen.width, Screen.height);
	private Dictionary<string, Vector2> TempLerpData = new Dictionary<string, Vector2>();

	void Start() {
		characterInfo = (CharacterInfo)FindObjectOfType(typeof(CharacterInfo));

		resolution.x = Screen.width;
		resolution.y = Screen.height;
	}

	private void LateUpdate() {

		if (resolution.x != Screen.width || resolution.y != Screen.height) {
			foreach (CharacterData character in CharactersInScene.Values) {
				UpadteCharacterPosition(character.characterGO);
			}
		}
	}

	public void AddCharacterToScene(string Name, CharacterState state, Vector2 position, bool enterScene = false, Vector2 startingPosition = default, float speed = 0f, bool FadeIn = false, float FadeSpeed = 0.5f) {
		if (CharacterArray == null) {
			Debug.LogError("No Character array gameObject found.");
			return;
		}

		CharactersInScene.TryGetValue(Name, out CharacterData tempchar);

		if (tempchar != null) {
			Debug.LogError("Character is already in the scene scene.");
			return;
		}

		int index = 0;

		foreach (Character temp in characterInfo.Characters) {
			if (temp.CharacterName == Name)
				index = Array.IndexOf(characterInfo.Characters, temp);
		}

		position = GetPosition(position);

		characterInfo.Characters[index].CharacterOnScene = true;
		characterInfo.Characters[index].CharacterPosition = position;
		characterInfo.Characters[index].CurrentState = state;

		CharacterData characterData = new CharacterData();

		GameObject character = new GameObject();

		character.AddComponent<RectTransform>();
		character.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
		character.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
		character.GetComponent<RectTransform>().pivot = new Vector2(0, 0);

		GameObject baseLayer = new GameObject();

		baseLayer.transform.SetParent(character.transform);

		baseLayer.AddComponent<RectTransform>();
		baseLayer.AddComponent<Image>();

		baseLayer.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
		baseLayer.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
		baseLayer.GetComponent<RectTransform>().sizeDelta = new Vector2(0,0);


		character.name = Name;
		switch (state.stateType) {
			case CharacterState.StateType.SingleLayer: {
					baseLayer.GetComponent<Image>().sprite = state.StateImage;
					character.GetComponent<RectTransform>().sizeDelta = new Vector2(state.StateImage.rect.width * state.StateImage.spriteAtlasTextureScale, state.StateImage.rect.height * state.StateImage.spriteAtlasTextureScale);
					break;
				}
			case CharacterState.StateType.MultiLayer: {
					GameObject expressionLayer = new GameObject();

					expressionLayer.transform.SetParent(character.transform);

					expressionLayer.AddComponent<RectTransform>();
					expressionLayer.AddComponent<Image>();

					baseLayer.GetComponent<Image>().sprite = state.BaseImage;

					expressionLayer.GetComponent<Image>().sprite = state.StateImage;

					expressionLayer.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
					expressionLayer.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
					expressionLayer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

					characterData.ExpressionLayer = expressionLayer.GetComponent<Image>();

					character.GetComponent<RectTransform>().sizeDelta = new Vector2(state.BaseImage.texture.width, state.BaseImage.texture.height);
					break;
				}
		}

		characterData.characterGO = character;
		characterData.BaseLayer = baseLayer.GetComponent<Image>();

		if (FadeIn)
			StartCoroutine(FadeInCharacter(state, characterData, FadeSpeed));

		character.transform.SetParent(CharacterArray.transform);

		if (enterScene) {
			StartCoroutine(Lerp(startingPosition, position, speed, characterData));
		} else {
			character.transform.GetComponent<RectTransform>().position = position;
		}

		CharactersInScene.Add(Name, characterData);
	}

	public void MoveCharacter(string Name, Vector2 position, float speed) {
		int index = 0;

		CharactersInScene.TryGetValue(Name, out CharacterData character);

		if (character == null) {
			Debug.LogError("Character is not in the scene");
			return;
		}

		foreach (Character temp in characterInfo.Characters) {
			if (temp.CharacterName == Name)
				index = Array.IndexOf(characterInfo.Characters, temp);
		}

		Vector2 currentPosition = characterInfo.Characters[index].CharacterPosition;

		TempLerpData.TryGetValue(Name, out Vector2 tempPos);

		if(tempPos != null) {
			StopCoroutine(Lerp(tempPos, currentPosition, speed, character));
			TempLerpData.Remove(Name);

			position = GetPosition(position);

			StartCoroutine(Lerp(character.characterGO.transform.position, GetPosition(position), speed, character));

			characterInfo.Characters[index].CharacterPosition = position;
			TempLerpData.Add(Name, character.characterGO.transform.position);
			return;
		}

		currentPosition = GetPosition(currentPosition);
		position = GetPosition(position);

		characterInfo.Characters[index].CharacterPosition = position;

		StartCoroutine(Lerp(currentPosition, position, speed, character));
		TempLerpData.Add(Name, currentPosition);
		
	}

	public void ChangeCharacterState(string Name, CharacterState state, bool transition = false, float speed = 0.5f) {
		CharactersInScene.TryGetValue(Name, out CharacterData character);

		if (character == null) {
			Debug.LogError("Character is not in the scene");
			return;
		}

		if(transition) {
			StartCoroutine(TransitionCharacterState(state, character, speed));
		} else {
			if(character.ExpressionLayer == null) {
				character.BaseLayer.sprite = state.StateImage;
			} else {
				character.BaseLayer.sprite = state.BaseImage;
				character.ExpressionLayer.sprite = state.StateImage;
			}
		}
	}

	public void RemoveCharacterFromScene(string Name, Vector2 position, bool exitScene = false, float speed = 0f, bool FadeOut = false, float FadeSpeed = 0.5f) {
		int index = 0;

		CharactersInScene.TryGetValue(Name, out CharacterData character);

		if (character == null) {
			Debug.LogError("Character is not in the scene");
			return;
		}

		foreach (Character temp in characterInfo.Characters) {
			if (temp.CharacterName == Name)
				index = Array.IndexOf(characterInfo.Characters, temp);
		}

		Vector2 currentPosition = characterInfo.Characters[index].CharacterPosition;

		currentPosition = GetPosition(currentPosition);
		position = GetPosition(position);

		if(FadeOut) 
			StartCoroutine(FadeOutCharacter(characterInfo.Characters[index].CurrentState, character, FadeSpeed, true));

		characterInfo.Characters[index].CharacterOnScene = false;
		characterInfo.Characters[index].CharacterPosition = position;
		characterInfo.Characters[index].CurrentState = null;

		if(exitScene)
			StartCoroutine(Lerp(currentPosition, position, speed, character));

		CharactersInScene.Remove(Name);
	}

	public void UpadteCharacterPosition(GameObject _character) {
		foreach (Character character in characterInfo.Characters) {
			if (character.CharacterName == _character.name) {
				_character.transform.GetComponent<RectTransform>().position = GetPosition(character.CharacterPosition);
			}
		}
		resolution.x = Screen.width;
		resolution.y = Screen.height;
	}

	public Vector2 GetPosition(Vector2 position, bool OverrideValues = false) {
		if (OverrideValues) {
			position.x /= resolution.x;
			position.y /= resolution.y;
		} else {
			position.x /= 2650;
			position.y /= 1440;
		}

		position.x *= Screen.width;
		position.y *= Screen.height;

		return position;
	}

	IEnumerator Lerp(Vector2 startingPos, Vector2 Pos, float lerpDuration, CharacterData character) {
		float timeElapsed = 0;

		while (timeElapsed < lerpDuration) {
			character.characterGO.transform.GetComponent<RectTransform>().position = new Vector2(Mathf.Lerp(startingPos.x, Pos.x, timeElapsed / lerpDuration), Mathf.Lerp(startingPos.y, Pos.y, timeElapsed / lerpDuration));

			timeElapsed += Time.deltaTime;

			yield return null;
		}
		TempLerpData.Remove(character.characterGO.name);
	}

	IEnumerator FadeInCharacter(CharacterState state, CharacterData character, float lerpDuration = 0.5f) {
		float timeElapsed = 0;

		while (timeElapsed < lerpDuration) {
			switch (state.stateType) {
				case CharacterState.StateType.SingleLayer: {
						character.BaseLayer.color = new Color32(255, 255, 255, (byte) Mathf.Lerp(0, 255, timeElapsed / lerpDuration));
						break;
					}
				case CharacterState.StateType.MultiLayer: {
						character.BaseLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed / lerpDuration));
						character.ExpressionLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed / lerpDuration));
						break;
					}
			}

			timeElapsed += Time.deltaTime;

			yield return null;
		}
		switch (state.stateType) {
			case CharacterState.StateType.SingleLayer: {
					character.BaseLayer.color = new Color32(255, 255, 255, 255);
					break;
				}
			case CharacterState.StateType.MultiLayer: {
					character.BaseLayer.color = new Color32(255, 255, 255, 255);
					character.ExpressionLayer.color = new Color32(255, 255, 255, 255);
					break;
				}
		}
	}

	IEnumerator TransitionCharacterState(CharacterState state, CharacterData character, float lerpDuration = 0.5f) {
		float timeElapsed = 0;
		float timeElapsed2 = 0;

		GameObject baselayer = character.BaseLayer.gameObject;
		GameObject expressionlayer = null;

		if(state.stateType == CharacterState.StateType.MultiLayer) {
			expressionlayer = character.ExpressionLayer.gameObject;
		}

		if(state.BaseImage != character.BaseLayer.sprite || state.StateImage != character.BaseLayer.sprite) {
			GameObject baseLayer = Instantiate(baselayer, character.characterGO.transform);

			baseLayer.transform.SetParent(character.characterGO.transform);

			baseLayer.GetComponent<RectTransform>().sizeDelta = baselayer.GetComponent<RectTransform>().sizeDelta;

			character.BaseLayer = baseLayer.GetComponent<Image>();

			switch (state.stateType) {
				case CharacterState.StateType.SingleLayer: {
						baseLayer.GetComponent<Image>().sprite = state.StateImage;
						break;
					}
				case CharacterState.StateType.MultiLayer: {
						GameObject expressionLayer = Instantiate(expressionlayer, character.characterGO.transform);

						expressionLayer.transform.SetParent(character.characterGO.transform);

						expressionLayer.AddComponent<RectTransform>();
						expressionLayer.AddComponent<Image>();

						expressionLayer.GetComponent<Image>().sprite = state.StateImage;

						expressionLayer.GetComponent<RectTransform>().sizeDelta = expressionlayer.GetComponent<RectTransform>().sizeDelta;

						character.ExpressionLayer = expressionLayer.GetComponent<Image>();

						baseLayer.GetComponent<Image>().sprite = state.BaseImage;
						break;
					}
			}
		}

		while (timeElapsed < lerpDuration && timeElapsed2 < lerpDuration) {
			switch (state.stateType) {
				case CharacterState.StateType.SingleLayer: {
						baselayer.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));
						break;
					}
				case CharacterState.StateType.MultiLayer: {
						baselayer.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));
						expressionlayer.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));
						break;
					}
			}

			timeElapsed += Time.deltaTime;

			if(timeElapsed / lerpDuration <= 0.5f) {
				while (timeElapsed2 < lerpDuration) {
					switch (state.stateType) {
						case CharacterState.StateType.SingleLayer: {
								character.BaseLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed2 / lerpDuration));
								break;
							}
						case CharacterState.StateType.MultiLayer: {
								character.BaseLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed2 / lerpDuration));
								character.ExpressionLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed2 / lerpDuration));
								break;
							}
					}

					timeElapsed2 += Time.deltaTime;

					yield return null;
				}
			}

			yield return null;
		}

		Destroy(baselayer);

		switch(state.stateType) {
			case CharacterState.StateType.MultiLayer: {
					Destroy(expressionlayer);
					break;
				}
		}
	}

	IEnumerator FadeOutCharacter(CharacterState state, CharacterData character, float lerpDuration = 0.5f,  bool destroy = false) {
		float timeElapsed = 0;

		while (timeElapsed < lerpDuration) {
			switch (state.stateType) {
				case CharacterState.StateType.SingleLayer: {
						character.BaseLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));
						break;
					}
				case CharacterState.StateType.MultiLayer: {
						character.BaseLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));
						character.ExpressionLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));
						break;
					}
			}

			timeElapsed += Time.deltaTime;

			yield return null;
		}
		switch (state.stateType) {
			case CharacterState.StateType.SingleLayer: {
					character.BaseLayer.color = new Color32(255, 255, 255, 0);
					break;
				}
			case CharacterState.StateType.MultiLayer: {
					character.BaseLayer.color = new Color32(255, 255, 255, 0);
					character.ExpressionLayer.color = new Color32(255, 255, 255, 0);
					break;
				}
		}

		if (destroy) {
			Destroy(character.characterGO);
		}
	}
}

[Serializable]
public class CharacterData {
	public GameObject characterGO;
	public Image BaseLayer;
	public Image ExpressionLayer;
}
