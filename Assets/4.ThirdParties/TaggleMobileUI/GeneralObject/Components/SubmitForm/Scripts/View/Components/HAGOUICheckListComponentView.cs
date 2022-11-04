using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class HAGOUICheckListComponentView : MonoBehaviour, HAGOUIIComponent
{
	public bool isInitByUser = false; //using for dynamic init
    [Space(12)] //blanck space on inspector

	private CanvasGroup m_canvas;
	private ToggleGroup m_tglGroup;
	private HAGOUIFormItemStatusView m_formItem;
	private Text m_txtTitle;
	private Transform m_content;
	//
	private GameObject m_prefItem;

	//param
	private HAGOUIToggleListDTO m_data;
	private bool m_isInitComplete = false;
	private bool m_isEditMode;
	
	void Awake()
	{
		//prevent child auto init by default
		m_prefItem = transform.Find("Content/TglItem").gameObject;
		foreach(HAGOUIToggleComponentView view in transform.Find("Content").GetComponentsInChildren<HAGOUIToggleComponentView>())
		{
			if(view.gameObject == m_prefItem)
			{
				continue;
			}

			Debug.Log("=== Init " + view.gameObject.name);
			HAGOUIToggleComponentView itemView = view.GetComponent<HAGOUIToggleComponentView>();
			itemView.isInitByUser = true;
		}
	}

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

		m_data = (HAGOUIToggleListDTO)data;
		m_isEditMode = isEditMode;

		//find reference
        m_canvas = GetComponent<CanvasGroup>();
        m_formItem = GetComponent<HAGOUIFormItemStatusView>();
		m_tglGroup = GetComponent<ToggleGroup>();
		m_content = transform.Find("Content");
        m_txtTitle = transform.Find("Title/TxtTitle").GetComponent<Text>();
		
		m_canvas.interactable = m_isEditMode;
		//
		if(this.m_data != null) //handle dynamic UI value
        {
			//find reference dynamic UI
			m_prefItem = transform.Find("Content/TglItem").gameObject;
			m_prefItem.SetActive(false);

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
			InitFixedItems();
		}

		m_isInitComplete = true;
	}

	private void InitDynamicItems()
	{
		//clear old items
		ClearItems();

		int itemIndex = 0;
		foreach(HAGOUIToggleOptionDTO optionData in m_data.Options)
		{
			GameObject go = Instantiate(m_prefItem, m_content);
			HAGOUIToggleComponentView itemView = go.GetComponent<HAGOUIToggleComponentView>();

			itemView.Init(optionData, m_isEditMode);

			itemIndex++;
		}
	}

	private void InitFixedItems()
	{
		foreach(Transform tf in m_content)
		{
			HAGOUIToggleComponentView itemView = tf.GetComponent<HAGOUIToggleComponentView>();
			itemView.Init(null, m_isEditMode);
		}
	}

	private void ClearItems()
	{
		foreach(Transform tf in m_content)
		{
			if(tf.gameObject == m_prefItem)
			{
				continue;
			}

			if(Application.isPlaying)
			{
				Destroy(tf.gameObject);
			}
			else
			{
				DestroyImmediate(tf.gameObject);
			}
		}
	}

	public void ActiveError()
    {
        m_formItem.ActiveError();
    }

	public void ResetError()
	{
		m_formItem.ResetError();
	}

	public List<long> GetValue()
	{
		List<long> rs = new List<long>();

		foreach(Transform tf in m_content)
		{
			if(tf.gameObject == m_prefItem)
			{
				continue;
			}

			HAGOUIToggleComponentView itemView = tf.GetComponent<HAGOUIToggleComponentView>();

			if(itemView.GetValue())
			{
				rs.Add(itemView.GetID());
			}
		}

		return rs;
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
		return JsonConvert.SerializeObject(GetValue());
	}

	public object ExportView(int id)
	{
		List<HAGOUIToggleOptionDTO> options = new List<HAGOUIToggleOptionDTO>();

		if(m_content == null || m_txtTitle == null)
		{
			m_content = transform.Find("Content");
			m_txtTitle = transform.Find("Title/TxtTitle").GetComponent<Text>();
		}
		
		int childID = 1;
		foreach(Transform tf in m_content)
		{
			if(tf.gameObject == m_prefItem)
			{
				continue;
			}

			HAGOUIToggleComponentView itemView = tf.GetComponent<HAGOUIToggleComponentView>();
			HAGOUIToggleOptionDTO dto = (HAGOUIToggleOptionDTO)itemView.ExportView(childID);

			if(dto != null)
			{
				options.Add(dto);
			}

			childID++;
		}

		return new HAGOUIToggleListDTO(id, m_txtTitle.text, GetKeyForm(), options);
	}

    public string GetFormType()
    {
        return HAGOServiceKey.PARAM_CHECK_LIST_COMPONENT;
    }

    public void SetValue(string value)
    {
        //TODO: handle later
    }
}