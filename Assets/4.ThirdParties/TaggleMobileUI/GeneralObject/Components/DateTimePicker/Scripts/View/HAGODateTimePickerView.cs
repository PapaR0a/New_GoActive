using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HAGODateTimePickerView : MonoBehaviour
{
    private CanvasGroup m_canvas;
    private Transform m_content;
    private Button m_btnCancel;
    private Button m_btnDone;
    private HAGOTimePickerView m_timePickerView;
    private HAGODatePickerComponentView m_datePickerView;

    //param
    private HAGODateTimePickerType m_type;

    void OnDestroy()
    {
        Destroy();
    }

    public void Destroy()
    {
        //register event
		HAGODateTimePickerControl.Api.ExitEvent -= ClosePopup;
    }

    public void InitSingleDatePicker(DateTime selectedDate, Dictionary<DateTime,bool> dateMarkData = null)
    {
        m_type = HAGODateTimePickerType.SingleDate;

        //update view
        InitView();
        m_datePickerView.InitDatePicker(null, selectedDate, dateMarkData);

        ShowPopup();
    }

    public void InitMultiDatePicker(Dictionary<DateTime,bool> dateMarkData = null)
    {
        m_type = HAGODateTimePickerType.MultiDate;

        //update view
        InitView();
        m_datePickerView.InitMultiDatePicker(
            (lstDateSelected) => {
                SetInteractableButtonDone(lstDateSelected.Count > 0);
            },
            null,
            dateMarkData
        );

        ShowPopup();
    }

    public void InitTimePicker(HAGOTimePickerType timePickerType, TimeSpan limitDuration)
    {
        m_type = HAGODateTimePickerType.Time;

        //update view
        InitView();
        StartCoroutine(IEInitTimePickerView(timePickerType, limitDuration));
    }

    private void InitView()
    {
        //find reference
        m_canvas = GetComponent<CanvasGroup>();
        m_content = transform.Find("Content");
        m_timePickerView = transform.Find("Content/Body/TimePicker").GetComponent<HAGOTimePickerView>();
        m_datePickerView = transform.Find("Content/Body/DatePicker").GetComponent<HAGODatePickerComponentView>();
        m_btnCancel = transform.Find("Content/Body/BottomBar/BtnBack").GetComponent<Button>();
        m_btnDone = transform.Find("Content/Body/BottomBar/BtnDone").GetComponent<Button>();

        //update view
        m_canvas.alpha = 0f;
        m_timePickerView.gameObject.SetActive(m_type == HAGODateTimePickerType.Time);
        m_datePickerView.gameObject.SetActive(m_type != HAGODateTimePickerType.Time);
        SetInteractableButtonDone(m_type != HAGODateTimePickerType.MultiDate);

        //add listener
        m_btnCancel.onClick.AddListener(CancelOnClick);
        m_btnDone.onClick.AddListener(DoneOnClick);

        //register event
		HAGODateTimePickerControl.Api.ExitEvent += ClosePopup;
    }

    private IEnumerator IEInitTimePickerView(HAGOTimePickerType timePickerType, TimeSpan limitDuration)
    {
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(m_timePickerView.Init(timePickerType, limitDuration));
        yield return new WaitForEndOfFrame();
        
        ShowPopup();
    }

    private void CancelOnClick()
    {
        HAGODateTimePickerControl.Api.Exit();
    }

    private void DoneOnClick()
    {
        if(m_type == HAGODateTimePickerType.Time)
        {
            HAGODateTimePickerControl.Api.CompleteTimePicker(m_timePickerView.GetValue());
        }
        else if(m_type == HAGODateTimePickerType.SingleDate)
        {
            HAGODateTimePickerControl.Api.CompleteSingleDatePicker(m_datePickerView.GetSingleDateValue());
        }
        else if(m_type == HAGODateTimePickerType.MultiDate)
        {
            HAGODateTimePickerControl.Api.CompleteMultiDatePicker(m_datePickerView.GetMultiDateValue());
        }
    }

    private void SetInteractableButtonDone(bool isInteractable)
    {
        m_btnDone.interactable = isInteractable;
    }

    private void ClosePopup(Action callback = null)
    {
        HAGOTweenUtils.HidePopup(m_canvas, m_content, callback, false);
    }

    private void ShowPopup()
    {
        HAGOTweenUtils.ShowPopup(m_canvas, m_content);
    }
}
