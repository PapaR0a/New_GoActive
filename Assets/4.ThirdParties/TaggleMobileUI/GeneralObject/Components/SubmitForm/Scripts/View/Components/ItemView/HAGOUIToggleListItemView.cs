using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Note: only use in HAToggleListComponent so only init via script in HAToggleListComponent, not using isInitByUser = false like others
public class HAGOUIToggleListItemView : MonoBehaviour
{
	private Toggle m_tglControl;
	private Text m_txtTitle;
	
	//param
	public HAGOUIToggleOptionDTO data;
    private bool m_isInitComplete = false;
	private bool m_isEditMode;

	public void Init(HAGOUIToggleOptionDTO data, bool isEditMode)
	{
		if(m_isInitComplete)
        {
            Debug.LogError(this.GetType().Name + " already init!");
            return;
        }

		this.data = data;
		this.m_isEditMode = isEditMode;

		//find reference
		m_tglControl = GetComponent<Toggle>();
		m_txtTitle = transform.Find("TxtTitle")?.GetComponent<Text>() ?? null;
		m_tglControl.interactable = m_isEditMode;

		if(this.data != null) //handle dynamic UI value
		{
			//set title
            if(m_txtTitle != null)
            {
                m_txtTitle.text = this.data.Title;
            }
			//set toggle is on or off
            if(m_tglControl != null)
            {
                m_tglControl.isOn = this.data.DefaulValue;
            }

			//active gameobject because prefItem instantiate is disable by default
			gameObject.SetActive(true);
		}

		m_isInitComplete = true;
	}

	public void SetGroup(ToggleGroup group)
	{
		m_tglControl.group = group;
	}

	public void SetValue(bool isOn)
	{
		m_tglControl.isOn = isOn;
	}

	public long GetID()
    {
        return data != null ? data.ID : transform.GetSiblingIndex();
    }

    public bool GetValue()
    {
        return m_tglControl.isOn;
    }
	
	public HAGOUIToggleOptionDTO ExportView(int id)
	{
		if(m_tglControl == null || m_txtTitle == null)
		{
			m_tglControl = GetComponent<Toggle>();
			m_txtTitle = transform.Find("TxtTitle")?.GetComponent<Text>();
		}

		return new HAGOUIToggleOptionDTO(
            id,
            m_txtTitle.text,
            string.Empty,
            GetValue()
        );
	}
}
