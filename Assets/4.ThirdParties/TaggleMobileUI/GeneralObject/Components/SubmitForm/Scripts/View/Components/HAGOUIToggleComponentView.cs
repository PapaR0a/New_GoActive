using System;
using DG.Tweening;
using Honeti;
using UnityEngine;
using UnityEngine.UI;

public class HAGOUIToggleComponentView : MonoBehaviour, HAGOUIIComponent
{
    public bool isInitByUser = false; //using for dynamic init
    [Space(12)] //blanck space on inspector

    public Image rectTgl;
    public Image imgTglHandler;
    private CanvasGroup m_canvas;
    private Toggle m_tglControl;
    public Toggle TglControl
    {
        get { return m_tglControl; }
    }
    private Text m_txtTitle;
    private HAGOUIFormItemStatusView m_formItem;

    //param
    public Color colorOn;
    public Color colorOff;
    private float posOff; //default tglHandler off position 
    private float posOn; //default tglHandler on position
    //
    private HAGOUIToggleOptionDTO m_data;
    private bool m_isInitComplete = false;
    private bool m_isEditMode;

    void Start()
    {
        if(!isInitByUser)
        {
            Init(null, true);
        }
    }

    ///<summary>
    ///using for dynamic instantiate
    ///</summary>
    public void Init(object data, bool isEditMode)
    {
        if(m_isInitComplete)
        {
            Debug.LogError(this.GetType().Name + " already init! Please set isInitByUser = false in inspector if init via script.");
            return;
        }

        this.m_data = (HAGOUIToggleOptionDTO)data;
        this.m_isEditMode = isEditMode;

        //find reference
        m_canvas = GetComponent<CanvasGroup>();
        m_tglControl = GetComponent<Toggle>() ?? null;
        m_formItem = GetComponent<HAGOUIFormItemStatusView>() ?? null;
        m_txtTitle = transform.Find("Title/TxtTitle")?.GetComponent<Text>() ?? null;
        
        //handle general value
        if (imgTglHandler != null && rectTgl != null)
        {
            LayoutElement layoutRectTgl = rectTgl.GetComponent<LayoutElement>();
            posOff = (imgTglHandler.transform as RectTransform).anchoredPosition.x;
            posOn = layoutRectTgl.preferredWidth - (imgTglHandler.transform as RectTransform).sizeDelta.x - posOff;
        }
        //
        m_canvas.interactable = m_isEditMode;
		//
        if(this.m_data != null) //handle dynamic UI value
        {
            //set title
            SetTitle(this.m_data.Title);

            //set key form response error
            if(m_formItem != null)
            {
                m_formItem.keyItem = this.m_data.KeyForm;
            }
            
            //set toggle is on or off
            if(m_tglControl != null)
            {
                SetToggle(m_data.DefaulValue);
            }

            //active gameobject because prefItem instantiate is disable by default
			gameObject.SetActive(true);
        }
        else
        {
            OnValueChanged(m_tglControl.isOn);
        }

        //add listener
        m_tglControl.onValueChanged.AddListener(OnValueChanged);

        m_isInitComplete = true;
    }

    private void SetToggle(bool isOn)
    {
        m_tglControl.isOn = isOn;
        //
        OnValueChanged(m_tglControl.isOn);
    }

    private void SetTitle(string content)
    {
        //set title
        if(m_txtTitle != null || !string.IsNullOrEmpty(content))
        {
            m_txtTitle.text = content;
        }
    }

    private void OnValueChanged(bool isOn)
    {
        if (rectTgl == null || imgTglHandler == null)
        {
            return;
        }

        DOTween.Complete(imgTglHandler.transform as RectTransform);
        DOTween.Complete(rectTgl);

        (imgTglHandler.transform as RectTransform).DOAnchorPosX(isOn ? posOn : posOff, 0.1f).OnStart(() => { m_tglControl.interactable = false; }).OnComplete(() => { m_tglControl.interactable = true; }).SetEase(Ease.OutExpo);
        rectTgl.DOColor(isOn ? colorOn : colorOff, 0.3f).SetEase(Ease.OutExpo);
    }

    public void SetInteractable(bool isOn)
    {
        m_canvas.interactable = isOn;
    }

    public long GetID()
    {
        return m_data != null ? m_data.ID : transform.GetSiblingIndex();
    }

    public bool GetValue()
    {
        return m_tglControl.isOn;
    }
    public void SetValue(bool value)
    {
        m_tglControl.isOn = value;
    }

    public string GetKeyForm()
	{
		if(m_formItem == null)
		{
			m_formItem = GetComponent<HAGOUIFormItemStatusView>();
		}

		return m_formItem != null ? m_formItem.keyItem : string.Empty;
	}

	public string GetJsonValue()
	{
		return m_tglControl.isOn.ToString();
	}

    public void ActiveError()
    {
        m_formItem.ActiveError();
    }

    public void ResetError()
	{
		m_formItem.ResetError();
	}

    public object ExportView(int id)
	{
        if(m_tglControl == null || m_txtTitle == null)
        {
            m_tglControl = GetComponent<Toggle>();
            m_txtTitle = transform.Find("Title/TxtTitle")?.GetComponent<Text>();
        }

		return new HAGOUIToggleOptionDTO(
            id,
            m_txtTitle.text,
            GetKeyForm(),
            GetValue()
        );
	}

    public string GetFormType()
    {
        return HAGOServiceKey.PARAM_TOGGLE_COMPONENT;
    }

	public void SetValue(string content)
	{
		try
		{
			bool isOn = bool.Parse(content);
			SetToggle(isOn);
		}
		catch(Exception ex)
		{
			Debug.Log("[HAToggleComponentView] Cannot parse value: " + ex.ToString());
		}
	}
}