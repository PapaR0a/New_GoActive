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
    public Action onUpdatePainRecords;
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

    public void SubmitMissionsStatusData(string key, string[] keys, int[] values)
    {
        if (!GAMissionsModel.Api.GetMissionStatuses().ContainsKey(key))
            return;

        var statuses = GAMissionsModel.Api.GetMissionStatuses()[key];
        for (int i = 0; i < keys.Length; i++)
        {
            if (i < keys.Length && i < values.Length)
            {
                var newData = new MissionData();
                newData.key = keys[i];
                newData.value = values[i];

                bool hasAlreadyAnswered = false;
                foreach(var oldData in statuses)
                {
                    if (oldData.key == newData.key)
                    {
                        hasAlreadyAnswered = true;
                        break;
                    }
                }

                if (!hasAlreadyAnswered)
                {
                    statuses.Add(newData);
                }
                else
                {
                    for (int a = 0; a < statuses.Count; a++)
                    {
                        if (statuses[a] == newData)
                        {
                            statuses[a] = newData;
                            break;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < statuses.Count; i++)
        {
            if (string.IsNullOrEmpty( statuses[i].key) )
            {
                statuses.RemoveAt(i);
            }
        }

        GAMissionsModel.Api.UpdateMissionStatuses(key, statuses);

        CPELoginControl.Api.SubmitAppData((JObject)JToken.FromObject(GAMissionsModel.Api.missionsStatuses), GAConstants.SCHEMA_MISSION_STATUS);

        Debug.Log($"<color=yellow> Mission Status: {JsonConvert.SerializeObject(GAMissionsModel.Api.missionsStatuses)} </color>");
    }

    public void SubmitPainDiary()
    {
        CPELoginControl.Api.SubmitAppData(JToken.FromObject(GAMissionsModel.Api.cachedDiaryRecords), GAConstants.SCHEMA_PAIN_DIARY);
        Debug.Log($"<color=yellow> Pain Diary Records: {JsonConvert.SerializeObject(GAMissionsModel.Api.cachedDiaryRecords)} </color>");
    }

    // Unique per sending
    public void SubmitRecordData(JObject data, string schemaName = "")
    {
        CPELoginControl.Api.SubmitAppData(data, schemaName, true);

        Debug.Log($"<color=yellow> Record Data: {JsonConvert.SerializeObject(data)} </color>");
    }
}
