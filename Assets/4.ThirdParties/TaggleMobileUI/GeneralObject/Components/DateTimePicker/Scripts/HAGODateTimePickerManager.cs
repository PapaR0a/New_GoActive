using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HAGODateTimePickerManager : MonoBehaviour
{
	private static HAGODateTimePickerManager m_api;
    public static HAGODateTimePickerManager Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = Instantiate(Resources.Load<GameObject>(HAGOConstant.PREFAB_DATETIME_PICKER)).GetComponent<HAGODateTimePickerManager>();
            }
            return m_api;
        }
    }

	public void InitTimePicker(Action<TimeSpan> callback, HAGOTimePickerType timePickerType = HAGOTimePickerType.Format12Hours)
	{
        HAGODateTimePickerControl.Api.ResultCallbackTimeSpanEvent = callback;

        //init view
        HAGODateTimePickerView view = transform.Find("Canvas").GetComponent<HAGODateTimePickerView>();
        view.InitTimePicker(timePickerType, TimeSpan.Zero);
	}

	public void InitTimePickerDuration(Action<TimeSpan> callback, TimeSpan limitDuration = new TimeSpan())
	{
        HAGODateTimePickerControl.Api.ResultCallbackTimeSpanEvent = callback;

        //init view
        HAGODateTimePickerView view = transform.Find("Canvas").GetComponent<HAGODateTimePickerView>();
        view.InitTimePicker(HAGOTimePickerType.Duration, !limitDuration.Equals(new TimeSpan()) ? limitDuration : TimeSpan.Zero);
	}

    /// <summary>
    /// Initialization single date picker
    /// </summary>
	///<param name = "onDateSelectedEvent">Return a list selected date after user selected day on calendar
	///or user call FinishSelectMultipleDate() if in single select mode</param>
	///<param name = "dateMarkData">Dates will have circle mark icon</param>
	public void InitSingleDatePicker(Action<DateTime> onDateSelectedEvent, DateTime selectedDate = new DateTime(), Dictionary<DateTime,bool> dateMarkData = null)
	{
		HAGODateTimePickerControl.Api.ResultCallbackDateTimeEvent = onDateSelectedEvent;

        //init view
        HAGODateTimePickerView view = transform.Find("Canvas").GetComponent<HAGODateTimePickerView>();
        view.InitSingleDatePicker(selectedDate == new DateTime() ? DateTime.Today : selectedDate, dateMarkData);
	}

	/// <summary>
    /// Initialization multiple date picker
    /// </summary>
	///<param name = "onValueChanged">Return a list selected dates after user selected day on calendar</param>
	///<param name = "onMultiDateSelectedEvent">Return a list selected dates after user call FinishSelectMultipleDate() if in multiple select mode</param>
	///<param name = "dateMarkData">Dates will have circle mark icon</param>
	public void InitMultiDatePicker(Action<List<DateTime>> onMultiDateSelectedEvent, Dictionary<DateTime,bool> dateMarkData = null)
	{
		HAGODateTimePickerControl.Api.ResultCallbackMultiDateEvent = onMultiDateSelectedEvent;

        //init view
        HAGODateTimePickerView view = transform.Find("Canvas").GetComponent<HAGODateTimePickerView>();
        view.InitMultiDatePicker(dateMarkData);
    }

    public void Destroy()
    {
        HAGODateTimePickerView view = transform.Find("Canvas").GetComponent<HAGODateTimePickerView>();
        view.Destroy();
        //
        Destroy(this.gameObject);
    }
}