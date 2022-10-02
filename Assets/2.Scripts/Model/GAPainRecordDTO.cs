using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAPainRecordDTO
{
    public string recordTitle;

    public float painValue;
    public List<bool> options = new List<bool>();
    public string other;

    public DateTime? painStarted;
    public int typeOfPain;
    public int painEndedType;
    public string duration;
    public DateTime? painEnded;
    public string thoughts;

    public float matterValue;
    public List<bool> activities = new List<bool>();
    public string otherActivity;
    public string activityThoughts;

    public GAPainRecordDTO(string recordTitle = "", float painValue = -1, List<bool> values = null, string otherNote = "", DateTime? painStarted = null, int typeOfPain = 0, int painEndedType = 0, string durationOfPain = "", DateTime? painEnded = null, string thoughts = "", float matterValue = -1, List<bool> activities = null, string otherActivity = "", string activityThoughts = "")
    {
        this.recordTitle = recordTitle;

        this.painValue = painValue;
        options = values;
        other = otherNote; 

        this.painStarted = painStarted;
        this.typeOfPain = typeOfPain;
        this.painEndedType = painEndedType;
        duration = durationOfPain;
        this.painEnded = painEnded;
        this.thoughts = thoughts;

        this.matterValue = matterValue;
        this.activities = activities;
        this.otherActivity = otherActivity;
        this.activityThoughts = activityThoughts;
    }
}
