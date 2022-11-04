using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HAGODatePickerMonthTabView : MonoBehaviour
{
	private Text m_txtYearTitle;

    //param
    public int monthSelected;
	
    void OnDestroy ()
    {
        Destroy();
    }

    /// <summary>
    /// Unregister event when gameobject destroy
    /// </summary>
    public void Destroy ()
    {
        //register event
        for (int month = 1; month <= 12; month++)
        {
            GameObject goBtn = transform.Find(string.Format("Content/BtnMonth ({0})", month)).gameObject;
            HAGODatePickerMonthTabMonthItemView itemView = goBtn.GetComponent<HAGODatePickerMonthTabMonthItemView>();

            itemView.Destroy();
        }
        //
		HAGODateTimePickerControl.Api.OnMonthSelectedEvent -= OnMonthSelectedHandler;
        HAGODateTimePickerControl.Api.OnYearSelectedEvent -= OnYearSelectedHandler;
    }

	/// <summary>
    /// Initialization view
    /// </summary>
	public void Init (int monthDefault, int year)
	{
        monthSelected = monthDefault;

		//find reference
		m_txtYearTitle = transform.Find("TxtCurYear").GetComponent<Text>();
		//
		for (int month = 1; month <= 12; month++)
        {
            GameObject goBtn = transform.Find(string.Format("Content/BtnMonth ({0})", month)).gameObject;
            HAGODatePickerMonthTabMonthItemView itemView = goBtn.GetComponent<HAGODatePickerMonthTabMonthItemView>();

            itemView.Init(month);
			goBtn.SetActive(true);
            
            if(month == monthSelected)
			{
				itemView.SetColorItemSelected(true);
			}
        }

		SetTextYearTitle(year);

        //register event
		HAGODateTimePickerControl.Api.OnMonthSelectedEvent += OnMonthSelectedHandler;
        HAGODateTimePickerControl.Api.OnYearSelectedEvent += OnYearSelectedHandler;
	}

	/// <summary>
    /// Set current year title
    /// </summary>
    private void SetTextYearTitle (int year)
    {
        m_txtYearTitle.text = year.ToString();
    }

    /// <summary>
    /// Handler month selected
    /// </summary>
	private void OnMonthSelectedHandler (int month)
	{
		monthSelected = month;
	}

    /// <summary>
    /// Handler year selected
    /// </summary>
	private void OnYearSelectedHandler (int year)
	{
		SetTextYearTitle(year);
	}
}
