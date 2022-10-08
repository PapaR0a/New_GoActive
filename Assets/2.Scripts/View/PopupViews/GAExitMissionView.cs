using HutongGames.PlayMaker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAExitMissionView : MonoBehaviour
{
    public int missionNumber;

    private string startDate = "";
    private string endDate = "";

    private Text missionDataText;

    private void Start()
    {
        startDate = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");

        missionDataText = GetComponent<Text>() ?? null;

        Debug.Log($"<color=yellow>Current unlocking type: {GAMissionsModel.Api.missionUnlocking} </color>");
        if (GAMissionsModel.Api.missionUnlocking == 1)
        {
            Debug.Log($"<color=yellow> Opened Mission Number: {missionNumber}</color>");
            GAMissionsControl.Api.UnlockNewMission(missionNumber);
        }

        GAMissionsControl.Api.onToggleMainCamera?.Invoke(false);
    }

    private void OnDestroy()
    {
        endDate = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        SubmitData("Exit Mission");
        GAMissionsControl.Api.SubmitUserData();

        GAMissionsControl.Api.onToggleMainCamera?.Invoke(true);

        GAMissionsControl.Api.onToggleMission?.Invoke(true);
    }

    private void SubmitData(string activityName = "")
    {
        if (missionDataText == null)
            return;

        GAMissionDTO missionData = new GAMissionDTO
            (
            activityName: activityName,
            missionName: gameObject.name.Replace("_save", ""),
            dateStarted: startDate,
            dateEnded: endDate,
            activities: missionDataText.text
            );

        GAMissionsControl.Api.SubmitRecordData((JObject)JToken.FromObject(missionData), GAConstants.SCHEMA_MISSION_DATA);
    }
}
