using HutongGames.PlayMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAMissionsModel
{
    public static GAMissionsModel m_api;
    public static GAMissionsModel Api
    {
        get
        {
            if (m_api == null)
                m_api = new GAMissionsModel();
            return m_api;
        }
    }

    public int unlockedMissionsCount = 0;
    public int missionUnlocking = 0;
    public float minimumDistanceToTravel = PlayerPrefs.GetInt(GAConstants.KEY_MINIMUM_DISTANCE, 1000);
    public int minimumStepsRequired = PlayerPrefs.GetInt(GAConstants.KEY_MINIMUM_STEPS, 1000);

    public List<List<GAPainRecordDTO>> cachedDiaryRecords = null;

    private GAPlayerDataDTO playerData = null;

    public int lifePoints = 0;

    public float distanceRemaining = 0;
    public float distanceTraveled = 0;
    public float distanceTotalTraveled = 0;

    public string settingPassword = "";

    public int stepsRemaining = 0;
    public int stepsMade = 0;

    public string patientStory = "";

    public Dictionary<string, List<MissionData>> missionsStatuses = null;

    public void UpdateMissionStatuses(string key, List<MissionData> updatedData)
    {
        missionsStatuses[key] = updatedData;
    }

    public Dictionary<string, List<MissionData>> GetMissionStatuses()
    {
        if (missionsStatuses == null)
        {
            missionsStatuses = new Dictionary<string, List<MissionData>>()
            {
                {"m1d1_save", new List<MissionData>() },
                {"m1d2_save", new List<MissionData>() },
                {"m1d3_save", new List<MissionData>() },
                {"m1d4_save", new List<MissionData>() },
                {"m1d5_save", new List<MissionData>() },
                {"m1d6_save", new List<MissionData>() },
                {"m2d1_save", new List<MissionData>() },
                {"m2d2_save", new List<MissionData>() },
                {"m2d3_save", new List<MissionData>() },
                {"m2d4_save", new List<MissionData>() },
                {"m2d5_save", new List<MissionData>() },
                {"m2d6_save", new List<MissionData>() }
            };
        }

        return missionsStatuses;
    }

    public void UpdatePlayerData(GAPlayerDataDTO data)
    {

    }

    public GAPlayerDataDTO GetCurrentPlayerData()
    {
        GAMissionsControl.Api.onUpdatePlayerData?.Invoke();

        playerData = new GAPlayerDataDTO
            (
                lifePoints: FsmVariables.GlobalVariables.GetFsmInt("GA_Lifepoints").Value,
                unlockedMissionsCount: unlockedMissionsCount,
                painDiaryRecords: cachedDiaryRecords,
                missionUnlocking: missionUnlocking,
                minimumDistanceRequired: minimumDistanceToTravel,
                minimumStepsRequired: minimumStepsRequired,
                settingsPassword: settingPassword,
                distanceRemaining: distanceRemaining,
                distanceTraveled: distanceTraveled,
                distanceTotalTraveled: distanceTotalTraveled,
                stepsMade: stepsMade,
                patientStory: patientStory
            );

        return playerData;
    }
}
