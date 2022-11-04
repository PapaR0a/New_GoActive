using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class HAGOTimePickerScrollableItemView : MonoBehaviour
{
    private ScrollRect m_scrollView;
    private VerticalScrollSnap m_scrollSnap;
    private UI_InfiniteScroll m_infiniteScroll;
    //
    private GameObject m_prefItem;

    //param
    private int m_selectedValue;

    public IEnumerator Init(HAGOTimePickerScrollType timeType, int minValue = -1, int maxValue = -1)
    {
        //find reference
        m_scrollView = GetComponent<ScrollRect>();
        m_scrollSnap = GetComponent<VerticalScrollSnap>();
        m_infiniteScroll = GetComponent<UI_InfiniteScroll>();
        //
        m_prefItem = transform.Find("TxtItem").gameObject;
        m_prefItem.SetActive(false);

        //add listener
        m_scrollSnap.OnSelectionChangeEndEvent.AddListener(OnValueChangeHandler);

        //update view
        if(timeType == HAGOTimePickerScrollType.Hour)
        {
            yield return StartCoroutine(InitHourView(minValue, maxValue));
        }
        else if(timeType == HAGOTimePickerScrollType.Minute)
        {
            yield return StartCoroutine(InitMinuteView(minValue, maxValue));
        }
        else
        {
            yield return StartCoroutine(InitHalfDayPeriodView());
        }
    }

    private void OnValueChangeHandler(int value)
    {
        m_selectedValue = int.Parse(m_scrollSnap.ChildObjects[value].name);
    }

    private IEnumerator InitHourView(int minValue, int maxValue)
    {
        for(int i = minValue; i <= maxValue; i++)
        {
            GameObject go = CreateItem(i.ToString());
            m_scrollSnap.AddChild(go);
        }

        if(maxValue > 10)
        {
            yield return new WaitForEndOfFrame();
            m_infiniteScroll.Init();
        }

        yield return new WaitForEndOfFrame();
        m_scrollSnap.ChangePage(DateTime.Now.TimeOfDay.Hours < maxValue ? DateTime.Now.TimeOfDay.Hours : DateTime.Now.TimeOfDay.Hours - maxValue);
    }

    private IEnumerator InitMinuteView(int minValue, int maxValue)
    {
        for(int i = minValue; i <= maxValue; i++)
        {
            GameObject go = CreateItem(i.ToString());
            m_scrollSnap.AddChild(go);
        }
        
        if(maxValue > 10)
        {
            yield return new WaitForEndOfFrame();
            m_infiniteScroll.Init();
        }
        
        yield return new WaitForEndOfFrame();
        m_scrollSnap.ChangePage(DateTime.Now.TimeOfDay.Minutes);
    }

    private IEnumerator InitHalfDayPeriodView()
    {
        GameObject goPM = CreateItem("PM");
        m_scrollSnap.AddChild(goPM);

        GameObject goAM = CreateItem("AM");
        m_scrollSnap.AddChild(goAM);

        yield return new WaitForEndOfFrame();
        m_scrollSnap.ChangePage(DateTime.Now.ToString("tt") == "AM" ? 1 : 0);
    }

    private GameObject CreateItem(string name)
    {
        GameObject go = Instantiate(m_prefItem, m_scrollView.content);
        go.name = name == "AM" ? "0" : name == "PM" ? "1" : name;
        go.GetComponent<Text>().text = name;
        go.GetComponent<HAGOTimePickerTextOptionItemView>().Init(transform.position.y);
        go.SetActive(true);

        return go;
    }

    public int GetValue()
    {
        return m_selectedValue;
    }
}
