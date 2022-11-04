using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HAGOUIFormPopupView : MonoBehaviour
{
    private CanvasGroup m_canvas;
    private Transform m_content;
    private ScrollRect m_scrollRect;
    private Button m_btnBack;
    private Text m_txtTitle;
    private Text m_txtDesc;
    //
    private HAGOUIFormComponentView m_formView;
    //
    private GameObject m_objDisclaimer;
    private Toggle m_tglDisclaimer;
    private Text m_txtToggleDisclaimer;
    private Text m_txtDisclaimerTerms;
    //
    private Button m_btnSubmit;

    //param
    private HAGOUIFormPopupDTO m_data;
    private Action<HAGOUIFormSubmitDataDTO> m_onCompleteEvent;

    void OnDestroy()
    {
        Destroy();
    }

    public void Destroy()
    {
        //unregister event
        HAGOSubmitFormControl.Api.ExitEvent -= HidePopup;
    }

    public void Init(HAGOUIFormPopupDTO data, Action<HAGOUIFormSubmitDataDTO> onCompleteEvent)
    {
        //find reference
        m_canvas = GetComponent<CanvasGroup>();
        m_content = transform.Find("Content");
        m_btnBack = transform.Find("Content/TopBar/BtnBack").GetComponent<Button>();
        m_scrollRect = transform.Find("Content/Body").GetComponent<ScrollRect>();
        m_txtTitle = transform.Find("Content/Body/Viewport/Content/Info/TxtName").GetComponent<Text>();
        m_txtDesc = transform.Find("Content/Body/Viewport/Content/Info/TxtDesc").GetComponent<Text>();
        m_formView = transform.Find("Content/Body/Viewport/Content/Info/Form").GetComponent<HAGOUIFormComponentView>();
        m_objDisclaimer = transform.Find("Content/Body/Viewport/Content/Info/Disclaimer").gameObject;
        m_tglDisclaimer = transform.Find("Content/Body/Viewport/Content/Info/Disclaimer/TglDisclaimer").GetComponent<Toggle>();
        m_txtToggleDisclaimer = transform.Find("Content/Body/Viewport/Content/Info/Disclaimer/TglDisclaimer/TxtTerms").GetComponent<Text>();
        m_txtDisclaimerTerms = transform.Find("Content/Body/Viewport/Content/Info/Disclaimer/ScrDisclaimerTerms/Viewport/Content/TxtDesc").GetComponent<Text>();
        m_btnSubmit = transform.Find("Content/Body/Viewport/Content/BtnSubmit").GetComponent<Button>();
        
        //update view
        m_canvas.alpha = 0f;

        //add listener
        m_btnBack.onClick.AddListener(BackOnClick);
        m_btnSubmit.onClick.AddListener(SubmitOnClick);

        //register event
        HAGOSubmitFormControl.Api.ExitEvent += HidePopup;

        OnInitFormHandler(data, onCompleteEvent);
    }

    private void BackOnClick()
    {
        HAGOSubmitFormControl.Api.Exit();
    }

    private void SubmitOnClick()
    {
        //check form valid
        HAGOUIFormSubmitDataDTO formData = null;
        if(m_data.IsRequireFormData)
        {
            HAGOUIFormDataResultDTO formResult = m_formView.GetFormDataResult();
            if(formResult.IsSuccess)
            {
                formData = formResult.Data;
            }
            else
            {
                return;
            }
        }

        if(m_data.IsRequireDisclaimer)
        {
            if(!m_tglDisclaimer.isOn)
            {
                OnDisclaimerError();
                return;
            }
            else
            {
                ResetDisclaimerError();
            }
        }

        HAGOSubmitFormControl.Api.CompleteSubmitForm(formData);
    }

    private void OnInitFormHandler(HAGOUIFormPopupDTO data, Action<HAGOUIFormSubmitDataDTO> onCompleteEvent)
    {
        m_data = data;
        m_onCompleteEvent = onCompleteEvent;

        //update title
        m_txtTitle.text = m_data.Title;

        //init desc
        bool showDesc = string.IsNullOrEmpty(m_data.Desc);
        m_txtDesc.gameObject.SetActive(showDesc);
        if(showDesc)
        {
            m_txtDesc.text = m_data.Desc;
        }

        //init form submit
        bool showForm = m_data.FormData != null;
        m_formView.gameObject.SetActive(showForm);
        if(showForm)
        {
            m_formView.Init(true, m_data.FormData, m_scrollRect);
            Transform tfError = m_formView.txtError.transform;
            tfError.SetParent(m_formView.transform.parent);
            tfError.SetAsLastSibling();
        }

        //init disclaimer
        bool showDisclaimer = m_data.IsRequireDisclaimer;
        m_objDisclaimer.SetActive(showDisclaimer);
        if(showDisclaimer)
        {
            m_txtDisclaimerTerms.text = m_data.DisclaimerData.TermsContent;
            m_txtToggleDisclaimer.text = m_data.DisclaimerData.ToggleContent;
        }

        ShowPopup();
    }

    private void OnDisclaimerError()
    {
        HAGOTweenUtils.DOShakeError(m_objDisclaimer.transform);
        HAGOTweenUtils.DOColorError(m_txtToggleDisclaimer);
    }

    private void ResetDisclaimerError()
    {
        m_txtDisclaimerTerms.color = HAGOUtils.ParseColorFromString(HAGOConstant.COLOR_TEXT_DARK_DEFAULT);
    }

    private void ShowPopup()
    {
        HAGOTweenUtils.ShowPopup(m_canvas, m_content);
    }

    private void HidePopup(Action callback)
    {
        HAGOTweenUtils.HidePopup(m_canvas, m_content, callback, false);
    }
}
