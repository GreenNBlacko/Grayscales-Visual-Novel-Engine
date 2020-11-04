using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class CharacterManager : MonoBehaviour {
	public GameObject CharacterArray = null;
	private Dictionary<string, CharacterData> CharactersInScene = new Dictionary<string, CharacterData>();
	private CharacterInfo characterInfo = null;
	private Vector2 resolution = new Vector2(Screen.width, Screen.height);
	private Dictionary<string, Vector2> TempLerpData = new Dictionary<string, Vector2>();
	private Dictionary<string, Coroutine> enumerators = new Dictionary<string, Coroutine>();

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

		character.AddComponent<Image>();
		character.GetComponent<Image>().sprite = state.StateImage;
		character.GetComponent<Image>().SetNativeSize();
		Destroy(character.GetComponent<Image>());

		GameObject baseLayer = new GameObject();

		baseLayer.transform.SetParent(character.transform);

		baseLayer.AddComponent<RectTransform>();
		baseLayer.AddComponent<Image>();

		baseLayer.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
		baseLayer.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
		baseLayer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);


		character.name = Name;
		switch (state.stateType) {
			case CharacterState.StateType.SingleLayer: {
					baseLayer.GetComponent<Image>().sprite = state.StateImage;
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
					break;
				}
		}

		characterData.characterGO = character;
		characterData.BaseLayer = baseLayer.GetComponent<Image>();

		if (FadeIn)
			StartCoroutine(FadeInCharacter(state, characterData, FadeSpeed));

		character.transform.SetParent(CharacterArray.transform);

		if (enterScene) {
			enumerators.Add(Name, StartCoroutine(Lerp(startingPosition, position, speed, characterData)));
		} else {
			character.transform.GetComponent<RectTransform>().position = position;
		}

		character.GetComponent<RectTransform>().localScale = new Vector3(1 * GetCharacterFromState(state).CharacterScale.x, 1 * GetCharacterFromState(state).CharacterScale.y, 1);

		CharactersInScene.Add(Name, characterData);
	}

	public Character GetCharacterFromState(CharacterState state) {
		Character temp = new Character();

		foreach (Character character in characterInfo.Characters) {
			foreach (CharacterState characterState in character.CharacterStates) {
				if (characterState == state) {
					temp = character;
				}
			}
		}

		return temp;
	}

	public void MoveCharacter(string Name, Vector2 position, float speed) {
		int index = 0;

		if (!CharactersInScene.TryGetValue(Name, out CharacterData character)) {
			Debug.LogError("Character is not in the scene");
			return;
		}

		foreach (Character temp in characterInfo.Characters) {
			if (temp.CharacterName == Name)
				index = Array.IndexOf(characterInfo.Characters, temp);
		}

		Vector2 currentPosition = characterInfo.Characters[index].CharacterPosition;


		if (enumerators.TryGetValue(Name, out Coroutine enumerator) && enumerator != null) {
			StopCoroutine(enumerator);

			position = GetPosition(position);

			TempLerpData.TryGetValue(Name, out Vector2 tempPos);

			enumerators.Remove(Name);

			TempLerpData.Remove(Name);

			enumerators.Add(Name, StartCoroutine(Lerp(tempPos, position, speed, character)));

			characterInfo.Characters[index].CharacterPosition = position;
			return;
		}

		currentPosition = GetPosition(currentPosition);
		position = GetPosition(position);

		characterInfo.Characters[index].CharacterPosition = position;

		enumerators.Add(Name, StartCoroutine(Lerp(currentPosition, position, speed, character)));
	}

	public void ChangeCharacterState(string Name, CharacterState state, bool transition = false, float speed = 0.5f) {
		CharactersInScene.TryGetValue(Name, out CharacterData character);

		if (character == null) {
			Debug.LogError("Character is not in the scene");
			return;
		}

		if (transition) {
			StartCoroutine(TransitionCharacterState(state, character, speed));
		} else {
			if (character.ExpressionLayer == null) {
				character.BaseLayer.sprite = state.StateImage;

				character.characterGO.AddComponent<Image>();
				character.characterGO.GetComponent<Image>().sprite = state.StateImage;
				character.characterGO.GetComponent<Image>().SetNativeSize();
				Destroy(character.characterGO.GetComponent<Image>());
			} else {
				character.BaseLayer.sprite = state.BaseImage;
				character.ExpressionLayer.sprite = state.StateImage;

				character.characterGO.AddComponent<Image>();
				character.characterGO.GetComponent<Image>().sprite = state.StateImage;
				character.characterGO.GetComponent<Image>().SetNativeSize();
				Destroy(character.characterGO.GetComponent<Image>());
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

		characterInfo.Characters[index].CharacterOnScene = false;
		characterInfo.Characters[index].CharacterPosition = position;
		characterInfo.Characters[index].CurrentState = null;

		if (exitScene)
			StartCoroutine(Lerp(currentPosition, position, speed, character));

		if (FadeOut)
			StartCoroutine(FadeOutCharacter(characterInfo.Characters[index].CurrentState, character, FadeSpeed, true));
		else
			StartCoroutine(FadeOutCharacter(characterInfo.Characters[index].CurrentState, character, 0f, true));

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

		TempLerpData.Add(character.characterGO.name, new Vector2(Mathf.Lerp(startingPos.x, Pos.x, timeElapsed / lerpDuration), Mathf.Lerp(startingPos.y, Pos.y, timeElapsed / lerpDuration)));

		while (timeElapsed < lerpDuration) {

			character.characterGO.transform.GetComponent<RectTransform>().position = new Vector2(Mathf.Lerp(startingPos.x, Pos.x, timeElapsed / lerpDuration), Mathf.Lerp(startingPos.y, Pos.y, timeElapsed / lerpDuration));

			if (TempLerpData.TryGetValue(character.characterGO.name, out Vector2 tempPos))
				TempLerpData[character.characterGO.name] = new Vector2(Mathf.Lerp(startingPos.x, Pos.x, timeElapsed / lerpDuration), Mathf.Lerp(startingPos.y, Pos.y, timeElapsed / lerpDuration));
			

			timeElapsed += Time.deltaTime;

			yield return null;
		}
		enumerators.Remove(character.characterGO.name);
		TempLerpData.Remove(character.characterGO.name);
	}

	IEnumerator FadeInCharacter(CharacterState state, CharacterData character, float lerpDuration = 0.5f) {
		float timeElapsed = 0;

		while (timeElapsed < lerpDuration) {
			switch (state.stateType) {
				case CharacterState.StateType.SingleLayer: {
						character.BaseLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed / lerpDuration));
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
		switch (state.stateType) {
			case CharacterState.StateType.SingleLayer: {
					float timeElapsed = 0;
					GameObject baselayer = character.BaseLayer.gameObject;

					GameObject baseLayer = Instantiate(baselayer, character.characterGO.transform);

					baseLayer.transform.SetParent(character.characterGO.transform);

					baseLayer.GetComponent<RectTransform>().sizeDelta = baselayer.GetComponent<RectTransform>().sizeDelta;
					baseLayer.GetComponent<Image>().sprite = state.StateImage;

					character.BaseLayer = baseLayer.GetComponent<Image>();

					character.BaseLayer.color = new Color32(255, 255, 255, 0);

					while (timeElapsed < lerpDuration) {

						baselayer.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));

						character.BaseLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed * 1.5f / lerpDuration));

						timeElapsed += Time.deltaTime;

						yield return null;
					}

					break;
				}
			case CharacterState.StateType.MultiLayer: {
					if (character.BaseLayer.sprite != state.BaseImage && character.ExpressionLayer.sprite != state.StateImage) {
						float timeElapsed = 0;
						GameObject baselayer = character.BaseLayer.gameObject;

						GameObject baseLayer = Instantiate(baselayer, character.characterGO.transform);

						baseLayer.transform.SetParent(character.characterGO.transform);

						baseLayer.GetComponent<RectTransform>().sizeDelta = baselayer.GetComponent<RectTransform>().sizeDelta;
						baseLayer.GetComponent<Image>().sprite = state.BaseImage;

						character.BaseLayer = baseLayer.GetComponent<Image>();

						character.BaseLayer.color = new Color32(255, 255, 255, 0);

						GameObject expressionlayer = character.ExpressionLayer.gameObject;

						GameObject expressionLayer = Instantiate(expressionlayer, character.characterGO.transform);

						expressionLayer.transform.SetParent(character.characterGO.transform);

						expressionLayer.GetComponent<RectTransform>().sizeDelta = expressionLayer.GetComponent<RectTransform>().sizeDelta;
						expressionLayer.GetComponent<Image>().sprite = state.StateImage;

						character.ExpressionLayer = expressionLayer.GetComponent<Image>();

						character.ExpressionLayer.color = new Color32(255, 255, 255, 0);


						while (timeElapsed < lerpDuration) {

							baselayer.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));

							character.BaseLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed * 1.5f / lerpDuration));

							expressionlayer.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));

							character.ExpressionLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed * 1.5f / lerpDuration));

							timeElapsed += Time.deltaTime;

							yield return null;
						}

						Destroy(baselayer);
						Destroy(expressionlayer);
						break;
					}
					if (character.BaseLayer.sprite != state.BaseImage) {
						float timeElapsed = 0;
						GameObject baselayer = character.BaseLayer.gameObject;

						GameObject baseLayer = Instantiate(baselayer, character.characterGO.transform);

						baseLayer.transform.SetParent(character.characterGO.transform);

						baseLayer.GetComponent<RectTransform>().sizeDelta = baselayer.GetComponent<RectTransform>().sizeDelta;
						baseLayer.GetComponent<Image>().sprite = state.BaseImage;

						character.BaseLayer = baseLayer.GetComponent<Image>();

						character.BaseLayer.color = new Color32(255, 255, 255, 0);

						while (timeElapsed < lerpDuration) {

							baselayer.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));

							character.BaseLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed * 1.5f / lerpDuration));

							timeElapsed += Time.deltaTime;

							yield return null;
						}

						Destroy(baselayer);
					}
					if (character.ExpressionLayer.sprite != state.StateImage) {
						float timeElapsed = 0;
						GameObject expressionlayer = character.ExpressionLayer.gameObject;

						GameObject expressionLayer = Instantiate(expressionlayer, character.characterGO.transform);

						expressionLayer.transform.SetParent(character.characterGO.transform);

						expressionLayer.GetComponent<RectTransform>().sizeDelta = expressionLayer.GetComponent<RectTransform>().sizeDelta;
						expressionLayer.GetComponent<Image>().sprite = state.StateImage;

						character.ExpressionLayer = expressionLayer.GetComponent<Image>();

						character.ExpressionLayer.color = new Color32(255, 255, 255, 0);

						while (timeElapsed < lerpDuration) {

							expressionlayer.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));

							character.ExpressionLayer.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed * 1.5f / lerpDuration));

							timeElapsed += Time.deltaTime;

							yield return null;
						}
						Destroy(expressionlayer);
					}
					break;
				}
		}
	}

	IEnumerator FadeOutCharacter(CharacterState state, CharacterData character, float lerpDuration = 0.5f, bool destroy = false) {
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
