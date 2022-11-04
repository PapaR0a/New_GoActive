using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HAGODatePickerDayTabDayItemView : MonoBehaviour
{
	private Button m_btnDay;
	private Text m_txtLabel;
	private GameObject m_selectedObj;
	private Image m_imgMark; 

	//param
	private DateTime m_date;
	private int m_curMonth;
	private bool m_isMultiSelect;
	private bool m_isMark;

	//const
	private Color CONST_COLOR_TEXT_SELECTED = Color.white;
   	private Color CONST_COLOR_TEXT_NORMAL = new Color(50f / 255f, 50f / 255f, 50f / 255f, 255f / 255f);
	private Color CONST_COLOR_MARK_SELECTED = Color.white;
	private Color CONST_COLOR_MARK_NORMAL = new Color(35f / 255f, 177f / 255f, 136f / 255f, 255f / 255f);

    void OnDestroy ()
	{
		//unregister event
		HAGODateTimePickerControl.Api.OnDateSelectedEvent -= OnDateSelectedHandler;
		HAGODateTimePickerControl.Api.OnDateDeselectedEvent -= OnDateDeselectedHandler;
	}
    
	/// <summary>
    /// Initialization view
    /// </summary>
	public void Init (bool isMultiSelect, DateTime date, int curMonth, bool isMark)
	{
		m_date = date;
		m_curMonth = curMonth;
		m_isMultiSelect = isMultiSelect;
		m_isMark = isMark;

		// find reference
		m_btnDay = GetComponent<Button>();
		m_txtLabel = transform.Find("TxtDay").GetComponent<Text>();
		m_selectedObj = transform.Find("ImgSelected").gameObject;
		m_imgMark = transform.Find("ImgMark").GetComponent<Image>();

		// set value
		m_selectedObj.SetActive(false);
		m_imgMark.gameObject.SetActive(m_isMark);
		m_txtLabel.text = date.Day.ToString();
		m_txtLabel.color = new Color(CONST_COLOR_TEXT_NORMAL.r, CONST_COLOR_TEXT_NORMAL.g, CONST_COLOR_TEXT_NORMAL.b, m_date.Month == m_curMonth ? 1f : 0.5f);

		// add listener
		m_btnDay.onClick.AddListener(DayItemOnClick);

		//register event
		HAGODateTimePickerControl.Api.OnDateSelectedEvent += OnDateSelectedHandler;
		HAGODateTimePickerControl.Api.OnDateDeselectedEvent += OnDateDeselectedHandler;
    }

	/// <summary>
    /// Handle click on item button
    /// </summary>
	private void DayItemOnClick ()
	{
		if(IsSelected() && m_isMultiSelect)
		{
			HAGODateTimePickerControl.Api.DateDeselected(m_date);
		}
		else
		{		
			HAGODateTimePickerControl.Api.DateSelected(m_date);
		}
	}
	
	/// <summary>
    /// Handler date selected
    /// </summary>
	private void OnDateSelectedHandler (DateTime date)
	{
		if(m_isMultiSelect && m_date.Date != date.Date)
		{
			return;
		}

		SetColorItemSelected(m_date.Date == date.Date);
	}
	
	/// <summary>
    /// Handler date deselected
    /// </summary>
	private void OnDateDeselectedHandler (DateTime date)
	{
		if(m_date.Date != date.Date)
		{
			return;
		}
		
		SetColorItemSelected(m_date.Date != date.Date);
	}

	/// <summary>
    /// Return if item is selected or not
    /// </summary>
	private bool IsSelected()
	{
		return m_selectedObj.activeSelf;
	}

	/// <summary>
    /// Set color item when it selected or not
    /// </summary>
	public void SetColorItemSelected (bool isSelected)
	{
		m_selectedObj.SetActive(isSelected);
		m_txtLabel.color = isSelected ? CONST_COLOR_TEXT_SELECTED : new Color(CONST_COLOR_TEXT_NORMAL.r, CONST_COLOR_TEXT_NORMAL.g, CONST_COLOR_TEXT_NORMAL.b, m_date.Month == m_curMonth ? 1f : 0.5f);
		m_imgMark.color = isSelected ? CONST_COLOR_MARK_SELECTED : CONST_COLOR_MARK_NORMAL;
	}
}
