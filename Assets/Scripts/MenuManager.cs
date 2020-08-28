using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public RectTransform[] MenuGameObjects;

    public SavingAndLoading savingAndLoadingOptions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class SavingAndLoading {
    [Range(1, 25)]
    public int numberOfPages = 8;
    [Range(15, 60)]
    public int savesPerPage = 30;
}
