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

    private void Start()
    {
        // Get data from server
        //GetDiaryData();
    }

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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GetDiaryData();
        }
    }

    private void GetDiaryData()
    {
        List<GAPainRecordDTO> ItemsData = new List<GAPainRecordDTO>();

        foreach (var data in diaryItems)
        {
            ItemsData.Add(data.GetPainRecord());
        }

        Debug.LogError($"{JsonConvert.SerializeObject(ItemsData)}");
    }
}
