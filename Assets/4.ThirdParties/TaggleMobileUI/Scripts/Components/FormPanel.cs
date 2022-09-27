using Taggle.HealthApp.Others;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace Taggle.HealthApp.Components{
    public class FormPanel : MonoBehaviour{
        public PanelCommon Pnl { get { return GetComponent<PanelCommon>(); } }
        private Dictionary<string, InputFieldComp> fieldDict;
        public List<string> FieldNames;
        public List<InputFieldComp> Fields;
        public Text CommonError;
        public delegate bool ValidationCallback(Dictionary<string, InputFieldComp> fields, out List<InputFieldComp> errFields, out string msg);
        
        // Use this for initialization
        void Start() {
            Pnl.FadeInPnl();
            // Convert to dict
            fieldDict = new Dictionary<string, InputFieldComp>();
            for (var i=0; i< FieldNames.Count; i++)
            {
                fieldDict.Add(FieldNames[i], Fields.Count>i ? Fields[i]: null);
            }
        }

        // Set error text with translation
        private void SetErrorText(string msg, bool translate, string[] tParams)
        {
            if(CommonError == null)
                return;
                
            if (translate)
                Utils.UpdateLocalisedText(CommonError, msg, tParams);
            else
                CommonError.text = msg;
        }

        // Get text of field
        public string GetFieldText(string fieldName)
        {
            return fieldDict.ContainsKey(fieldName) ? fieldDict[fieldName].Field.text : "";
        }

        public void ShowSuccessMsg(string msg, bool translate = true, string[] tParams=null)
        {
            SetErrorText(msg, translate, tParams);
            CommonError.color = InputFieldComp.SuccessColor;
        }

        public void ShowErrorByFieldName(string name, string msg, bool translate=true, string[] tParams = null)
        {
            ShowError(msg, fieldDict.ContainsKey(name) ? fieldDict[name] : null, translate);
        }

        public void ShowError(string msg, InputFieldComp f=null, bool translate=true, string[] tParams = null)
        {
            f?.InvalidInput();
            SetErrorText(msg, translate, tParams);
        }


        public void ResetForm(bool clearText = false)
        {
            Utils.UpdateLocalisedText(CommonError, "^_");
            foreach (var f in Fields)
            {
                f.ResetInput(clearText);
            }
        }

        public bool IsValid(ValidationCallback customCheck = null)
        {
            foreach (var f in Fields)
            {
                var msg = "";
                if (!f.IsValid(out msg))
                {
                    ShowError(msg, f);
                    return false;
                }
            }
            // get fields for callback
            if (customCheck != null) {
                var msg = "";
                var errFields = new List<InputFieldComp>();
                if (!customCheck(fieldDict, out errFields, out msg))
                {
                    if (errFields!=null && msg!="")
                    {
                        errFields.ForEach(f => {
                            f.InvalidInput();
                            Debug.Log("ffaaaaaa " + f.gameObject);
                        });
                        ShowError(msg);
                    }
                    return false;
                }
            }

            return true;
        }

    }

}