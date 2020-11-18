using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CGManager : MonoBehaviour {
	public Transform BGDisplay;
	public Transform CGDisplay;

	public BG[] BGList;

	public CG[] CGList;

	public void ShowBG(int BG_ID) {
		if (BGDisplay.childCount < 1) {
			CreateArtworkObject(BGDisplay, BGList[BG_ID].bgImage);
		}
		if (BGDisplay.GetChild(BGDisplay.childCount - 1).GetComponent<Image>().sprite == BGList[BG_ID].bgImage) { return; }
		StartCoroutine(TransitionArtwork(BGDisplay, BGList[BG_ID].bgImage));
		BGList[BG_ID].bgViewed = true;
	}

	public void ShowCG(int CG_ID) {
		if (CGDisplay.childCount < 1) {
			CreateArtworkObject(CGDisplay, CGList[CG_ID].cgImage);
		}
		if (CGDisplay.GetChild(CGDisplay.childCount - 1).GetComponent<Image>().sprite == CGList[CG_ID].cgImage) { return; }
		StartCoroutine(TransitionArtwork(CGDisplay, CGList[CG_ID].cgImage));
		CGList[CG_ID].cgViewed = true;
	}

	public void CreateArtworkObject(Transform list, Sprite Artwork) {
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
	}

	IEnumerator TransitionArtwork(Transform list, Sprite Artwork) {
		float timeElapsed = 0, lerpDuration = 1.5f;

		GameObject temp = Instantiate(list.GetChild(0).gameObject, list);
		temp.transform.SetAsLastSibling();
		temp.name = Artwork.name;

		temp.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
		temp.GetComponent<Image>().sprite = Artwork;

		while (timeElapsed < lerpDuration) {
			list.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(255, 0, timeElapsed / lerpDuration));

			temp.GetComponent<Image>().color = new Color32(255, 255, 255, (byte)Mathf.Lerp(0, 255, timeElapsed * 2 / lerpDuration));

			timeElapsed += Time.deltaTime;

			yield return null;
		}

		Destroy(list.GetChild(0).gameObject);
	}
}

[System.Serializable]
public class CG {
	public Sprite cgImage;
	public bool cgViewed;

}

[System.Serializable]
public class BG {
	public Sprite bgImage;
	public bool bgViewed;
}