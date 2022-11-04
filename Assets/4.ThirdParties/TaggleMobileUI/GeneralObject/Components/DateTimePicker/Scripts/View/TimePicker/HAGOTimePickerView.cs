using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HAGOTimePickerView : MonoBehaviour
{
    private HAGOTimePickerScrollableItemView m_hourView;
    private HAGOTimePickerScrollableItemView m_minuteView;
    private HAGOTimePickerScrollableItemView m_halfDayPeriodView;

    //param
    private HAGOTimePickerType m_timePickerType;

    public IEnumerator Init(HAGOTimePickerType timePickerType, TimeSpan limitDuration)
    {
        m_timePickerType = timePickerType;

        //find reference
        m_hourView = transform.Find("ScrHour").GetComponent<HAGOTimePickerScrollableItemView>();
        m_minuteView = transform.Find("ScrMinute").GetComponent<HAGOTimePickerScrollableItemView>();
        m_halfDayPeriodView = transform.Find("ScrHalfDayPeriod").GetComponent<HAGOTimePickerScrollableItemView>();

        yield return StartCoroutine(IEUpdateView(m_timePickerType, limitDuration));
    }

    private IEnumerator IEUpdateView(HAGOTimePickerType timePickerType, TimeSpan limitDuration)
    {
        //show/hide AM/PM picker
        m_halfDayPeriodView.gameObject.SetActive(IsFormat12Hours());

        //init view
        switch(timePickerType)
        {
            case HAGOTimePickerType.Format12Hours:
                yield return StartCoroutine(m_hourView.Init(HAGOTimePickerScrollType.Hour, 0, 12));
                yield return StartCoroutine(m_minuteView.Init(HAGOTimePickerScrollType.Minute, 0, 59));
                yield return StartCoroutine(m_halfDayPeriodView.Init(HAGOTimePickerScrollType.HalfDayPeriod));
                break;

            case HAGOTimePickerType.Format24Hours:
                yield return StartCoroutine(m_hourView.Init(HAGOTimePickerScrollType.Hour, 0, 24));
                yield return StartCoroutine(m_minuteView.Init(HAGOTimePickerScrollType.Minute, 0, 59));
                break;

            case HAGOTimePickerType.Duration:
                bool isLimitDuration = !limitDuration.Equals(TimeSpan.Zero);
                int limitHours = isLimitDuration && limitDuration.Hours > 0 ? limitDuration.Hours : 24;
                int limitMinutes = isLimitDuration && limitDuration.Minutes > 0 ? limitDuration.Minutes : 59;
                yield return StartCoroutine(m_hourView.Init(HAGOTimePickerScrollType.Hour, 0, limitHours));
                yield return StartCoroutine(m_minuteView.Init(HAGOTimePickerScrollType.Minute, 0, limitMinutes));
                break;
        }
    }

    private bool IsFormat12Hours()
    {
        return m_timePickerType == HAGOTimePickerType.Format12Hours;
    }

    public TimeSpan GetValue()
    {
        int hours = m_hourView.GetValue() + (IsFormat12Hours() ? (m_halfDayPeriodView.GetValue() == (int)HAGOHalfDayPeriod.AM ? 0 : 12) : 0);
        int minutes = m_minuteView.GetValue();
        TimeSpan timeSpan = new TimeSpan(hours, minutes, 0);
        return timeSpan;
    }
}
