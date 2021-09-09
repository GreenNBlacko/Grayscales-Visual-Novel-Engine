using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TopToolBar : MonoBehaviour {

    void Update() {
        MouseOverToolbar();
    }

    public void MouseOverToolbar() {
        bool mouseOverToolbar = false;

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        foreach(RaycastResult result in raycastResults) {
            if (result.gameObject.transform == transform) { mouseOverToolbar = true; break; }
            if(result.gameObject.transform.IsChildOf(transform) && result.gameObject == EventSystem.current.currentSelectedGameObject) { EventSystem.current.SetSelectedGameObject(null); }
            if(result.gameObject.name == "HandleHP") { EventSystem.current.SetSelectedGameObject(null); }
        }

        if(mouseOverToolbar) {
            foreach(Image child in transform.GetComponentsInChildren<Image>()) {
                child.color = new Color32(255, 255, 255, 255);
			}
		} else {
            foreach (Image child in transform.GetComponentsInChildren<Image>()) {
                child.color = new Color32(255, 255, 255, 30);
            }
        }

        transform.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
    }
}
