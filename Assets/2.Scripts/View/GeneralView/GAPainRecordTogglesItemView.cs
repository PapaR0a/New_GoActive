using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAPainRecordTogglesItemView : MonoBehaviour
{
    public Text recordTitle = null;

    public Slider PainValue = null;
    public List<Toggle> Options = new List<Toggle>();
    public InputField Other = null;

    public DatePickerControl StartedDate = null;
    public DatePickerControl StartedTime = null;

    public List<Toggle> TypesOfPain = new List<Toggle>();
    public List<Toggle> PainEndedType = new List<Toggle>();
    public InputField PainDuration = null;

    public DatePickerControl EndedDate = null;
    public DatePickerControl EndedTime = null;

    public InputField Thoughts = null;

    public Slider MatterValue = null;
    public List<Toggle> Activities = new List<Toggle>();
    public InputField OtherActivity = null;
    public InputField ActivityThoughts = null;

    public void UpdateItem(GAPainRecordDTO cloudData)
    {
        #region pain section page

        if (recordTitle != null && !string.IsNullOrEmpty(cloudData.recordTitle))
        {
            recordTitle.text = cloudData.recordTitle;
        }

        if (PainValue != null && cloudData.painValue >= 0)
        {
            PainValue.value = cloudData.painValue;
        }

        if (Options != null && Options.Count > 0 && cloudData.options != null && cloudData.options.Count > 0)
        {
            for (int i = 0; i < cloudData.options.Count; i++)
            {
                Options[i].isOn = cloudData.options[i];
            }
        }
        
        if (Other != null && !string.IsNullOrEmpty(cloudData.other))
        {
            Other.text = cloudData.other;
        }

        if (Thoughts != null && !string.IsNullOrEmpty(cloudData.thoughts))
        {
            Thoughts.text = cloudData.thoughts;
        }

        if (cloudData.painStarted != null)
        {
            if (StartedDate != null)
            {
                StartedDate.timeOn = false;

                StartedDate.dateOn = true;
                StartedDate.fecha = (DateTime)cloudData.painStarted;
                StartedDate.actualizarFecha();
            }

            if (StartedTime != null)
            {
                StartedDate.dateOn = false;

                StartedTime.timeOn = true;
                StartedTime.fecha = (DateTime)cloudData.painStarted;
                StartedTime.actualizarFecha();
            }
        }

        if (TypesOfPain.Count > 0)
        {
            TypesOfPain[cloudData.typeOfPain].isOn = true;
        }

        if (PainEndedType.Count > 0)
        {
            PainEndedType[cloudData.painEndedType].isOn = true;
        }

        if (PainDuration != null)
        {
            PainDuration.text = cloudData.duration;
        }

        if (cloudData.painEnded != null)
        {
            if (EndedDate != null)
            {
                EndedDate.timeOn = false;

                EndedDate.dateOn = true;
                EndedDate.fecha = (DateTime)cloudData.painEnded;
                EndedDate.actualizarFecha();
            }

            if (EndedTime != null)
            {
                EndedTime.dateOn = false;

                EndedTime.timeOn = true;
                EndedTime.fecha = (DateTime)cloudData.painEnded;
                EndedTime.actualizarFecha();
            }
        }

        #endregion

        #region activities page

        if (MatterValue != null && cloudData.matterValue >= 0)
        {
            MatterValue.value = cloudData.matterValue;
        }

        if (Activities != null && Activities.Count > 0 && cloudData.activities != null && cloudData.activities.Count > 0)
        {
            for (int i = 0; i < cloudData.activities.Count; i++)
            {
                Activities[i].isOn = cloudData.activities[i];
            }
        }

        if (OtherActivity != null && !string.IsNullOrEmpty(cloudData.otherActivity))
        {
            OtherActivity.text = cloudData.otherActivity;
        }

        if (ActivityThoughts != null && !string.IsNullOrEmpty(cloudData.activityThoughts))
        {
            ActivityThoughts.text = cloudData.activityThoughts;
        }

        #endregion
    }

    public GAPainRecordDTO GetPainRecord()
    {
        GAPainRecordDTO currentData = new GAPainRecordDTO();

        #region pain section page

        if (recordTitle != null)
        {
            currentData.recordTitle = recordTitle.text;
        }

        if (PainValue != null)
        {
            currentData.painValue = PainValue.value;
        }

        if (Options.Count > 0)
        {
            currentData.options = new List<bool>();
            for (int i = 0; i < Options.Count; i++)
            {
                currentData.options.Add(Options[i].isOn);
            }
        }

        if (Other != null)
        {
            currentData.other = Other.text;
        }

        if (Thoughts != null)
        {
            currentData.thoughts = Thoughts.text;
        }

        if (StartedDate != null)
        {
            currentData.painStarted = StartedDate.fecha;
        }

        if (TypesOfPain.Count > 0)
        {
            for (int i = 0; i < TypesOfPain.Count; i++)
            {
                if (TypesOfPain[i].isOn)
                {
                    currentData.typeOfPain = i;
                    break;
                }
            }
        }

        if (PainEndedType.Count > 0)
        {
            for (int i = 0; i < PainEndedType.Count; i++)
            {
                if (PainEndedType[i].isOn)
                {
                    currentData.painEndedType = i;
                    break;
                }
            }
        }

        if (PainDuration != null)
        {
            currentData.duration = PainDuration.text;
        }

        if (EndedDate != null)
        {
            currentData.painEnded = EndedDate.fecha;
        }

        #endregion

        #region activities page

        if (MatterValue != null )
        {
            currentData.matterValue = MatterValue.value;
        }

        if (Activities.Count > 0)
        {
            currentData.activities = new List<bool>();
            for (int i = 0; i < Activities.Count; i++)
            {
                currentData.activities.Add(Activities[i].isOn);
            }
        }

        if (OtherActivity != null)
        {
           currentData.otherActivity = OtherActivity.text;
        }

        if (ActivityThoughts != null)
        {
            currentData.activityThoughts = ActivityThoughts.text;
        }

        #endregion

        return currentData;
    }
}
