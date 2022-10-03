using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GASubmitDataTriggerView : MonoBehaviour
{
    public void SubmitDataToServer()
    {
        GAMissionsControl.Api.SubmitUserData();
    }
}
