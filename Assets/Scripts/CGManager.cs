using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CGManager : MonoBehaviour {
	public Transform BGDisplay;
	public Transform CGDisplay;

	public Artwork[] BGList;

	public Artwork[] CGList;

	public void ShowBG(int BG_ID, float fadeSpeed = 0.7f) {
		if (BG_ID == -1) { StartCoroutine(TransitionArtwork(BGDisplay, null, fadeSpeed)); return; }
		if (BGDisplay.childCount > 0) if (BGDisplay.GetChild(BGDisplay.childCount - 1).GetComponent<Image>().sprite == BGList[BG_ID].artworkImage) { return; }
		StartCoroutine(TransitionArtwork(BGDisplay, BGList[BG_ID].artworkImage, fadeSpeed));
		BGList[BG_ID].artworkViewed = true;
	}

	public void ShowCG(int CG_ID, float fadeSpeed = 0.7f) {
		if (CG_ID == -1) { StartCoroutine(TransitionArtwork(CGDisplay, null, fadeSpeed)); return; }
		if (CGDisplay.childCount > 0) if (CGDisplay.GetChild(CGDisplay.childCount - 1).GetComponent<Image>().sprite == CGList[CG_ID].artworkImage) { return; }
		StartCoroutine(TransitionArtwork(CGDisplay, CGList[CG_ID].artworkImage, fadeSpeed));
		CGList[CG_ID].artworkViewed = true;
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
		return art;
	}

	IEnumerator TransitionArtwork(Transform list, Sprite Artwork, float lerpDuration) {
		float timeElapsed = 0;

		if(Artwork == null) {
			GameObject artworkObject = list.GetChild(list.childCount - 1).gameObject;

			artworkObject.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
			artworkObject.GetComponent<Image>().sprite = Artwork;

			while (timeElapsed <= lerpDuration) {
				artworkObject.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed / lerpDuration));

				timeElapsed += Time.deltaTime;

				yield return null;
			}

			Destroy(artworkObject);
		}

		GameObject temp;
		if (list.childCount > 0) temp = Instantiate(list.GetChild(0).gameObject, list);
		else temp = CreateArtworkObject(list, Artwork);

		temp.transform.SetAsLastSibling();
		temp.name = Artwork.name;

		temp.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
		temp.GetComponent<Image>().sprite = Artwork;

		while (timeElapsed <= lerpDuration) {
			if(list.childCount > 1) list.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));

			temp.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed / lerpDuration));

			timeElapsed += Time.deltaTime;

			yield return null;
		}

		if (list.childCount > 1) Destroy(list.GetChild(0).gameObject);
	}
}

[System.Serializable]
public class Artwork {
	public string Name;
	public Sprite artworkImage;
	public bool artworkViewed;

}