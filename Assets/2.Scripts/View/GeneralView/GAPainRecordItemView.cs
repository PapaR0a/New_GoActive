using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAPainRecordItemView : MonoBehaviour
{
    [SerializeField] private Text itemDate = null;
    [SerializeField] private GameObject recordThumbnail = null;
    [SerializeField] private List<GAPainRecordTogglesItemView> diaryItems = new List<GAPainRecordTogglesItemView>();

    public void SetThumbnailParent(Transform parent)
    {
        recordThumbnail.transform.parent = parent;
    }

    public void GenerateDiaryItems(List<GAPainRecordDTO> records)
    {
        for (int i = 0; i < records.Count; i++)
        {
            diaryItems[i].UpdateItem(records[i]);
        }
    }

    public List<GAPainRecordDTO> GetDiaryData()
    {
        List<GAPainRecordDTO> ItemsData = new List<GAPainRecordDTO>();

        foreach (var data in diaryItems)
        {
            ItemsData.Add(data.GetPainRecord());
        }

        return ItemsData;
    }

    public void SubmitDiaryData()
    {
        GAMissionsControl.Api.onUpdatePainRecords?.Invoke();

        GAMissionsControl.Api.SubmitPainDiary();
    }
}
