using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour {
	public GameObject CharacterArray = null;
	public List<GameObject> CharacterPrefabs;
	private Dictionary<string, GameObject> CharactersInScene = new Dictionary<string, GameObject>();
	private CharacterInfo characterInfo = null;
	private Vector2 resolution = new Vector2(Screen.width, Screen.height);

	void Start() {
		characterInfo = (CharacterInfo)FindObjectOfType(typeof(CharacterInfo));

		resolution.x = Screen.width;
		resolution.y = Screen.height;
	}

	private void LateUpdate() {

		if (resolution.x != Screen.width || resolution.y != Screen.height) {
			foreach (GameObject character in CharactersInScene.Values) {
				UpadteCharacterPosition(character);
			}
		}
	}

	public void AddCharacterToScene(string Name, CharacterState state, Vector2 position, bool enterScene = false, Vector2 startingPosition = default, float speed = 0f, bool FadeIn = false, float FadeSpeed = 0.5f) {
		if (CharacterArray == null) {
			Debug.LogError("No Character array gameObject found.");
			return;
		}

		CharactersInScene.TryGetValue(Name, out GameObject tempchar);

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

		foreach (GameObject prefab in CharacterPrefabs) {
			if (prefab.name == Name) {
				GameObject character = (GameObject)Instantiate(prefab, transform.position, transform.rotation);
				character.name = Name;
				switch (state.stateType) {
					case CharacterState.StateType.SingleLayer: {
							character.transform.GetChild(0).GetComponent<Image>().sprite = state.StateImage;
							break;
						}
					case CharacterState.StateType.MultiLayer: {
							character.transform.GetChild(0).GetComponent<Image>().sprite = state.BaseImage;
							character.transform.GetChild(1).GetComponent<Image>().sprite = state.StateImage;
							break;
						}
				}

				if(FadeIn)
					StartCoroutine(FadeInCharacter(state, character, FadeSpeed));

				character.transform.SetParent(CharacterArray.transform);

				if (enterScene) {
					StartCoroutine(Lerp(startingPosition, position, speed, FadeSpeed,character));
				} else {
					character.transform.GetComponent<RectTransform>().position = position;
				}

				CharactersInScene.Add(Name, character);
			}
		}
	}

	public void MoveCharacter(string Name, Vector2 position, float speed) {
		int index = 0;

		CharactersInScene.TryGetValue(Name, out GameObject character);

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

		characterInfo.Characters[index].CharacterPosition = position;

		StartCoroutine(Lerp(currentPosition, position, speed, speed, character));
	}

	public void ChangeCharacterState() {

	}

	public void RemoveCharacterFromScene(string Name, Vector2 position, bool exitScene = false, float speed = 0f, bool FadeOut = false, float FadeSpeed = 0.5f) {
		int index = 0;

		CharactersInScene.TryGetValue(Name, out GameObject character);

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
			StartCoroutine(Lerp(currentPosition, position, speed, FadeSpeed, character));

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

	IEnumerator Lerp(Vector2 startingPos, Vector2 Pos, float lerpDuration, float fadeSpeed, GameObject character) {
		float timeElapsed = 0;

		while (timeElapsed < lerpDuration) {
			character.transform.GetComponent<RectTransform>().position = new Vector2(Mathf.Lerp(startingPos.x, Pos.x, timeElapsed / lerpDuration), Mathf.Lerp(startingPos.y, Pos.y, timeElapsed / lerpDuration));

			timeElapsed += Time.deltaTime;

			yield return null;
		}
	}

	IEnumerator FadeInCharacter(CharacterState state, GameObject character, float lerpDuration = 0.5f) {
		float timeElapsed = 0;

		while (timeElapsed < lerpDuration) {
			switch (state.stateType) {
				case CharacterState.StateType.SingleLayer: {
						character.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, (byte) Mathf.Lerp(0, 255, timeElapsed / lerpDuration));
						break;
					}
				case CharacterState.StateType.MultiLayer: {
						character.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed / lerpDuration));
						character.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed / lerpDuration));
						break;
					}
			}

			timeElapsed += Time.deltaTime;

			yield return null;
		}
		switch (state.stateType) {
			case CharacterState.StateType.SingleLayer: {
					character.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
					break;
				}
			case CharacterState.StateType.MultiLayer: {
					character.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
					character.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
					break;
				}
		}
	}

	IEnumerator FadeOutCharacter(CharacterState state, GameObject character, float lerpDuration = 0.5f,  bool destroy = false) {
		float timeElapsed = 0;

		while (timeElapsed < lerpDuration) {
			switch (state.stateType) {
				case CharacterState.StateType.SingleLayer: {
						character.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));
						break;
					}
				case CharacterState.StateType.MultiLayer: {
						character.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));
						character.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));
						break;
					}
			}

			timeElapsed += Time.deltaTime;

			yield return null;
		}
		switch (state.stateType) {
			case CharacterState.StateType.SingleLayer: {
					character.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
					break;
				}
			case CharacterState.StateType.MultiLayer: {
					character.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
					character.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
					break;
				}
		}

		if (destroy) {
			Destroy(character);
		}
	}
}
