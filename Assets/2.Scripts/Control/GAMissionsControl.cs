using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GAMissionsControl
{
    private static GAMissionsControl m_api;

    public static GAMissionsControl Api
    {
        get { return m_api; }
        set { m_api = value; }
    }

    public Action<bool> onToggleMission;
    public Action onUnlockNewMission;
    public Action<int> onChangeMap;
    public Action onUpdatePlayerData;
    public Action<bool> onToggleMainCamera;

    public void RefreshMissions()
    {
        for (int i = 0; i < GAMissionsModel.Api.unlockedMissionsCount; i++)
        {
            onUnlockNewMission?.Invoke();
        }
    }

    public void UnlockNewMission(int missionNumber = -1)
    {
        int currentMission = GAMissionsModel.Api.unlockedMissionsCount;

        Debug.Log($"<color=yellow>Try to unlock new mission CurrentUnlocked:{currentMission}</color>");

        if (missionNumber <= 0 || missionNumber >= currentMission)
        {
            if (GAMissionsModel.Api.unlockedMissionsCount < 36)
                GAMissionsModel.Api.unlockedMissionsCount++;

            PlayerPrefs.SetInt(GAConstants.KEY_MISSIONS_UNLOCKED, GAMissionsModel.Api.unlockedMissionsCount);
            Debug.Log($"<color=yellow> New mission unlocked successfully!</color>");

            SubmitUserData();
        }

        RefreshMissions();
    }

    public void SelectMission(string sceneName)
    {
        SceneHelper.LoadSceneAdditiveAsync(sceneName);
        onToggleMission?.Invoke(false);
    }

    public void ChangeMap(int val)
    {
        onChangeMap?.Invoke(val);
    }

    // Updates data
    public void SubmitUserData()
    {
        GAPlayerDataDTO playerData = GAMissionsModel.Api.GetCurrentPlayerData();
        CPELoginControl.Api.SubmitAppData((JObject)JToken.FromObject(playerData));

        Debug.Log($"<color=yellow> PlayerData: {JsonConvert.SerializeObject(playerData)} </color>");
    }

    public void SubmitMissionsStatusData()
    {
        CPELoginControl.Api.SubmitAppData((JObject)JToken.FromObject(GAMissionsModel.Api.missionsStatuses), GAConstants.SCHEMA_MISSION_STATUS);

        Debug.Log($"<color=yellow> PlayerData: {JsonConvert.SerializeObject(GAMissionsModel.Api.missionsStatuses)} </color>");
    }

    // Unique per sending
    public void SubmitRecordData(JObject data, string schemaName = "")
    {
        CPELoginControl.Api.SubmitAppData(data, schemaName, true);

        Debug.Log($"<color=yellow> Record Data: {JsonConvert.SerializeObject(data)} </color>");
    }
}
