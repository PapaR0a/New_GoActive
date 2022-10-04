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

    public int unlockedMissionsCount = PlayerPrefs.GetInt(GAConstants.KEY_MISSIONS_UNLOCKED, 0);
    public int missionUnlocking = PlayerPrefs.GetInt(GAConstants.KEY_UNLOCKING_TYPE, 0);
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

    public void UpdatePlayerData(GAPlayerDataDTO data)
    {

    }

    public GAPlayerDataDTO GetCurrentPlayerData()
    {
        GAMissionsControl.Api.onUpdatePlayerData?.Invoke();

        playerData = new GAPlayerDataDTO
            (
            lifePoints: lifePoints,
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
