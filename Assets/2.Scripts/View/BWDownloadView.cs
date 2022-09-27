using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BWDownloadView : MonoBehaviour
{
	private CanvasGroup m_canvas;
    private Slider m_progressBar;
    
	//param
	private bool m_isShowedUI = false;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        BWDownloadControl.Api.Init();
    }
    
    private void Init()
    {
		//find reference
		m_canvas = GetComponent<CanvasGroup>();
        m_progressBar = transform.Find("ProgressBar").GetComponent<Slider>();

		//handle value
		m_canvas.alpha = 0f;
		m_progressBar.value = 0f;

        //unregister event
        BWDownloadControl.Api.ProcessEvent += ProgressChangeHandler;	
    }

    void OnDestroy()
    {
        BWDownloadControl.Api.ProcessEvent -= ProgressChangeHandler;
    }

	private bool CanShowUI(float progress)
	{
		bool canShowUI = !m_isShowedUI && m_progressBar.value == 0f && progress != 1f;
		m_isShowedUI = true;

		return canShowUI;
	}

    private void ProgressChangeHandler(float progress)
    {
		if(CanShowUI(progress))
			FadeIn();

        m_progressBar.value = progress;

		//Download finish
		if(progress == 1f)
			FadeOut();
    }

	private void FadeIn()
	{
		m_canvas.alpha = 0f;
		m_canvas.DOFade(1f,0.3f).SetEase(Ease.OutExpo);
	}

	private void FadeOut()
	{
		if(m_canvas.alpha == 0f)
			return;
		
		m_canvas.alpha = 1f;
		m_canvas.DOFade(0f,0.3f).SetEase(Ease.OutExpo);
	}
}
