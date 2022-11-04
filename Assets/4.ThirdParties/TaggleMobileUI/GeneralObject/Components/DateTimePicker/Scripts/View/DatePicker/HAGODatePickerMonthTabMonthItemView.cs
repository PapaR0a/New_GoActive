using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HAGODatePickerMonthTabMonthItemView : MonoBehaviour
{
	private Button m_btnMonth;
	private Text m_txtLabel;

	//param
	private int m_month;

	//const
	private Color CONST_COLOR_TEXT_SELECTED = new Color(33f / 255f, 188f / 255f, 154f / 255f, 255f / 255f);
	private Color CONST_COLOR_TEXT_NORMAL = new Color(66f / 255f, 66f / 255f, 66f / 255f, 255f / 255f);

	void OnDestroy ()
	{
		Destroy ();
	}

	public void Destroy ()
	{
		//unregister event
		HAGODateTimePickerControl.Api.OnMonthSelectedEvent -= OnMonthSelectedHandler;
	}

	/// <summary>
    /// Initialization view
    /// </summary>
	public void Init (int month)
	{
		m_month = month;

		// find reference
		m_btnMonth = GetComponent<Button>();
		m_txtLabel = transform.Find("TxtMonth").GetComponent<Text>();

		// add listener
		m_btnMonth.onClick.AddListener(MonthItemOnClick);

		//register event
		HAGODateTimePickerControl.Api.OnMonthSelectedEvent += OnMonthSelectedHandler;
	}

	/// <summary>
    /// Handle click on item button
    /// </summary>
	private void MonthItemOnClick ()
	{
		HAGODateTimePickerControl.Api.MonthSelected(m_month);
	}

	/// <summary>
    /// Handler month selected
    /// </summary>
	private void OnMonthSelectedHandler (int month)
	{
		SetColorItemSelected(m_month == month);
	}

	/// <summary>
    /// Set color item when it selected or not
    /// </summary>
	public void SetColorItemSelected (bool isSelected)
	{
		if(m_txtLabel != null)
		{
			m_txtLabel.color = isSelected ? CONST_COLOR_TEXT_SELECTED : CONST_COLOR_TEXT_NORMAL;
		}
	}
}
