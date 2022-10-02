using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GASettingsView : MonoBehaviour
{
    [SerializeField] private InputField m_minimumDistanceInput = null;
    [SerializeField] private InputField m_minimumStepInput = null;
    [SerializeField] private InputField m_passwordInput = null;

    private void Start()
    {
        m_minimumDistanceInput.text = GAMapModel.Api.minimumDistanceToTravel.ToString();
        m_minimumStepInput.text = GAMapModel.Api.minimumStepsRequired.ToString();
    }

    /// <summary>
    /// Mission unlocking types: 0 - each day or after finishing map walk goal, 1 - unlock next mission once current unlocked mission is played
    /// </summary>
    /// <param name="unlockingType"></param>
    public void OnChangeUnlockingMissions(int unlockingType)
    {
        GAMissionsModel.Api.missionUnlockingType = unlockingType;
    }

    public void OnApplyChanges()
    {
        if (m_passwordInput.text != GAConstants.DEFAULT_PASSWORD)
            return;

        /// MINIMUM DISTANCE TO TRAVEL SCOPE
        {
            var newMinimumDistance = (int)GAMapModel.Api.minimumDistanceToTravel;

            if (m_minimumDistanceInput.text != string.Empty)
                int.TryParse(m_minimumDistanceInput.text, out newMinimumDistance);

            PlayerPrefs.SetInt(GAConstants.KEY_MINIMUM_DISTANCE, newMinimumDistance);
            GAMapModel.Api.minimumDistanceToTravel = newMinimumDistance;
        }

        /// MINIMUM STEPS TO TRAVEL SCOPE
        {
            var newMinimumSteps = GAMapModel.Api.minimumStepsRequired;

            if (m_minimumStepInput.text != string.Empty)
                int.TryParse(m_minimumStepInput.text, out newMinimumSteps);

            PlayerPrefs.SetInt(GAConstants.KEY_MINIMUM_STEPS, newMinimumSteps);
            GAMapModel.Api.minimumStepsRequired = newMinimumSteps;
        }

        /// save unlocking type
        {
            PlayerPrefs.SetInt(GAConstants.KEY_UNLOCKING_TYPE, GAMissionsModel.Api.missionUnlockingType);
        }

        GAMissionsControl.Api.SubmitUserData();

        ResetInputFields();
    }

    /// <summary>
    /// Called when password is correct
    /// </summary>
    private void ResetInputFields()
    {
        m_minimumDistanceInput.text = string.Empty;
        m_minimumStepInput.text = string.Empty;
        m_passwordInput.text = string.Empty;

        GAMapControl.Api.ChooseGameType(); //Reset Map walk
    }
}
