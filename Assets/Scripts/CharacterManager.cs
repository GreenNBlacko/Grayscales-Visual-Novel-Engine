using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour {
	public GameObject CharacterArray = null;

	// private methods
	private Dictionary<int, CharacterData> CharactersInScene = new Dictionary<int, CharacterData>();
	public static SentenceManager sentenceManager => MainManager.sentenceData;

	void Awake() {
		//sentenceManager = FindObjectOfType<DialogueManager>().scripts.sentenceManager;

		LeanTween.reset();
	}

	public async Task AddCharacterToScene(int Name, OnSentenceInit.StartingPlace startPosition, CharacterState state, Vector2 position, bool enterScene = false, float speed = 0f, bool FadeIn = false, float FadeSpeed = 0.5f) {
		await AddCharacterToScene(Name, startPosition, Vector2.zero, state, position, enterScene, speed, FadeIn, FadeSpeed);
	}

	public async Task AddCharacterToScene(int Name, OnSentenceInit.StartingPlace startPosition, Vector2 customsStartPosition, CharacterState state, Vector2 position, bool enterScene = false, float speed = 0f, bool FadeIn = false, float FadeSpeed = 0.5f) {
		if (CharacterArray == null) {
			Debug.LogError("No Character array gameObject found.");
			return;
		}

		CharactersInScene.TryGetValue(Name, out CharacterData tempchar);

		if (tempchar != null) {
			Debug.LogError("Character(" + Name + ") is already in the scene.");
			return;
		}

		CharacterData characterData = new CharacterData();

		GameObject character = new GameObject();

		character.AddComponent<RectTransform>();
		character.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
		character.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
		character.GetComponent<RectTransform>().pivot = new Vector2(0, 0);

		character.AddComponent<Image>();
		character.GetComponent<Image>().sprite = state.BaseLayer;
		character.GetComponent<Image>().SetNativeSize();
		Destroy(character.GetComponent<Image>());

		characterData.characterGO = character;

		character.name = SentenceTools.GetSelectedLanguagePack().characters[Name].CharacterName;

		characterData.CreateSprites(state, out Image baseLayer, out Image expressionLayer);

		position = GetPosition(position);

		characterData.BaseLayer = baseLayer;

		character.transform.SetParent(CharacterArray.transform);

		character.GetComponent<RectTransform>().localScale = new Vector3(1 * GetCharacter(Name).CharacterScale.x, 1 * GetCharacter(Name).CharacterScale.y, 1);

		CharactersInScene.Add(Name, characterData);

		Vector2 startingPosition = new Vector2(0, 0);

		switch (startPosition) {
			case OnSentenceInit.StartingPlace.Left: {
				startingPosition = new Vector2(0 - baseLayer.rectTransform.rect.width, 0);
				break;
			}
			case OnSentenceInit.StartingPlace.Right: {
				startingPosition = new Vector2(2560 + baseLayer.rectTransform.rect.width, 0);
				break;
			}
			case OnSentenceInit.StartingPlace.Custom: {
				startingPosition = customsStartPosition;
				break;
			}
		}

		character.GetComponent<RectTransform>().anchoredPosition = startingPosition;

		characterData.BaseLayer = baseLayer;
		if (state.stateType == CharacterState.StateType.MultiLayer)
			characterData.ExpressionLayer = expressionLayer;

		await characterData.CharacterEnter(position, speed, FadeSpeed, enterScene, FadeIn);
	}

	public Character GetCharacter(int Name) {
		return sentenceManager.Characters[Name];
	}

	public async Task MoveCharacter(int index, Vector2 position, float speed) {
		if (!CharactersInScene.TryGetValue(index, out CharacterData character)) {
			Debug.LogError("Character is not in the scene");
			return;
		}

		position = GetPosition(position);

		await character.CharacterMove(position, speed);
	}

	public async Task ChangeCharacterState(int index, CharacterState state, bool transition = false, float speed = 0.5f) {
		CharactersInScene.TryGetValue(index, out CharacterData character);

		if (character == null) {
			Debug.LogError("Character is not in the scene");
			return;
		}

		if (transition) {
			await character.CharacterTransition(state, speed);
		} else {
			character.characterGO.AddComponent<Image>();

			character.BaseLayer.sprite = state.BaseLayer;

			character.characterGO.GetComponent<Image>().sprite = state.BaseLayer;

			if (character.ExpressionLayer != null) {
				character.ExpressionLayer.sprite = state.ExpressionLayer;
			}

			character.characterGO.GetComponent<Image>().SetNativeSize();
			Destroy(character.characterGO.GetComponent<Image>());
		}
	}

	public async Task RemoveCharacterFromScene(int index, Vector2 position, bool exitScene = false, float speed = 0f, bool FadeOut = false, float FadeSpeed = 0.5f) {
		CharactersInScene.TryGetValue(index, out CharacterData character);

		if (character == null) {
			Debug.LogError("Character is not in the scene");
			return;
		}

		position = GetPosition(position);

		await character.CharacterExit(position, speed, FadeSpeed, exitScene, FadeOut, true);
	}

	public void SetCharacterIndex(int name, int index) {
		if (!CharactersInScene.TryGetValue(name, out CharacterData character)) {
			Debug.LogError("Character is not in scene");
			return;
		}

		if (index >= CharactersInScene.Count) {
			index = CharactersInScene.Count - 1;
		}

		if (index < 0) {
			index = 0;
		}

		character.characterGO.transform.SetSiblingIndex(index);

		Debug.Log(name + "'s index set to " + index);
	}

	public void RemoveAllCharactersFromScene() {
		StopAllCoroutines();
		foreach (var value in CharactersInScene.Keys)
			_ = RemoveCharacterFromScene(value, Vector2.zero);
	}

	public bool CharacterInScene(int index) {
		return CharactersInScene.TryGetValue(index, out _);
	}

	public static Vector2 GetPosition(Vector2 position) {
		position.x /= 2650;
		position.y /= 1440;

		position.x *= 1920;
		position.y *= 1080;

		return position;
	}
}

