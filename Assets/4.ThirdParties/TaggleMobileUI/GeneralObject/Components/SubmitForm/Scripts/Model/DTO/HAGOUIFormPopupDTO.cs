using System.Collections;
using System.Collections.Generic;
using Honeti;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HAGOUIFormPopupDTO
{
    public string Title { get; set; }
    public string Desc { get; set; }
    public HAGOUIFormComponentDTO FormData { get; set; }
    public bool IsRequireFormData { get { return FormData != null; } }
    public HAGOUIDisclaimerDTO  DisclaimerData { get; set; }
    public bool IsRequireDisclaimer { get { return DisclaimerData != null; } }

    public HAGOUIFormPopupDTO(){}
    public HAGOUIFormPopupDTO (JObject data)
    {
        Title = data.Value<string>(HAGOServiceKey.PARAM_TITLE);
        Desc = data.Value<string>(HAGOServiceKey.PARAM_DESC);
        //
        if(data.ContainsKey(HAGOServiceKey.PARAM_FORM_DATA))
        {
            JObject jo = data.Value<JObject>(HAGOServiceKey.PARAM_FORM_DATA);
            FormData = jo != null ? new HAGOUIFormComponentDTO(jo) : null;
        }
        else
        {
            FormData = null;
        }
        //
        if(data.ContainsKey(HAGOServiceKey.PARAM_DISCLAIMER_DATA))
        {
            JObject jo = data.Value<JObject>(HAGOServiceKey.PARAM_DISCLAIMER_DATA);
            DisclaimerData = jo != null ? new HAGOUIDisclaimerDTO(jo) : null;
        }
        else
        {
            DisclaimerData = null;
        }
    }
}

public class HAGOUIDisclaimerDTO
{
    public string TermsContent { get; set; }
    public string ToggleContent { get; set; }

    public HAGOUIDisclaimerDTO (JObject data)
    {
        TermsContent = data.Value<string>(HAGOServiceKey.PARAM_TERMS_CONTENT);
        //
        string tglContent = data.Value<string>(HAGOServiceKey.PARAM_TOGGLE_CONTENT);
        ToggleContent = !string.IsNullOrEmpty(tglContent) ? tglContent : I18N.instance.getValue(HAGOLangConstant.AGREE_DISCLAIMER);
    }
}