using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Taggle.HealthApp.Components{
    public enum InputError
    {
        Empty = 1,
        InvalidEmail = 2,
        LongerThanMax = 3
    }

    public class InputFieldComp : MonoBehaviour {
        public static Color ErrorColor = new Color(255,126,121,255)*(1/255f);
        public static Color NormalColor = new Color(189,189,189,189)*(1/255f);
        public static Color SuccessColor = new Color(77,137,203,255)*(1/255f);

        //
        public Image Icon;
        public Text Label;
        public Text Placeholder;

        // have to be unique within a panel
        public InputField Field { get { return gameObject.GetComponent<InputField>(); } }
        public Image Bar;
        public List<InputError> Checks;
        public List<string> CheckParams;
        public List<string> ErrorMsgs;

        // Use this for initialization
        void Start () {
            // auto find sub components
            Icon = transform.Find("Icon").GetComponent<Image>();
            Label = transform.Find("Label").GetComponent<Text>();
            Placeholder = transform.Find("Placeholder").GetComponent<Text>();
            Bar = transform.Find("Bar").GetComponent<Image>();
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {return false;}
        }

        public static void SetColor(Color err, Color normal, Color success)
        {
            ErrorColor = err;
            NormalColor = normal;
            SuccessColor = success;
        }

        public bool IsValid(out string msg)
        {
            msg = "";
            for (var i=0; i<Checks.Count; i++)
            {
                var check = Checks[i];
                if (check == InputError.Empty && Field.text.Trim() == "")
                {
                    msg = ErrorMsgs[i];
                    return false;
                }
                    
                if (check == InputError.InvalidEmail && !IsValidEmail(Field.text))
                {
                    msg = ErrorMsgs[i];
                    return false;
                }   
            }
            return true;
        }

        public InputFieldComp InvalidInput()
        {
            Bar.color = InputFieldComp.ErrorColor;
            gameObject.transform.DOShakePosition(1f, 3, 10, 90);
            return this;
        }

        public InputFieldComp ResetInput(bool clearText=false)
        {
            Bar.color = InputFieldComp.NormalColor;
            if (clearText) Field.text = "";
            return this;
        }
    }

}