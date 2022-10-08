using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GAMissionStatusDTO
{
    public List<string> keys;
    public List<bool> values;

    public GAMissionStatusDTO(List<string> keys, List<bool> values)
    {
        var statuses = GAMissionsModel.Api.missionsStatuses;

        for (int i = 0; i < keys.Count; i++)
        {

        }
    }
}

public class MissionData 
{
    public string key;
    public bool value;
}
