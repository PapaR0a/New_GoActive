using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HAGOUIInputFieldContentType
{
    Standard,
    Integer,
    Decimal
}

public enum HAGOUIAttachmentType
{
    Audio,
    Image
}

public class HAGOSubmitFormModel
{
    private static HAGOSubmitFormModel m_api;
    public static HAGOSubmitFormModel Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new HAGOSubmitFormModel();
            }
            return m_api;
        }
    }
    
    public Dictionary<string, Type> ComponentDict = new Dictionary<string, Type>() {
        {HAGOServiceKey.PARAM_DATETIME_COMPONENT, typeof(HAGOUIDateTimeComponentDTO)},
        {HAGOServiceKey.PARAM_DATE_COMPONENT, typeof(HAGOUIDateTimeComponentDTO)},
        {HAGOServiceKey.PARAM_TIME_COMPONENT, typeof(HAGOUIDateTimeComponentDTO)},
        {HAGOServiceKey.PARAM_DROPDOWN_COMPONENT, typeof(HAGOUIDropdownDTO)},
        {HAGOServiceKey.PARAM_TOGGLE_COMPONENT, typeof(HAGOUIToggleOptionDTO)},
        {HAGOServiceKey.PARAM_CHECK_LIST_COMPONENT, typeof(HAGOUIToggleListDTO)},
        {HAGOServiceKey.PARAM_TOGGLE_LIST_COMPONENT, typeof(HAGOUIToggleListDTO)},
        {HAGOServiceKey.PARAM_INPUTFIELD_COMPONENT, typeof(HAGOUIInputFieldDTO)},
        {HAGOServiceKey.PARAM_INPUTFIELD_MULTILINE_COMPONENT, typeof(HAGOUIInputFieldDTO)},
        {HAGOServiceKey.PARAM_ATTACHMENT_COMPONENT, typeof(HAGOUIAttachmentDTO)},
    };

    public Dictionary<string, Type> ComponentViewDict = new Dictionary<string, Type>() {
        {HAGOServiceKey.PARAM_DATETIME_COMPONENT, typeof(HAGOUIDateTimeComponentView)},
        {HAGOServiceKey.PARAM_DATE_COMPONENT, typeof(HAGOUIDateComponentView)},
        {HAGOServiceKey.PARAM_TIME_COMPONENT, typeof(HAGOUITimeComponentView)},
        {HAGOServiceKey.PARAM_DROPDOWN_COMPONENT, typeof(HAGOUIDropdownComponentView)},
        {HAGOServiceKey.PARAM_TOGGLE_COMPONENT, typeof(HAGOUIToggleComponentView)},
        {HAGOServiceKey.PARAM_CHECK_LIST_COMPONENT, typeof(HAGOUICheckListComponentView)},
        {HAGOServiceKey.PARAM_TOGGLE_LIST_COMPONENT, typeof(HAGOUIToggleListComponentView)},
        {HAGOServiceKey.PARAM_INPUTFIELD_COMPONENT, typeof(HAGOUIInputFieldComponentView)},
        {HAGOServiceKey.PARAM_INPUTFIELD_MULTILINE_COMPONENT, typeof(HAGOUIInputFieldMultilineComponentView)},
        {HAGOServiceKey.PARAM_ATTACHMENT_COMPONENT, typeof(HAGOUIAttachmentItemView)},
    };
}