using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HAGODatePickerCoreView : MonoBehaviour
{
	private Button m_btnTabDay;
	private Button m_btnTabMonth;
	private Button m_btnTabYear;
	//
	private HAGODatePickerDayTabView m_dayView;
	private HAGODatePickerMonthTabView m_monthView;
	private HAGODatePickerYearTabView m_yearView;

	//param
	[HideInInspector]
	public bool isMultiSelect;
	private DateTime m_defaultSelectedDate = DateTime.Today;
	//
	private DateTime m_dateSelectedData; // return data for DatePicker single date result
	private Dictionary<DateTime,bool> m_dateMarkData; // list dates have circle mark icon
	//
	private List<DateTime> m_listDateSelectedData; // return data for DatePicker with multiple dates result
	private Action<List<DateTime>> m_onValueChangedMultipleDateSelectedEvent; // notify value changed in multiple date
	//
	public Action<DateTime> onResponseDateSelectedEvent; // response date selected
	private Action<List<DateTime>> m_onResponseMultipleDateSelectedEvent; // response list date selected

	//const
	private Color CONST_COLOR_TAB_ACTIVE = new Color(33f / 255f, 188f / 255f, 154f / 255f, 255f / 255f);
	private Color CONST_COLOR_TAB_NORMAL = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
	private Color CONST_COLOR_TAB_OUTLINE_ACTIVE = new Color(33f / 255f, 188f / 255f, 154f / 255f, 255f / 255f);
	private Color CONST_COLOR_TAB_OUTLINE_NORMAL = new Color(189 / 255f, 189 / 255f, 189 / 255f, 189 / 255f);
    private Color CONST_COLOR_TEXT_ACTIVE = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
   	private Color CONST_COLOR_TEXT_NORMAL = new Color(50f / 255f, 50f / 255f, 50f / 255f, 255f / 255f);

	//enum
    public enum DatePickerTabType
    {
        Day,
        Month,
        Year
    }

	void OnDestroy ()
	{
		Destroy();
	}

	public void Destroy ()
	{
		//unregister event
		m_monthView?.Destroy();
		m_yearView?.Destroy();
		//
		HAGODateTimePickerControl.Api.OnDateSelectedEvent -= OnDateSelectedHandler;
		HAGODateTimePickerControl.Api.OnDateDeselectedEvent -= OnDateDeselectedHandler;
		HAGODateTimePickerControl.Api.OnMonthSelectedEvent -= OnMonthSelectedHandler;
		HAGODateTimePickerControl.Api.OnYearSelectedEvent -= OnYearSelectedHandler;
	}

	/// <summary>
    /// Initialization single date picker
    /// </summary>
	///<param name = "onDateSelectedEvent">Return a list selected date after user selected day on calendar
	///or user call FinishSelectMultipleDate() if in single select mode</param>
	///<param name = "dateMarkData">Dates will have circle mark icon</param>
	public virtual void InitDatePicker(Action<DateTime> onDateSelectedEvent, DateTime selectedDate, Dictionary<DateTime,bool> dateMarkData = null)
	{
		//handle value
		isMultiSelect = false;
		m_defaultSelectedDate = selectedDate;
		m_dateMarkData = dateMarkData;

		//register event
		onResponseDateSelectedEvent = onDateSelectedEvent;
	}

	/// <summary>
    /// Initialization multiple date picker
    /// </summary>
	///<param name = "onValueChanged">Return a list selected dates after user selected day on calendar</param>
	///<param name = "onMultiDateSelectedEvent">Return a list selected dates after user call FinishSelectMultipleDate() if in multiple select mode</param>
	///<param name = "dateMarkData">Dates will have circle mark icon</param>
	public virtual void InitMultiDatePicker (Action<List<DateTime>> onValueChangedEvent, Action<List<DateTime>> onMultipleDateSelectedEvent, Dictionary<DateTime,bool> dateMarkData = null)
	{
		//handle value
		isMultiSelect = true;
		m_dateMarkData = dateMarkData;

		//register event
		m_onValueChangedMultipleDateSelectedEvent = onValueChangedEvent;
		m_onResponseMultipleDateSelectedEvent = onMultipleDateSelectedEvent;
	}

	/// <summary>
    /// Initialization view
    /// </summary>
	public virtual void InitView()
	{
		//find reference
		m_btnTabDay = transform.Find("Tabs/BtnDay").GetComponent<Button>();
        m_btnTabMonth = transform.Find("Tabs/BtnMonth").GetComponent<Button>();
        m_btnTabYear = transform.Find("Tabs/BtnYear").GetComponent<Button>();
		//
		m_dayView = transform.Find("DayView").GetComponent<HAGODatePickerDayTabView>();
		m_monthView = transform.Find("MonthView").GetComponent<HAGODatePickerMonthTabView>();
		m_yearView = transform.Find("YearView").GetComponent<HAGODatePickerYearTabView>();

		//init other view
		m_listDateSelectedData = new List<DateTime>();
		//
		m_yearView.Init(m_defaultSelectedDate.Year);
		m_monthView.Init(m_defaultSelectedDate.Month, m_yearView.yearSelected);
		m_dayView.Init(isMultiSelect, m_dateMarkData);
		//
		ChangeTab(DatePickerTabType.Day);

		//add listener
		m_btnTabDay.onClick.AddListener(TabDayOnClick);
		m_btnTabMonth.onClick.AddListener(TabMonthOnClick);
		m_btnTabYear.onClick.AddListener(TabYearOnClick);

		//register event
		HAGODateTimePickerControl.Api.OnDateSelectedEvent += OnDateSelectedHandler;
		HAGODateTimePickerControl.Api.OnDateDeselectedEvent += OnDateDeselectedHandler;
		HAGODateTimePickerControl.Api.OnMonthSelectedEvent += OnMonthSelectedHandler;
		HAGODateTimePickerControl.Api.OnYearSelectedEvent += OnYearSelectedHandler;
	}

	/// <summary>
    /// Handle click on tab day
    /// </summary>
	private void TabDayOnClick ()
	{
		ChangeTab(DatePickerTabType.Day);
	}

	/// <summary>
    /// Handle click on tab month
    /// </summary>
	private void TabMonthOnClick ()
	{
		ChangeTab(DatePickerTabType.Month);
	}

	/// <summary>
    /// Handle click on tab year
    /// </summary>
	private void TabYearOnClick ()
	{
		ChangeTab(DatePickerTabType.Year);
	}

	/// <summary>
    /// Handler date selected
    /// </summary>
	private void OnDateSelectedHandler (DateTime date)
	{
		if(isMultiSelect)
		{
			m_listDateSelectedData.Add(date);
			m_onValueChangedMultipleDateSelectedEvent?.Invoke(m_listDateSelectedData);
		}
		else
		{
			m_dateSelectedData = date;

			OnDateSelectedResponseHandler();
		}
	}
	
	/// <summary>
    /// Handler date deselected
    /// </summary>
	private void OnDateDeselectedHandler (DateTime date)
	{
		m_listDateSelectedData.Remove(date);
		m_onValueChangedMultipleDateSelectedEvent?.Invoke(m_listDateSelectedData);
	}

	/// <summary>
    /// Handler month selected
    /// </summary>
	private void OnMonthSelectedHandler (int month)
	{
		ChangeTab(DatePickerTabType.Day);
	}

	/// <summary>
    /// Handler year selected
    /// </summary>
	private void OnYearSelectedHandler (int year)
	{
		ChangeTab(DatePickerTabType.Month);
	}

	/// <summary>
    /// Change current tab to day, month or year
    /// </summary>
	private void ChangeTab (DatePickerTabType tab)
	{
		// change color current active tab
        OnTabChangeHandler(tab);

        // show current active tab content
        m_dayView.gameObject.SetActive(tab == DatePickerTabType.Day);
        m_monthView.gameObject.SetActive(tab == DatePickerTabType.Month);
        m_yearView.gameObject.SetActive(tab == DatePickerTabType.Year);

        // update view when date change
        if (tab == DatePickerTabType.Day)
        {
			if(isMultiSelect)
			{
				m_dayView.OnDateChangeHandler(m_monthView.monthSelected, m_yearView.yearSelected, m_listDateSelectedData);
			}
			else
			{
				m_dayView.OnDateChangeHandler(m_monthView.monthSelected, m_yearView.yearSelected, new List<DateTime>(){ m_defaultSelectedDate });
			}
        }
        else if (tab == DatePickerTabType.Month)
        {
			//do nothing
        }
        else if (tab == DatePickerTabType.Year)
        {
           	m_yearView.ForceScrollToYearSelected();
        }
	}

	/// <summary>
    /// Tab change handler
    /// </summary>
	private void OnTabChangeHandler (DatePickerTabType tab)
	{
		UpdateTabColor(m_btnTabDay, tab == DatePickerTabType.Day);
        UpdateTabColor(m_btnTabMonth, tab == DatePickerTabType.Month);
        UpdateTabColor(m_btnTabYear, tab == DatePickerTabType.Year);
	}

	/// <summary>
    /// Change color tab to green when pressed or white if not
    /// </summary>
    private void UpdateTabColor (Button btnTab, bool isActive)
    {
        // active or unactive interactable current selected tab
        btnTab.interactable = !isActive;

		// update color
        btnTab.GetComponent<Image>().color = isActive ? CONST_COLOR_TAB_ACTIVE : CONST_COLOR_TAB_NORMAL;
        btnTab.transform.Find("Text").GetComponent<Text>().color = isActive ? CONST_COLOR_TEXT_ACTIVE : CONST_COLOR_TEXT_NORMAL;
		//
        if (btnTab.GetComponent<Outline>() != null)
        {
            btnTab.GetComponent<Outline>().effectColor = isActive ? CONST_COLOR_TAB_OUTLINE_ACTIVE : CONST_COLOR_TAB_OUTLINE_NORMAL;
        }
    }

	/// <summary>
    /// Response DateTime / List<DateTime> selected
    /// </summary>
	public void OnDateSelectedResponseHandler()
	{
		if(isMultiSelect)
		{
			// Debug.Log("======= DateSelectedHandler =======");
			// foreach(DateTime d in m_listDateSelectedData.OrderBy(d => d.Date))
			// {
			// 	Debug.Log(d.ToString("dd MM yyyy"));
			// }
			// Debug.Log("===================================");

			m_onResponseMultipleDateSelectedEvent?.Invoke(GetMultiDateValue());
		}
		else
		{
			// Debug.Log("======= DateSelectedHandler =======");
			// Debug.Log(m_dateSelectedData.ToString("dd MM yyyy"));
			// Debug.Log("===================================");

			onResponseDateSelectedEvent?.Invoke(GetSingleDateValue());
		}
	}

	public List<DateTime> GetMultiDateValue()
	{
		return m_listDateSelectedData.OrderBy(d => d.Date).ToList();
	}

	public DateTime GetSingleDateValue()
	{
		return m_dateSelectedData;
	}
}
