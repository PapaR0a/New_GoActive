using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HAGOUIDateTimeComponentView : MonoBehaviour, HAGOUIIComponent
{
	public bool isInitByUser = false; //using for dynamic init
    [Space(12)] //blanck space on inspector

	private CanvasGroup m_canvas;
	private HAGOUIFormItemStatusView m_formItem;
	private Text m_txtTitle;
	private Button m_btnDate;
	private Text m_txtDate;
	private Button m_btnTime;
	private Text m_txtTime;

	//param
	private HAGOUIDateTimeComponentDTO m_data;
	private DateTime m_value;
	private bool m_isInitComplete = false;
	private bool m_isEditMode;
	
	void Start()
    {
        if(!isInitByUser)
        {
			Init(null, true);
        }
    }

	public void Init(object data, bool isEditMode)
	{
		if(m_isInitComplete)
        {
            Debug.LogError(this.GetType().Name + " already init! Please set isInitByUser = false in inspector if init via script.");
            return;
        }

		this.m_data = (HAGOUIDateTimeComponentDTO)data;
		m_isEditMode = isEditMode;

		//find reference
        m_canvas = GetComponent<CanvasGroup>();
        m_formItem =  GetComponent<HAGOUIFormItemStatusView>();
        m_txtTitle = transform.Find("Date/BtnDate/TxtTitle").GetComponent<Text>();
		m_btnDate = transform.Find("Date/BtnDate").GetComponent<Button>();
		m_txtDate = transform.Find("Date/BtnDate/Text").GetComponent<Text>();
		m_btnTime = transform.Find("Time/BtnTime").GetComponent<Button>();
		m_txtTime = transform.Find("Time/BtnTime/Text").GetComponent<Text>();
		
		m_canvas.interactable = m_isEditMode;
		//
		if(this.m_data != null) //handle dynamic UI value
        {
			//set title
            if(m_txtTitle != null)
            {
                m_txtTitle.text = this.m_data.Title;
            }
			//set key form response error
			if(m_formItem != null)
			{
				m_formItem.keyItem = this.m_data.KeyForm;
			}
			//
			SetDateTime(m_data.Value);
		}
		else //handle fixed UI value
		{
			SetDateTime(DateTime.Now);
		}

		//add listener
		m_btnTime.onClick.AddListener(TimeOnClick);
		m_btnDate.onClick.AddListener(DateOnClick);

		m_isInitComplete = true;
	}

	private void DateOnClick()
	{
		HAGODateTimePickerManager.Api.InitSingleDatePicker(SetDateTime);
	}

	private void TimeOnClick()
	{
		HAGODateTimePickerManager.Api.InitTimePicker((val) => SetDateTime(m_value.Date.Add(val)));
	}

	public void SetDateTime(DateTime dateTime)
	{
		if (dateTime == null)
		{
			return;
		}

		m_value = dateTime;
		SetTextValue();
	}

	private void SetTextValue()
	{
		m_txtDate.text = m_value.ToString(HAGOConstant.FORMAT_DATE);
		m_txtTime.text = m_value.ToString(HAGOConstant.FORMAT_TIME_12_HOURS);
	}

	public void ActiveError()
    {
        m_formItem.ActiveError();
    }

	public void ResetError()
	{
		m_formItem.ResetError();
	}

	public string GetKeyForm()
	{
		if(m_formItem == null)
		{
			m_formItem = GetComponent<HAGOUIFormItemStatusView>();
		}

		return m_formItem != null ? m_formItem.keyItem : string.Empty;
	}

	public DateTime GetValue()
	{
		return m_value;
	}

	public string GetJsonValue()
	{
		return HAGOUtils.GetEpochTimeFromDateTime(m_value).ToString();
	}

	public long GetID()
    {
        return m_data != null ? m_data.ID : transform.GetSiblingIndex();
    }

	public object ExportView(int id)
	{
		if(m_txtTitle == null)
        {
			m_txtTitle = transform.Find("Date/BtnDate/TxtTitle").GetComponent<Text>();
        }

		return new HAGOUIDateTimeComponentDTO(
			id,
			m_txtTitle.text,
			GetKeyForm(),
			GetValue()
		);
	}

    public string GetFormType()
    {
        return HAGOServiceKey.PARAM_DATETIME_COMPONENT;
    }

	public void SetValue(string value)
    {
		try
		{
			long dateEpoch = long.Parse(value);
			DateTime date = HAGOUtils.GetDateTimeFromEpoch(dateEpoch);

			SetDateTime(date);
		}
		catch(Exception ex)
		{
			Debug.Log("[HADateTimeComponentView] Cannot parse: " + ex.ToString());
		}
    }
}
