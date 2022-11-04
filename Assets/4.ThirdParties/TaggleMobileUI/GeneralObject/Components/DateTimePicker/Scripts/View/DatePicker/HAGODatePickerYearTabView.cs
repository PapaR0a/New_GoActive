using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HAGODatePickerYearTabView : MonoBehaviour
{
	private ScrollRect m_scrollRect;
	private Transform m_yearContent;
	//
	private GameObject m_prefYearItem;

	//param
	public int yearSelected;
	//
	private Dictionary<int, RectTransform> m_rectYearItems;

	//const
	private int CONST_YEAR_MIN = 1900;
    private int CONST_YEAR_MAX = DateTime.Today.Year + 100;

	void OnDestroy ()
	{
		Destroy();
	}

	/// <summary>
    /// Unregister event when gameobject destroy
    /// </summary>
	public void Destroy ()
	{
		//unregister event
		foreach(Transform tf in m_yearContent)
		{
			if(tf.gameObject != m_prefYearItem)
			{
				tf.GetComponent<HAGODatePickerYearTabYearItemView>().Destroy();
			}
		}
		//
		HAGODateTimePickerControl.Api.OnYearSelectedEvent -= OnYearSelectedHandler;
	}

	/// <summary>
    /// Initialization view
    /// </summary>
	public void Init (int yearDefault)
	{
		yearSelected = yearDefault;

		//find reference
		m_scrollRect = transform.Find("ScrollView").GetComponent<ScrollRect>();
		m_yearContent = transform.Find("ScrollView/Viewport/Content");
		m_prefYearItem = transform.Find("ScrollView/Viewport/Content/YearItem").gameObject;
		
		//handle value
		m_rectYearItems = new Dictionary<int, RectTransform>();
		//
		for (int year = CONST_YEAR_MIN; year <= CONST_YEAR_MAX; year++)
        {
            GameObject go = Instantiate(m_prefYearItem, m_yearContent);
            HAGODatePickerYearTabYearItemView itemView = go.GetComponent<HAGODatePickerYearTabYearItemView>();

			itemView.Init(year);
			go.SetActive(true);

			if(year == yearSelected)
			{
				itemView.SetColorItemSelected(true);
			}

			// cache year items rect transform to force scroll
			m_rectYearItems.Add(year, go.transform as RectTransform);
        }

		//register event
		HAGODateTimePickerControl.Api.OnYearSelectedEvent += OnYearSelectedHandler;
	}

	/// <summary>
    /// Handler year selected
    /// </summary>
	private void OnYearSelectedHandler (int year)
	{
		yearSelected = year;
	}

	/// <summary>
    /// Force scrollView focus current selected object
    /// </summary>
    private IEnumerator ScrollTo (RectTransform target)
    {
		float offsetY = 128f;

        Canvas.ForceUpdateCanvases();
		yield return new WaitForEndOfFrame();

		RectTransform contentPanel = m_yearContent as RectTransform;
        contentPanel.anchoredPosition3D = new Vector2
		(
			contentPanel.anchoredPosition.x,
			((Vector2) m_scrollRect.transform.InverseTransformPoint(contentPanel.position)
			- (Vector2) m_scrollRect.transform.InverseTransformPoint(target.position)).y 
			- offsetY
		);
    }

	/// <summary>
    /// Force scrollView focus current yearItem
    /// </summary>
	public void ForceScrollToYearSelected ()
	{
		StartCoroutine(ScrollTo(m_rectYearItems[yearSelected]));
	}
}