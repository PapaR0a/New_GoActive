using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HAGOUIFormItemStatusView : MonoBehaviour
{
    public string keyItem;
    public Text m_txtContent;
    public Text m_txtLabel;
    public Image m_barLine;
    public Image m_icon;
    public Outline m_outline;

    //params
    private Color m_colorTxtContent;
    private Color m_colorBarLine;
    private Color m_colorIcon;
    private Color m_colorLabel;
    private Color m_colorOutline;

    // Use this for initialization
    private void Start()
    {
        Init();
    }

    private void OnDestroy()
    {
        HAGOSubmitFormControl.Api.OnFormItemResetColorEvent -= OnFormItemResetColorHandler;
        HAGOSubmitFormControl.Api.OnFormItemErrorEvent -= OnFormItemErrorHandler;
    }

    private void Init()
    {
        if (m_txtContent == null)
        {
            m_txtContent = transform.Find("Text")?.GetComponent<Text>();
        }
        m_colorTxtContent = m_txtContent != null ? m_txtContent.color : Color.white;

        if (m_barLine == null)
        {
            m_barLine = transform.Find("Bar")?.GetComponent<Image>();
        }
        m_colorBarLine = m_barLine != null ? m_barLine.color : Color.white;

        if (m_icon == null)
        {
            m_icon = transform.Find("Icon")?.GetComponent<Image>();
        }
        m_colorIcon = m_icon != null ? m_icon.color : Color.white;

        if (m_txtLabel == null)
        {
            m_txtLabel = transform.Find("Label")?.GetComponent<Text>();
        }
        m_colorLabel = m_txtLabel != null ? m_txtLabel.color : Color.white;

        m_colorOutline = m_outline != null ? m_outline.effectColor : Color.white;

        HAGOSubmitFormControl.Api.OnFormItemErrorEvent += OnFormItemErrorHandler;
        HAGOSubmitFormControl.Api.OnFormItemResetColorEvent += OnFormItemResetColorHandler;
    }

    private void OnFormItemErrorHandler(string keyItem)
    {
        // Debug.Log("OnFormItemErrorHandler: " + keyItem);

        if (this.keyItem != keyItem)
        {
            return;
        }

        ActiveError();
    }

    private void OnFormItemResetColorHandler(string keyItem)
    {
        if (keyItem != "" && this.keyItem != keyItem)
        {
            return;
        }

        ResetError();
    }

    public void ActiveError()
    {
        Color colorError = HAGOUtils.ParseColorFromString(HAGOConstant.COLOR_ERROR);

        SetContentColor(colorError);
        SetBarLineColor(colorError);
        SetIconColor(colorError);
        SetLabelColor(colorError);
        SetOutlineColor(colorError);

        HAGOTweenUtils.DOShakeError(transform);
    }

    public void ResetError()
    {
        SetContentColor(m_colorTxtContent);
        SetBarLineColor(m_colorBarLine);
        SetIconColor(m_colorIcon);
        SetLabelColor(m_colorLabel);
        SetOutlineColor(m_colorOutline);
    }

    private void SetBarLineColor(Color color)
    {
        if (m_barLine != null)
        {
            m_barLine.color = color;
        }
    }

    private void SetContentColor(Color color)
    {
        if (m_txtContent != null)
        {
            m_txtContent.color = color;
        }
    }

    private void SetIconColor(Color color)
    {
        if (m_icon != null)
        {
            m_icon.color = color;
        }
    }

    private void SetLabelColor(Color color)
    {
        if (m_txtLabel != null)
        {
            m_txtLabel.color = color;
        }
    }

    private void SetOutlineColor(Color color)
    {
        if (m_outline != null)
        {
            m_outline.effectColor = color;
        }
    }
}