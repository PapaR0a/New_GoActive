using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAUIPopupView : MonoBehaviour
{
    private void Start()
    {
        GAMissionsControl.Api.onUpdatePlayerData += UpdateDiaryCache;
    }

    private void OnDestroy()
    {
        GAMissionsControl.Api.onUpdatePlayerData -= UpdateDiaryCache;
    }

    private void UpdateDiaryCache()
    {
        GAMissionsModel.Api.cachedDiaryRecords = GetDiaryRecords();
    }

    public List<List<GAPainRecordDTO>> GetDiaryRecords()
    {
        List<List<GAPainRecordDTO>> diaryRecords = new List<List<GAPainRecordDTO>>();

        foreach (Transform tf in transform)
        {
            var painRecord = tf.gameObject.GetComponent<GAPainRecordItemView>();
            if (painRecord != null)
            {
                diaryRecords.Add(painRecord.GetDiaryData());
            }
        }

        return diaryRecords;
    }
}
