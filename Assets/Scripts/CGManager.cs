using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CGManager : MonoBehaviour {
	public Transform BGDisplay;
	public Transform CGDisplay;

	public Artwork[] BGList;

	public Artwork[] CGList;

	public async Task ShowBG(int BG_ID, bool transition = false, float fadeSpeed = 0.7f) {
		if (BG_ID == -1) {
			if (transition)
				await ArtworkTransition(BGDisplay, null, fadeSpeed);
			else
				CreateArtworkObject(BGDisplay, null);
			return;
		}

		if (!transition) { CreateArtworkObject(BGDisplay, BGList[BG_ID].artworkImage); return; }

		if (BGDisplay.childCount > 0)
			if (BGDisplay.GetChild(BGDisplay.childCount - 1).GetComponent<Image>().sprite == BGList[BG_ID].artworkImage) { return; }
		await ArtworkTransition(BGDisplay, BGList[BG_ID].artworkImage, fadeSpeed);
	}

	public async Task ShowCG(int CG_ID, bool transition = false, float fadeSpeed = 0.7f) {
		if (CG_ID == -1) {
			if (transition)
				await ArtworkTransition(CGDisplay, null, fadeSpeed);
			else
				CreateArtworkObject(CGDisplay, null);
			return; 
		}

		if (!transition) { CreateArtworkObject(CGDisplay, CGList[CG_ID].artworkImage); return; }

		if (CGDisplay.childCount > 0) 
			if (CGDisplay.GetChild(CGDisplay.childCount - 1).GetComponent<Image>().sprite == CGList[CG_ID].artworkImage) { return; }

		await ArtworkTransition(CGDisplay, CGList[CG_ID].artworkImage, fadeSpeed);
	}

	public GameObject CreateArtworkObject(Transform list, Sprite Artwork) {
		GameObject art = new GameObject();
		art.transform.SetParent(list);
		art.name = Artwork.name;
		RectTransform tr = art.AddComponent<RectTransform>();
		Image temp = art.AddComponent<Image>();
		temp.sprite = Artwork;
		tr.localScale = Vector3.one;
		tr.pivot = new Vector2(0, 0);
		tr.anchorMin = new Vector2(0, 0);
		tr.anchorMax = new Vector2(1, 1);
		tr.anchoredPosition = new Vector2(0, 0);
		tr.sizeDelta = new Vector2(1, 1);
		tr.SetAsLastSibling();
		return art;
	}

	public async Task ArtworkTransition(Transform List, Sprite Artwork, float transitionSpeed) {
		float fadeOutMultiplier = 1.7f;

		Task oldFade = null, newFade;

		if (List.childCount > 0) {
			RectTransform artworkObject = List.GetChild(List.childCount - 1).GetComponent<RectTransform>();

			oldFade = FadeArtwork(artworkObject, 0, transitionSpeed * fadeOutMultiplier);

		}

		Image newArtwork = CreateArtworkObject(List, Artwork).GetComponent<Image>();

		newArtwork.color = SentenceTools.SetColorAlpha(newArtwork.color, 0);

		newFade = FadeArtwork(newArtwork.rectTransform, 1, transitionSpeed);

		if (oldFade == null)
			oldFade = newFade;

		while (!oldFade.IsCompleted || !newFade.IsCompleted) {
			await Task.Delay(1);
		}

		return;
	}

	public async Task FadeArtwork(RectTransform artwork, float to, float fadeSpeed, bool destroy = false) {
		LTDescr anim = LeanTween.alpha(artwork, to, fadeSpeed);

		while (LeanTween.isTweening(anim.id))
			await Task.Delay(1);

		if (destroy)
			Destroy(artwork);
	}
}