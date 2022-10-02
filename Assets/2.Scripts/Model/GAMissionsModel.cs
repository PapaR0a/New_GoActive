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
    public int unlockNextMissions = PlayerPrefs.GetInt(GAConstants.KEY_UNLOCK_NEXT_MISSIONS, 0);
}