[Serializable]
public class CharacterData {
	public GameObject characterGO;
	public Image BaseLayer;
	public Image ExpressionLayer;

	private LTDescr MoveAnim;
	private LTDescr BaseAnim;
	private LTDescr ExpressionAnim;

	public async Task CharacterEnter(Vector2 Pos, float EnterSpeed, float FadeSpeed, bool enterScene, bool fadeIn) {
		if (enterScene)
			_ = CharacterMove(Pos, EnterSpeed);

		if (fadeIn) {
			BaseLayer.color = SentenceTools.SetColorAlpha(BaseLayer.color, 0);

			if (ExpressionLayer != null)
				ExpressionLayer.color = SentenceTools.SetColorAlpha(ExpressionLayer.color, 0);

			_ = CharacterFade(1, FadeSpeed);
		}


		while (MoveAnim != null || BaseAnim != null) {
			await Task.Delay(1);
		}

		return;
	}

	public async Task CharacterExit(Vector2 Pos, float ExitSpeed, float FadeSpeed, bool ExitScene, bool fadeOut, bool destroy = false) {
		if (ExitScene)
			_ = CharacterMove(Pos, ExitSpeed);

		if (fadeOut) {
			_ = CharacterFade(0, FadeSpeed);
		}


		while (MoveAnim != null || BaseAnim != null) {
			await Task.Delay(1);
		}

		if (destroy) {
			UnityEngine.Object.Destroy(characterGO);
		}

		return;
	}

	public async Task CharacterTransition(CharacterState state, float transitionSpeed) {
		float fadeOutMultiplier = 1.7f;

		if (BaseLayer.sprite == state.BaseLayer && ExpressionLayer != null && ExpressionLayer.sprite == state.ExpressionLayer)
			return;

		if (BaseLayer.sprite != state.BaseLayer)
			_ = FadeBase(0, transitionSpeed * fadeOutMultiplier);

		if (ExpressionLayer != null && ExpressionLayer.sprite != state.ExpressionLayer)
			_ = FadeExpression(0, transitionSpeed * fadeOutMultiplier);

		GameObject baLayer = BaseLayer.gameObject;
		GameObject exLayer = ExpressionLayer != null ? ExpressionLayer.gameObject : null;

		CreateSprites(state, out BaseLayer, out ExpressionLayer);

		BaseLayer.color = SentenceTools.SetColorAlpha(BaseLayer.color, 0);

		if (ExpressionLayer != null)
			ExpressionLayer.color = SentenceTools.SetColorAlpha(ExpressionLayer.color, 0);

		if (BaseLayer.sprite != state.BaseLayer)
			_ = FadeBase(1, transitionSpeed);

		if (state.stateType == CharacterState.StateType.MultiLayer && ExpressionLayer.sprite != state.ExpressionLayer)
			_ = FadeExpression(1, transitionSpeed);

		while (BaseAnim != null || ExpressionAnim != null) {
			await Task.Delay(1);
		}

		UnityEngine.Object.Destroy(baLayer);
		if (exLayer != null)
			UnityEngine.Object.Destroy(exLayer);

		return;
	}

