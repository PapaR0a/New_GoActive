using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Honeti;
using UnityEngine;
using UnityEngine.UI;

public class CPEPopupMessageView : MonoBehaviour
{
	private CanvasGroup m_canvas;
	private GameObject m_iconSuccess;
	private GameObject m_iconError;
	private Text m_txtContent;
	private Button m_btnOK;

	//param
	private Action m_onClose;

	public void Init(BWPopupIconType iconType, string content = "", bool autoTurnOff = false, Action onClose = null)
	{
		m_onClose = onClose;

		//find reference
		m_canvas = transform.Find("Content").GetComponent<CanvasGroup>();
		m_txtContent = transform.Find("Content/TxtContent").GetComponent<Text>();
		m_btnOK = transform.Find("Content/Navigation/BtnOK").GetComponent<Button>();
		m_iconSuccess = transform.Find("Content/IconSuccess").gameObject;
		m_iconError = transform.Find("Content/IconError").gameObject;

		//handle value
		m_iconSuccess.gameObject.SetActive(iconType == BWPopupIconType.Success);
		m_iconError.gameObject.SetActive(iconType == BWPopupIconType.Error);
		m_txtContent.text = GetContent(iconType,content);
		m_btnOK.gameObject.SetActive(!autoTurnOff);

		//add listener
		m_btnOK.onClick.AddListener(OKOnClick);

		OpenPopup(autoTurnOff);
	}

	private void OKOnClick()
	{
		ClosePopup();
	}

	private string GetContent(BWPopupIconType iconType, string content)
	{
		if(content == "")
		{
			return I18N.instance.getValue(iconType == BWPopupIconType.Success ? CPELangConstant.SUCCESSFUL : CPELangConstant.FAILED);
		}
		
		return content;
	}

	private void OpenPopup(bool autoTurnOff, float delayAutoTurnOff = 2f)
	{
		m_canvas.interactable = false;
		m_canvas.gameObject.transform.DOScale(Vector3.one, 0.25f);
		m_canvas.DOFade(0f,0.25f).From()
		.OnComplete(()=>{
			m_canvas.interactable = true;
			if(autoTurnOff)
			{
				Invoke("ClosePopup", delayAutoTurnOff);
			}
		});
	}
	
	private void ClosePopup()
	{
		m_canvas.interactable = false;
		m_canvas.gameObject.transform.DOScale(Vector3.one * 0.75f, 0.25f);
		m_canvas.DOFade(0f,0.25f).OnComplete(()=>{
			m_onClose?.Invoke();
			Destroy(gameObject);
		});
	}
}
