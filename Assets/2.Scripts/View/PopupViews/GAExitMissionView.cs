using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAExitMissionView : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener( ()=> GAMissionsControl.Api.onToggleMission?.Invoke(true) );
    }

    private void OnDestroy()
    {
        GAMissionsControl.Api.onToggleMission?.Invoke(true);
        gameObject.GetComponent<Button>().onClick.AddListener( () => GAMissionsControl.Api.onToggleMission?.Invoke(true) );
    }
}
