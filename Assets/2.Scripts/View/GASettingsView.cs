using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GASettingsView : MonoBehaviour
{
    [SerializeField] private InputField m_minimumDistanceInput = null;
    [SerializeField] private InputField m_minimumStepInput = null;
    [SerializeField] private InputField m_passwordInput = null;

    [SerializeField] private GameObject m_diaryItemPref = null;
    [SerializeField] private Transform m_thumbnailParent = null;
    [SerializeField] private Transform m_optionParent = null;

    private void Start()
    {
        m_minimumDistanceInput.text = GAMapModel.Api.minimumDistanceToTravel.ToString();
        m_minimumStepInput.text = GAMapModel.Api.minimumStepsRequired.ToString();
    }

    private void CreateDiaryItems(List<List<GAPainRecordDTO>> painRecords)
    {
        if (painRecords != null && painRecords.Count > 0)
        {
            StartCoroutine(InstantiateItems(painRecords));
        }
    }

    private IEnumerator InstantiateItems(List<List<GAPainRecordDTO>> painRecords)
    {
        var waitForFrame = new WaitForEndOfFrame();

        foreach(var record in painRecords)
        {
            var painRecordView = Instantiate(m_diaryItemPref, m_optionParent).GetComponent<GAPainRecordItemView>();
            painRecordView.GenerateDiaryItems(record);
            painRecordView.SetThumbnailParent(m_thumbnailParent);

            yield return waitForFrame;
        }

        yield return null;
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
