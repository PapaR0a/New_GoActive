using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HAGOSubmitFormControl
{
	private static HAGOSubmitFormControl m_api { get; set; }
    public static HAGOSubmitFormControl Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new HAGOSubmitFormControl();
            }
            return m_api;
        }
    }

	//events
	public Action<Action> ExitEvent;
	public Action<HAGOUIFormSubmitDataDTO> ResultCallbackEvent { get; set; }
	//
	public Action<long, string, Transform> OnFormErrorEvent { get; set; }
    public Action<string> OnFormItemErrorEvent { get; set; } //show error effect for form item (row, bar, inputfield,...) by keyItem
    public Action<string> OnFormItemResetColorEvent { get; set; }  //reset color form item. Reset all when string is empty

	public void ActiveGeneralError(long formID, string error)
	{
		ActiveError(formID, error, string.Empty, null);
	}

    public void ShowFormItemError(string keyItem)
    {
        OnFormItemErrorEvent?.Invoke(keyItem);
    }

	public void ActiveError(long formID, string error, string keyForm, Transform tfComp = null)
	{
		ShowFormItemError(keyForm);
		OnFormErrorEvent?.Invoke(formID, error, tfComp);
	}

    public void ResetFormColor()
    {
        OnFormItemResetColorEvent?.Invoke(string.Empty);
    }

	public void CompleteSubmitForm(HAGOUIFormSubmitDataDTO data)
    {
        ResultCallbackEvent?.Invoke(data);
        Exit();
    }

    public void Exit()
    {
        ExitEvent?.Invoke(() => HAGOSubmitFormManager.Api.Destroy());
    }
}