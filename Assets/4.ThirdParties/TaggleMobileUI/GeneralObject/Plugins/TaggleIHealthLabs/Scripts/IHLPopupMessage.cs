using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IHLPopupMessage : IHLSingleton<IHLPopupMessage>
{
    public GameObject canvasObject;
    public Image bg;
    public TMP_Text messageText;
    public Color backgroundColor;
    public Color backgroundErrorColor;
    public Image dataNullImage;
    private void Start() {
        bg.color = backgroundColor;
        dataNullImage.color = backgroundErrorColor;
        dataNullImage.gameObject.SetActive(false);
        canvasObject.SetActive(false);
    }
    public void ShowMessage(string message)
    {
        bg.color = backgroundColor;
        StartCoroutine(AutoHide(message));
    }
     public void ShowErrorMessage(string message)
    {
        bg.color = backgroundErrorColor;
        StartCoroutine(AutoHide(message));
    }
    public void ShowErrorDelay()
    {
        bg.gameObject.SetActive(false);
        dataNullImage.color = backgroundErrorColor;
        dataNullImage.gameObject.SetActive(true);
        StartCoroutine(AutoHideDelay());
    }
    public void HideIndicator()
    {
        bg.color = backgroundColor;
        bg.gameObject.SetActive(true);
        dataNullImage.gameObject.SetActive(false);
        messageText.text = "";
        canvasObject.SetActive(false);
    }
    IEnumerator AutoHide(string message = "")
    {
        messageText.text = message;
        canvasObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        HideIndicator();
    }
    IEnumerator AutoHideDelay()
    {
        canvasObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        HideIndicator();
    }
}
