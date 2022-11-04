using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class HAGOUIAttachmentComponentView : MonoBehaviour, HAGOUIIComponent
{
	public bool isInitByUser = false; //using for dynamic init
    [Space(12)] //blanck space on inspector
	//
	private CanvasGroup m_canvas;
	private Text m_txtTitle;
	private HAGOUIFormItemStatusView m_formItem;
	private Transform m_content;
	private GameObject m_buttonBar;
	private Button m_btnAddImage;
	private Button m_btnAddAudio;
	//
	private GameObject m_prefItem;

	//param
	private HAGOUIAttachmentDTO m_data;
	private bool m_isInitComplete = false;
	private bool m_isEditMode;

	void Start()
	{
		if(!isInitByUser)
        {
			InitView(true);
        }
	}

	public void Init(object data, bool isEditMode)
	{
		m_data = (HAGOUIAttachmentDTO)data;

		InitView(isEditMode);
	}

	private void InitView(bool isEditMode)
	{
		m_isEditMode = isEditMode;

		if(!m_isInitComplete)
        {
			m_isInitComplete = true;
			
            //find reference
			m_canvas = GetComponent<CanvasGroup>();
			m_formItem = GetComponent<HAGOUIFormItemStatusView>();
			m_txtTitle = transform.Find("TxtTitle").GetComponent<Text>();
			m_content = transform.Find("Detail/Viewport/Content");
			m_buttonBar = transform.Find("Buttons").gameObject;
			m_btnAddImage = transform.Find("Buttons/BtnImage").GetComponent<Button>();
			m_btnAddAudio = transform.Find("Buttons/BtnAudio").GetComponent<Button>();

			//update view
			m_prefItem = transform.Find("Detail/Viewport/Content/Item").gameObject;
			m_prefItem.SetActive(false);
			//
			m_btnAddAudio.gameObject.SetActive(false); //TODO: handle show AudioPicker later

			//add listener
			m_btnAddImage.onClick.AddListener(AddImageOnClick);
			m_btnAddAudio.onClick.AddListener(AddAudioOnClick);
        }

		//handle value
		m_buttonBar.gameObject.SetActive(m_isEditMode);

		if(m_data != null)
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

			InitItemsValue();
		}
	}

	private void OnAddImageHandler(Texture2D texture)
	{
		HAGOUIAttachmentItemDTO data = new HAGOUIAttachmentItemDTO(m_content.childCount, HAGOUIAttachmentType.Image, texture);
		CreateItem(data);
	}

	private void OnAddAudioHandler(AudioClip clip)
	{
		HAGOUIAttachmentItemDTO data = new HAGOUIAttachmentItemDTO(m_content.childCount, HAGOUIAttachmentType.Audio, clip);
		CreateItem(data);
	}

	private void CreateItem(HAGOUIAttachmentItemDTO data)
	{
		GameObject go = Instantiate(m_prefItem, m_content);
		go.SetActive(true);

		go.GetComponent<HAGOUIAttachmentItemView>().Init(m_isEditMode, data);
	}

	private void InitItemsValue()
	{
		foreach(HAGOUIAttachmentItemDTO dto in m_data.Data)
		{
			CreateItem(dto);
		}
	}

    private void AddAudioOnClick()
    {
		//TODO: show audio picker & record later
		// HAControl.Api.ShowAudioRecord(OnAddAudioHandler);
    }

    private void AddImageOnClick()
    {
		HAGONativeGalleryControl.Api.PickImage(OnAddImageHandler);
    }

	private long GetID()
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
		List<string> values = new List<string>();

		foreach(Transform tf in m_content)
		{
			if(tf.gameObject != m_prefItem)
			{
				values.Add(tf.GetComponent<HAGOUIAttachmentItemView>().GetJsonValue());
			}
		}

		return JsonConvert.SerializeObject(values);
	}

	public object ExportView(int id)
	{
		Debug.Log("ExportView " + this.name);

		if(m_txtTitle == null)
        {
            m_txtTitle = transform.Find("TxtTitle").GetComponent<Text>();
        }

		return new HAGOUIAttachmentDTO(
			id,
			m_txtTitle.text,
			GetKeyForm(),
			null
		);
	}

    public string GetFormType()
    {
        return HAGOServiceKey.PARAM_ATTACHMENT_COMPONENT;
    }

    public void ActiveError()
    {
        m_formItem.ActiveError();
    }

	public void ResetError()
	{
		m_formItem.ResetError();
	}

    public void SetValue(string value)
    {
        //TODO: handle later
    }
}
