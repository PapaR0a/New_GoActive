using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAPatientStoryView : MonoBehaviour
{
    public InputField storyInput = null;

    private void Start()
    {
        if (storyInput != null)
        {
            storyInput.text = GAMissionsModel.Api.patientStory;
        }
    }

    public void OnSavePatientStory()
    {
        if (storyInput != null)
        {
            GAMissionsModel.Api.patientStory = storyInput.text;
        }
    }
}
