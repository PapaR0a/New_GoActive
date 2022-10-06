using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GALoginView : MonoBehaviour
{
    private InputField m_Email = null;
    private InputField m_Password = null;
    private Button m_BtnLogin = null;

    // Start is called before the first frame update
    void Start()
    {
        m_Email = transform.Find("inputField_email").GetComponent<InputField>();
        m_Password = transform.Find("inputField_password").GetComponent<InputField>();
        m_BtnLogin = transform.Find("login").GetComponent<Button>();

        m_BtnLogin.onClick.AddListener(OnLogin);
    }

    private void OnLogin()
    {
        CPELoginControl.Api.Login(m_Email.text, m_Password.text);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            CPELoginControl.Api.Login("goactive14", "Aa123123!@#G14");
        }
    }

#endif
}
