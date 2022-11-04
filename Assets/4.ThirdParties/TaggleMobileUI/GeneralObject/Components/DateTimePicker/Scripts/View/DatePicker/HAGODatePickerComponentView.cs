using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HAGODatePickerComponentView : HAGODatePickerCoreView
{
	/// <summary>
    /// Initialization single date picker
    /// </summary>
	///<param name = "onValueChanged">Return a list selected date after user selected day on calendar
	///or user call FinishSelectMultipleDate() if single select mode</param>
	///<param name = "dateMarkData">Dates will have circle mark icon</param>
	public override void InitDatePicker(Action<DateTime> onValueChanged, DateTime selectedDate, Dictionary<DateTime,bool> dateMarkData = null)
	{
		base.InitDatePicker(onValueChanged, selectedDate, dateMarkData);
		base.InitView();
	}

	/// <summary>
    /// Initialization multiple date picker
    /// </summary>
	///<param name = "onValueChanged">Return a list selected dates after user selected day on calendar</param>
	///<param name = "onMultiDateSelectedEvent">Return a list selected dates after user call FinishSelectMultipleDate() if multiple select mode</param>
	///<param name = "dateMarkData">Dates will have circle mark icon</param>
	public override void InitMultiDatePicker (Action<List<DateTime>> onValueChanged, Action<List<DateTime>> onMultiDateSelectedEvent = null, Dictionary<DateTime,bool> dateMarkData = null)
	{
		base.InitMultiDatePicker(onValueChanged, onMultiDateSelectedEvent, dateMarkData);
		base.InitView();
	}
}
