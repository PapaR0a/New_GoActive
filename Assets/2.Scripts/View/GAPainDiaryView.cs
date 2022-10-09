using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GAPainDiaryView : MonoBehaviour
{
    [SerializeField] private GameObject m_diaryItemPref = null;
    [SerializeField] private Transform m_thumbnailParent = null;
    [SerializeField] private Transform m_optionParent = null;

    private void Start()
    {
        
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GAMissionsControl.Api.SubmitUserData();
        }
#endif
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
}
