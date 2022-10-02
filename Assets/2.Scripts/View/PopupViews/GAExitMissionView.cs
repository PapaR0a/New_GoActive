using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAExitMissionView : MonoBehaviour
{
    public int missionNumber;

    private void Start()
    {
        if (GAMissionsModel.Api.unlockNextMissions == 1)
        {
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
