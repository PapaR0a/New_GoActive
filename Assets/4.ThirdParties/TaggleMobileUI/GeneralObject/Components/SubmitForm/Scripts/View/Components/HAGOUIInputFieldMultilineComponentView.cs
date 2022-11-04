using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HAGOUIInputFieldMultilineComponentView : MonoBehaviour, HAGOUIIComponent
{
	public bool isInitByUser = false; //using for dynamic init
    [Space(12)] //blanck space on inspector

	private CanvasGroup m_canvas;
	private HAGOUIFormItemStatusView m_formItem;
	private Text m_txtPlaceholder;
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
		m_txtTitle = transform.Find("TxtTitle").GetComponent<Text>();
		m_txtPlaceholder = transform.Find("IpfContent/Placeholder").GetComponent<Text>();

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
		}

		m_isInitComplete = true;
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
			m_txtPlaceholder = transform.Find("IpfContent/Placeholder").GetComponent<Text>();
			m_txtTitle = transform.Find("TxtTitle").GetComponent<Text>();
		}

		return new HAGOUIInputFieldDTO(
			id,
			m_txtTitle.text,
			"#" + m_txtPlaceholder.text,
			GetValue(),
			string.Empty,
			HAGOUIInputFieldContentType.Standard,
			GetKeyForm()
		);
	}

    public string GetFormType()
    {
        return HAGOServiceKey.PARAM_INPUTFIELD_MULTILINE_COMPONENT;
    }
}
