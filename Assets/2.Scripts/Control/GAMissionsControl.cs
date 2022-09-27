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

    public void RefreshMissions()
    {
        for (int i = 0; i < GAMissionsModel.Api.unlockedMissionsCount; i++)
        {
            onUnlockNewMission?.Invoke();
        }
    }

    public void UnlockNewMission()
    {
        GAMissionsModel.Api.unlockedMissionsCount++;
        PlayerPrefs.SetInt(GAConstants.KEY_MISSIONS_UNLOCKED, GAMissionsModel.Api.unlockedMissionsCount);

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
}
