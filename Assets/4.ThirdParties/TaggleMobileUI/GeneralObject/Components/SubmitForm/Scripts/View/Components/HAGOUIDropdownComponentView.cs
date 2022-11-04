using System;
using System.Collections;
using System.Collections.Generic;
using Honeti;
using UnityEngine;
using UnityEngine.UI;

public class HAGOUIDropdownComponentView : MonoBehaviour, HAGOUIIComponent
{
	public bool isInitByUser = false; //using for dynamic init
    [Space(12)] //blanck space on inspector

	private CanvasGroup m_canvas;
	private HAGOUIFormItemStatusView m_formItem;
	private Text m_txtTitle;
	private Dropdown m_ddControl;

	//param
	private HAGOUIDropdownDTO m_data;
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

		this.m_data = (HAGOUIDropdownDTO)data;
		this.m_isEditMode = isEditMode;

		//find reference
        m_canvas = GetComponent<CanvasGroup>();
        m_formItem = GetComponent<HAGOUIFormItemStatusView>();
        m_txtTitle = transform.Find("Control/TxtTitle").GetComponent<Text>();
		m_ddControl = transform.Find("Control").GetComponent<Dropdown>();
		
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
			InitDynamicItems();
		}
		else //handle fixed UI value
		{
			m_ddControl.RefreshShownValue();
		}

		m_isInitComplete = true;
	}

	private void InitDynamicItems()
	{
		StartCoroutine(IECreateItem());
	}

	private IEnumerator IECreateItem()
    {
		//clear old items
		m_ddControl.ClearOptions();

        foreach(HAGOUIDropdownOptionDTO option in m_data.Options)
        {
            if(!string.IsNullOrEmpty(option.Icon))
            {
                Texture2D texture = null;
                ResourceUtils.DownloadTextureURL(option.Icon, (rs) => {
                    texture = rs;
                });

                yield return new WaitUntil(()=> texture != null);

                Sprite sprIcon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f,0.5f));
                m_ddControl.options.Add(new Dropdown.OptionData(option.Name, sprIcon));
            }
            else
            {
                m_ddControl.options.Add(new Dropdown.OptionData(option.Name, null));
            }
        }

        yield return new WaitForEndOfFrame();

		SetOption(m_data.DefaultValueIndex);
    }

	public void SetOption(int index)
	{
		m_ddControl.value = index;
        m_ddControl.RefreshShownValue();
	}

	public void ActiveError()
    {
        m_formItem.ActiveError();
    }

	public void ResetError()
	{
		m_formItem.ResetError();
	}

	public long GetValue()
	{
		return m_data.Options[m_ddControl.value].ID;
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
		return m_data.Options[m_ddControl.value].ID.ToString();
	}

	public long GetID()
    {
        return m_data != null ? m_data.ID : transform.GetSiblingIndex();
    }

	public object ExportView(int id)
	{
		List<HAGOUIDropdownOptionDTO> options = new List<HAGOUIDropdownOptionDTO>();

		if(m_ddControl == null || m_txtTitle == null)
		{
			m_ddControl = transform.Find("Control").GetComponent<Dropdown>();
			m_txtTitle = transform.Find("Control/TxtTitle").GetComponent<Text>();
		}
		
		int childID = 1;
		foreach(Dropdown.OptionData optData in m_ddControl.options)
		{
			HAGOUIDropdownOptionDTO dto = new HAGOUIDropdownOptionDTO(
				childID, 
			 	"#" + optData.text, 
				""
			);
			options.Add(dto);

			childID++;
		}

		return new HAGOUIDropdownDTO(
			id,
			m_txtTitle.text,
			GetKeyForm(),
			options,
			m_ddControl.value
		);
	}

    public string GetFormType()
    {
        return HAGOServiceKey.PARAM_DROPDOWN_COMPONENT;
    }

	public void SetValue(string value)
    {
		try
		{
			int option = int.Parse(value);
			SetOption(option);
		}
		catch(Exception ex)
		{
			Debug.Log("[HADropdownComponentView] Cannot parse: " + ex.ToString());
		}
    }
}
