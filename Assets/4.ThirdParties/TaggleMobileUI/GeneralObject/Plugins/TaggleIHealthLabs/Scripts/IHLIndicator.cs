using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IHLIndicator : IHLSingleton<IHLIndicator>
{
    public GameObject canvasObject;

    private void Start()
    {
        canvasObject.SetActive(false);
    }
    public void ShowIndicator()
    {
        canvasObject.SetActive(true);
        StartCoroutine(LoadIndicator(true));
    }

    public void HideIndicator()
    {
        canvasObject.SetActive(false);
        StartCoroutine(LoadIndicator(false));
    }
    IEnumerator LoadIndicator(bool isShow)
    {
#if UNITY_IPHONE
        Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.White);
#elif UNITY_ANDROID
        Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (isShow)
        {
            Handheld.StartActivityIndicator();
        }
        else
        {
            Handheld.StopActivityIndicator();
        }
#endif
        yield return null;
    }
}
