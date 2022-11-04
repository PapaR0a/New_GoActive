using System.Collections;
using System.Collections.Generic;
using Honeti;
using UnityEngine;

public class HAGODateTimePickerModel
{
    private static HAGODateTimePickerModel m_api;
    public static HAGODateTimePickerModel Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new HAGODateTimePickerModel();
            }
            return m_api;
        }
    }
}

public enum HAGODateTimePickerType
{
    SingleDate,
    MultiDate,
    Time
}

public enum HAGOTimePickerScrollType
{
    Hour,
    Minute,
    HalfDayPeriod
}

public enum HAGOHalfDayPeriod
{
    AM,
    PM
}

public enum HAGOTimePickerType
{
    Format12Hours,
    Format24Hours,
    Duration
}
