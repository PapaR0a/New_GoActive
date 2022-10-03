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

    public void SubmitUserData()
    {
        GAPlayerDataDTO playerData = GAMissionsModel.Api.GetCurrentPlayerData();
        CPELoginControl.Api.SubmitAppData((JObject)JToken.FromObject(playerData));

        Debug.Log($"<color=yellow> PlayerData: {JsonConvert.SerializeObject(playerData)} </color>");
    }
}
