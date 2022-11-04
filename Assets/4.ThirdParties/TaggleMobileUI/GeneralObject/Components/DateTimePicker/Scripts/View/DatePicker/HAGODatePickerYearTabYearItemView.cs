using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HAGODatePickerYearTabYearItemView : MonoBehaviour
{
	private Button m_btnYear;
	private Text m_txtLabel;

	//param
	private int m_year;

	//const
	private Color CONST_COLOR_TEXT_SELECTED = new Color(33f / 255f, 188f / 255f, 154f / 255f, 255f / 255f);
	private Color CONST_COLOR_TEXT_NORMAL = new Color(66f / 255f, 66f / 255f, 66f / 255f, 255f / 255f);

	void OnDestroy ()
	{
		Destroy();
	}

	public void Destroy()
	{
		//register event
		HAGODateTimePickerControl.Api.OnYearSelectedEvent -= OnYearSelectedHandler;
	}

	/// <summary>
    /// Initialization view
    /// </summary>
	public void Init (int year)
	{
		m_year = year;

		//find reference
		m_btnYear = GetComponent<Button>();
		m_txtLabel = transform.Find("TxtYear").GetComponent<Text>();

		//handle value
		m_txtLabel.text = year.ToString();

		//add listener
		m_btnYear.onClick.AddListener(YearItemOnClick);
		
		//register event
		HAGODateTimePickerControl.Api.OnYearSelectedEvent += OnYearSelectedHandler;
	}

	/// <summary>
    /// Handle click on item button
    /// </summary>
	private void YearItemOnClick ()
	{
		HAGODateTimePickerControl.Api.YearSelected(m_year);
	}

	/// <summary>
    /// Handler year selected
    /// </summary>
	private void OnYearSelectedHandler (int year)
	{
		SetColorItemSelected(m_year == year);
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
