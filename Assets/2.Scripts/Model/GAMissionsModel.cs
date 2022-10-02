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
    public int missionUnlockingType = PlayerPrefs.GetInt(GAConstants.KEY_UNLOCKING_TYPE, 0);
    public float minimumDistanceToTravel = PlayerPrefs.GetInt(GAConstants.KEY_MINIMUM_DISTANCE, 1000);
    public int minimumStepsRequired = PlayerPrefs.GetInt(GAConstants.KEY_MINIMUM_STEPS, 1000);

    public List<List<GAPainRecordDTO>> cachedDiaryRecords = null;

    private GAPlayerDataDTO playerData = null;

    public GAPlayerDataDTO GetCurrentPlayerData()
    {
        GAMissionsControl.Api.onUpdatePlayerData?.Invoke();

        playerData = new GAPlayerDataDTO
            (
            lifePoints: 0,
            unlockedMissionsCount: unlockedMissionsCount,
            availableMissions: new List<bool>() { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
            painDiaryRecords: cachedDiaryRecords,
            missionUnlocking: missionUnlockingType,
            minimumDistanceRequired: minimumDistanceToTravel,
            minimumStepsRequired: minimumStepsRequired,
            settingsPassword: GAConstants.DEFAULT_PASSWORD
            );

        return playerData;
    }
}