	public async Task CharacterMove(Vector2 Pos, float MoveSpeed) {
		MoveAnim = LeanTween.move(characterGO.GetComponent<RectTransform>(), Pos, MoveSpeed);

		while (LeanTween.isTweening(MoveAnim.id)) {
			await Task.Delay(1);
		}

		MoveAnim = null;

		return;
	}

	public async Task TestAnim() {
		LTDescr anim = LeanTween.move(characterGO.GetComponent<RectTransform>(), CharacterManager.GetPosition(new Vector2(2000, 0)), 10f);

		while (characterGO.GetComponent<RectTransform>().anchoredPosition.x < 500) {
			await Task.Delay(1);
		}

		LeanTween.cancel(anim.id);
	}

	public async Task CharacterFade(float to, float fadeSpeed, bool overrideCancel = false) {
		if (!overrideCancel && (BaseAnim != null || ExpressionAnim != null)) {
			LeanTween.cancel(BaseAnim.id);
			LeanTween.cancel(ExpressionAnim.id);
		}

		_ = FadeBase(to, fadeSpeed);

		if (ExpressionLayer != null) {
			_ = FadeExpression(to, fadeSpeed);
		}

		while (BaseAnim != null || ExpressionAnim != null) {
			await Task.Delay(1);
		}

		return;
	}

	public async Task FadeBase(float to, float fadeSpeed) {
		BaseAnim = LeanTween.alpha(BaseLayer.rectTransform, to, fadeSpeed);

		while (LeanTween.isTweening(BaseAnim.id))
			await Task.Delay(1);

		BaseAnim = null;
	}

	public async Task FadeExpression(float to, float fadeSpeed) {
		ExpressionAnim = LeanTween.alpha(BaseLayer.gameObject, to, fadeSpeed);

		while (LeanTween.isTweening(ExpressionAnim.id))
			await Task.Delay(1);

		ExpressionAnim = null;
	}

	public void CreateSprites(CharacterState state, out Image baLayer, out Image exLayer) {
		GameObject baseLayer = new GameObject();

		baseLayer.name = "Base Layer";

		baseLayer.transform.SetParent(characterGO.transform);

		baseLayer.AddComponent<RectTransform>();
		baseLayer.AddComponent<Image>();

		baseLayer.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
		baseLayer.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
		baseLayer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

		baseLayer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
		baseLayer.GetComponent<RectTransform>().localScale = Vector3.one;

		baLayer = baseLayer.GetComponent<Image>();

		exLayer = baLayer;

		baseLayer.GetComponent<Image>().sprite = state.BaseLayer;

		switch (state.stateType) {
			case CharacterState.StateType.MultiLayer: {
				GameObject expressionLayer = new GameObject();

				expressionLayer.name = "Expression Layer";

				expressionLayer.transform.SetParent(characterGO.transform);

				expressionLayer.AddComponent<RectTransform>();
				expressionLayer.AddComponent<Image>();

				expressionLayer.GetComponent<Image>().sprite = state.ExpressionLayer;

				expressionLayer.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
				expressionLayer.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
				expressionLayer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

				expressionLayer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
				expressionLayer.GetComponent<RectTransform>().localScale = Vector3.one;

				if (state.Advanced) {
					characterGO.GetComponent<Image>().sprite = state.BaseLayer;
					characterGO.GetComponent<Image>().SetNativeSize();

					UnityEngine.Object.Destroy(characterGO.GetComponent<Image>());

					baseLayer.GetComponent<Image>().SetNativeSize();

					baseLayer.GetComponent<RectTransform>().anchoredPosition = state.BaseLayerPosition;

					expressionLayer.GetComponent<Image>().SetNativeSize();

					expressionLayer.GetComponent<RectTransform>().anchoredPosition = state.ExpressionLayerPosition;
				}

				exLayer = expressionLayer.GetComponent<Image>();
				break;
			}
		}
	}
}
