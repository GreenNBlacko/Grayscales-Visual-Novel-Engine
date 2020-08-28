using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGManager : MonoBehaviour {
    public Image BGImage;
    public Image CGImage;

    public BG[] BGList;

    public CG[] CGList;

    public void ShowBG(int BG_ID) {
        BGImage.sprite = BGList[BG_ID].bgImage;
        BGList[BG_ID].bgViewed = true;
    }

    public void ShowCG(int CG_ID) {
        CGImage.sprite = CGList[CG_ID].cgImage;
        CGList[CG_ID].cgViewed = true;
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