using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAExitMissionView : MonoBehaviour
{
    public int missionNumber;

    private void Start()
    {
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
        GAMissionsControl.Api.SubmitUserData();

        GAMissionsControl.Api.onToggleMainCamera?.Invoke(true);

        GAMissionsControl.Api.onToggleMission?.Invoke(true);
    }
}
