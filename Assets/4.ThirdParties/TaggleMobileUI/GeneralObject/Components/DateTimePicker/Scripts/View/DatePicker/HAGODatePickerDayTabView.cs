using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Honeti;
using UnityEngine;
using UnityEngine.UI;

public class HAGODatePickerDayTabView : MonoBehaviour
{
	private Text m_txtMonthTitle;
	private Transform m_dayContent;
	//
    private GameObject m_prefDayItem;

    //param
    private bool m_isMultiSelect;
    private Dictionary<DateTime,bool> m_dateMarkData; // list dates have circle mark icon
	
	/// <summary>
    /// Initialization view
    /// </summary>
	public void Init (bool isMultiSelect, Dictionary<DateTime,bool> dateMarkData)
	{
        m_isMultiSelect = isMultiSelect;
        m_dateMarkData = dateMarkData;

		// find reference
		m_dayContent = transform.Find("Content");
		m_txtMonthTitle = transform.Find("TxtCurMonth").GetComponent<Text>();
		//
		m_prefDayItem = transform.Find("Content/DayItem").gameObject;
	}
	
	/// <summary>
    /// Notify day change handler
    /// </summary>
	public void OnDateChangeHandler (int month , int year, List<DateTime> lstDateSelected)
    {
		List<DateTime> dates = new List<DateTime>();

        DateTime startDate = GetFirstDayOfWeek(new DateTime(year, month, 1));
        DateTime lastDate = GetLastDayOfMonth(year, month);

        while (startDate <= lastDate)
        {
            dates.Add(startDate);
            startDate = startDate.AddDays(1);
        }

        if (dates.Count < 28) //min days
        {
            return;
        }

        // set current month title
        SetTextMonthTitle(month);

        // destroy all old items
		foreach(Transform tf in m_dayContent.transform)
		{
			if(tf.gameObject == m_prefDayItem)
			{
				continue;
			}

			Destroy(tf.gameObject);
		}

        foreach (DateTime date in dates)
        {
            bool isMark = m_dateMarkData?.ContainsKey(date.Date) ?? false;

            // instantiate button
            GameObject go = Instantiate(m_prefDayItem, m_dayContent);
			HAGODatePickerDayTabDayItemView itemView = go.GetComponent<HAGODatePickerDayTabDayItemView>();

            itemView.Init(m_isMultiSelect, date, month, isMark);
			go.SetActive(true);

            if(IsDateSelected(date, lstDateSelected))
            {
                itemView.SetColorItemSelected(true);
            }
		}
	}

	/// <summary>
    /// Return if current date is in selected list
    /// </summary>
	private bool IsDateSelected(DateTime date, List<DateTime> lstDateSelected)
	{
        foreach(DateTime dateSelected in lstDateSelected)
        {
            if(date.Date == dateSelected.Date)
            {
                return true;
            }
        }

        return false;
	}

	/// <summary>
    /// Returns the first day of next month.
    /// </summary>
    private DateTime GetFirstDayOfNextMonth (int year, int month)
    {
        return new DateTime(year, month, 1).AddMonths(1);
    }

    /// <summary>
    /// Returns the last day of current month.
    /// </summary>
    private DateTime GetLastDayOfMonth (int year, int month)
    {
        return GetFirstDayOfNextMonth(year, month).AddDays(-1);
    }

    /// <summary>
    /// Returns the first day of the week that the specified
    /// date is in using the current culture. 
    /// </summary>
    private DateTime GetFirstDayOfWeek (DateTime _dayInWeek)
    {
        CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
        return GetFirstDateOfWeek(_dayInWeek, defaultCultureInfo);
    }

    /// <summary>
    /// Returns the first day of the week that the specified date 
    /// is in. 
    /// </summary>
    private DateTime GetFirstDateOfWeek (DateTime dayInWeek, CultureInfo cultureInfo)
    {
        DayOfWeek firstDay = DayOfWeek.Monday;
        DateTime firstDayInWeek = dayInWeek.Date;

        while (firstDayInWeek.DayOfWeek != firstDay)
        {
            firstDayInWeek = firstDayInWeek.AddDays(-1);
        }

        // Debug.Log("Culture info: " + cultureInfo.ToString());
        return firstDayInWeek;
    }

	/// <summary>
    /// Set current month title
    /// </summary>
    private void SetTextMonthTitle (int month)
    {
        m_txtMonthTitle.text = HAGOUtils.GetMonthName(month);
    }
}
