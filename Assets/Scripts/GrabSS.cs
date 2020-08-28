using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class GrabSS : MonoBehaviour
{
    public DialogueManager mainScript;

    public Camera cam;

    private Texture2D imageOUT;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            TakeSS();
        }
    }

    public void TakeSS() {
        mainScript.HideUI();

        RenderTexture imageIN = new RenderTexture(Screen.width, Screen.height, 24);
        imageIN.Create();

        cam.targetTexture = imageIN;

        StartCoroutine(readPixels(imageIN));

        cam.targetTexture = null;

        mainScript.ShowUI();
    }

    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    public IEnumerator readPixels(RenderTexture imageIN) {
        yield return frameEnd;

        imageOUT = new Texture2D(Screen.width, Screen.height);
        imageOUT.ReadPixels(new Rect(0, 0, imageIN.width, imageIN.height), 0, 0);
        imageOUT.Apply();

        byte[] imageOUTbytes = imageOUT.EncodeToPNG();

        File.WriteAllBytes(Application.dataPath + "/Screenshot.png", imageOUTbytes);

        StopCoroutine(readPixels(imageIN));
    }
}
