using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class HAGOUIDateComponentView : MonoBehaviour, HAGOUIIComponent
{
	public bool isInitByUser = false; //using for dynamic init
    [Space(12)] //blanck space on inspector

	private CanvasGroup m_canvas;
	private Button m_btnDate;
	private HAGOUIFormItemStatusView m_formItem;
	private Text m_txtTitle;
	private Text m_txtDate;

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
		this.m_isEditMode = isEditMode;

		//find reference
        m_canvas = GetComponent<CanvasGroup>();
        m_formItem =  GetComponent<HAGOUIFormItemStatusView>();
		m_btnDate = transform.Find("BtnDate").GetComponent<Button>();
		m_txtDate = transform.Find("BtnDate/Text").GetComponent<Text>();
        m_txtTitle = transform.Find("BtnDate/TxtTitle").GetComponent<Text>();
		
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
			SetDate(m_data.Value);
		}
		else //handle fixed UI value
		{
			SetDate(DateTime.Today);
		}

		//add listener
		m_btnDate.onClick.AddListener(DateOnClick);

		m_isInitComplete = true;
	}

	private void DateOnClick()
	{
		HAGODateTimePickerManager.Api.InitSingleDatePicker(SetDate);
	}

	public void SetDate(DateTime date)
	{
		if (date == null)
		{
			return;
		}

		m_value = date.Date;
		SetTextDate();
	}

	private void SetTextDate()
	{
		m_txtDate.text = m_value.ToString(HAGOConstant.FORMAT_DATE);
	}

	public void ActiveError()
    {
        m_formItem.ActiveError();
    }

	public void ResetError()
	{
		m_formItem.ResetError();
	}

	public DateTime GetValue()
	{
		return m_value.Date;
	}

	public long GetID()
    {
        return m_data != null ? m_data.ID : transform.GetSiblingIndex();
    }

	public string GetKeyForm()
	{
		if(m_formItem == null)
		{
			m_formItem = GetComponent<HAGOUIFormItemStatusView>();
		}

		return m_formItem != null ? m_formItem.keyItem : string.Empty;
	}

	public string GetJsonValue()
	{
		return HAGOUtils.GetEpochTimeFromDateTime(m_value.Date).ToString();
	}
	
	public object ExportView(int id)
	{
		if(m_txtTitle == null)
        {
            m_txtTitle = transform.Find("BtnDate/TxtTitle").GetComponent<Text>();
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
        return HAGOServiceKey.PARAM_DATE_COMPONENT;
    }

    public void SetValue(string value)
    {
		try
		{
			long dateEpoch = long.Parse(value);
			DateTime date = HAGOUtils.GetDateTimeFromEpoch(dateEpoch);

			SetDate(date);
		}
		catch(Exception ex)
		{
			Debug.Log("[HADateComponentView] Cannot parse: " + ex.ToString());
		}
    }
}
