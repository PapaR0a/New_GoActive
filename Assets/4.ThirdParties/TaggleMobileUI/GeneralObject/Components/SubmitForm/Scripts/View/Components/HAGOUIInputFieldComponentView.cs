using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HAGOUIInputFieldComponentView : MonoBehaviour, HAGOUIIComponent
{
	public bool isInitByUser = false; //using for dynamic init
	public string unit = string.Empty; // unit if vsm data
	[SerializeField]
	public HAGOUIInputFieldContentType contentType; // content type inputfield
	public float minValue = -1; // min value validate for non standard content type
	public float maxValue = -1; // max value validate for non standard content type
	public int minLength = -1; // min lenght validate for non standard content type
	public int maxLength = -1; // max lenght validate for non standard content type
    [Space(12)] //blanck space on inspector

	private CanvasGroup m_canvas;
	private HAGOUIFormItemStatusView m_formItem;
	private Text m_txtPlaceholder;
	private Text m_txtUnit;
	private Text m_txtTitle;
	private InputField m_ipfContent;

	//param
	private HAGOUIInputFieldDTO m_data;
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

		m_data = (HAGOUIInputFieldDTO)data;
		m_isEditMode = isEditMode;

		//find reference
        m_canvas = GetComponent<CanvasGroup>();
        m_formItem = GetComponent<HAGOUIFormItemStatusView>();
		m_ipfContent = transform.Find("IpfContent").GetComponent<InputField>();
		m_txtTitle = transform.Find("IpfContent/TxtTitle").GetComponent<Text>();
		m_txtPlaceholder = transform.Find("IpfContent/Text/Placeholder").GetComponent<Text>();
		m_txtUnit = transform.Find("IpfContent/TxtUnit").GetComponent<Text>();

		m_canvas.interactable = m_isEditMode;
		//
		if(m_data != null) //handle dynamic UI value
        {
			//set title
			if(m_txtTitle != null) 
			{
				m_txtTitle.text = m_data.Title;
			}
			//set content
			if(m_ipfContent != null)
			{
				SetValue(m_data.Content);
				//
				this.contentType = m_data.ContentType;
				UpdateContentType(this.contentType);
			}
			//set placeholder
			if(m_txtPlaceholder != null)
			{
				m_txtPlaceholder.text = m_data.Placeholder;
			}
			//set key form response error
			if(m_formItem != null)
			{
				m_formItem.keyItem = this.m_data.KeyForm;
			}

			minValue = m_data.MinValue;
			maxValue = m_data.MaxValue;
			minLength = m_data.MinLength;
			maxLength  = m_data.MaxLength;

			unit = m_data.Unit;
		}
		else
		{
			UpdateContentType(this.contentType);
		}

		//set label unit if available
		m_txtUnit.text = unit;

		m_isInitComplete = true;
	}

    private void UpdateContentType(HAGOUIInputFieldContentType type)
	{
		m_ipfContent.contentType = type == HAGOUIInputFieldContentType.Integer ? InputField.ContentType.IntegerNumber : 
									type == HAGOUIInputFieldContentType.Decimal ? InputField.ContentType.DecimalNumber :
										InputField.ContentType.Standard;
	}

	public void SetValue(string content)
	{
		m_ipfContent.text = content;
	}

	public string GetValue()
	{
		return m_ipfContent.text;
	}

	public long GetID()
    {
        return m_data != null ? m_data.ID : transform.GetSiblingIndex();
    }

	public void ActiveError()
    {
        m_formItem.ActiveError();
    }
	
	public void ResetError()
	{
		m_formItem.ResetError();
	}

	public object ExportView(int id)
	{
		if(m_txtPlaceholder == null || m_ipfContent == null || m_formItem == null)
		{
			m_formItem = GetComponent<HAGOUIFormItemStatusView>();
			m_ipfContent = transform.Find("IpfContent").GetComponent<InputField>();
			m_txtPlaceholder = transform.Find("IpfContent/Text/Placeholder").GetComponent<Text>();
			m_txtTitle = transform.Find("IpfContent/TxtTitle").GetComponent<Text>();
		}

		return new HAGOUIInputFieldDTO(
			id,
			m_txtTitle.text,
			"#" + m_txtPlaceholder.text,
			GetValue(),
			unit,
			contentType,
			GetKeyForm(),
			minValue,
			maxValue,
			minLength,
			maxLength
		);
	}

    public string GetFormType()
    {
        return HAGOServiceKey.PARAM_INPUTFIELD_COMPONENT;
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
		return m_ipfContent.text;
	}

    public bool CheckValid()
    {
        switch(contentType)
        {
            case HAGOUIInputFieldContentType.Standard:
				if(!IsValidMinLength() || !IsValidMaxLength())
				{
					return false;
				}
				break;

            case HAGOUIInputFieldContentType.Integer:
            case HAGOUIInputFieldContentType.Decimal:
				if(!IsValidMinLength() || !IsValidMaxLength() || !IsValidMinValue() || !IsValidMaxValue())
				{
					return false;
				}
				break;

            default:
                return false;
        }

		m_formItem.ResetError();
		return true;
    }

    public bool IsValidMinLength()
    {
        return minLength != -1 ? GetValue().Length >= minLength : true;
    }

    public bool IsValidMaxLength()
    {
        return maxLength != -1 ? GetValue().Length <= maxLength : true;
    }

    public bool IsValidMinValue()
    {
		string valueStr = GetValue();
		if(string.IsNullOrEmpty(valueStr))
		{
			return false;
		}

		// Debug.Log("IsValidMinValue");
        if(minValue != -1f)
        {
			// Debug.Log("IsValidMinValue " + int.Parse(valueStr) + " >= " +  (int)minValue);
            switch(contentType)
            {   
                case HAGOUIInputFieldContentType.Integer:
                    return int.Parse(valueStr) >= (int)minValue;

                case HAGOUIInputFieldContentType.Decimal:
                    return float.Parse(valueStr) >= minValue;

                default:
                    return false;
            }
        }
        else
        {
            return true;
        }
    }

    public bool IsValidMaxValue()
    {
		string valueStr = GetValue();
		if(string.IsNullOrEmpty(valueStr))
		{
			return false;
		}

        if(maxValue != -1f)
        {
            switch(contentType)
            {   
                case HAGOUIInputFieldContentType.Integer:
                    return int.Parse(valueStr) <= (int)maxValue;

                case HAGOUIInputFieldContentType.Decimal:
                    return float.Parse(valueStr) <= maxValue;

                default:
                    return false;
            }
        }
        else
        {
            return true;
        }
    }
}
