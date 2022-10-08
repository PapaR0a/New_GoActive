using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GAMissionDTO
{
    public string activityName;
    public string missionName;

    public string dateStarted;
    public string dateEnded;

    public List<string> activities;

    public GAMissionDTO(string activityName = "", string missionName = "", string dateStarted = "", string dateEnded = "", string activities = null)
    {
        this.activityName = activityName;
        this.missionName = missionName;
        this.dateStarted = dateStarted;
        this.dateEnded = dateEnded;
        this.activities = ConvertIntoList(activities);
    }

    private List<string> ConvertIntoList(string value)
    {
        return value.Split(new string[] { "\r\n", "\r", "\n" },StringSplitOptions.None).ToList();
    }
}
