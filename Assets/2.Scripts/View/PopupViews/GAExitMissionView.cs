using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAExitMissionView : MonoBehaviour
{
    public int missionNumber;

    private void Start()
    {
        Debug.Log($"<color=yellow>Current unlocking type: {GAMissionsModel.Api.missionUnlockingType} </color>");
        if (GAMissionsModel.Api.missionUnlockingType == 1)
        {
            Debug.Log($"<color=yellow> Opened Mission Number: {missionNumber}</color>");
            GAMissionsControl.Api.UnlockNewMission(missionNumber);
        }
        
        //gameObject.GetComponent<Button>()?.onClick.AddListener( ()=> GAMissionsControl.Api.onToggleMission?.Invoke(true) );
    }

    private void OnDestroy()
    {
        GAMissionsControl.Api.onToggleMission?.Invoke(true);
        //gameObject.GetComponent<Button>()?.onClick.AddListener( () => GAMissionsControl.Api.onToggleMission?.Invoke(true) );
    }
}
