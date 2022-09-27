using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAWalkOptionsView : MonoBehaviour
{
    public void ChooseDistance()
    {
        GAMapControl.Api.ChooseGameType((int)GAMapModel.GoalType.Distance);
    }

    public void ChooseDestination()
    {
        GAMapControl.Api.ChooseGameType((int)GAMapModel.GoalType.Marker);
    }

    public void ChooseSteps()
    {
        GAMapControl.Api.ChooseGameType((int)GAMapModel.GoalType.Steps);
    }
}
