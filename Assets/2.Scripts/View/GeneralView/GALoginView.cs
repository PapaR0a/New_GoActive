using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GALoginView : MonoBehaviour
{
    public GameObject loginError;

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
        CPELoginControl.Api.Login(m_Email.text, m_Password.text, ToggleError);
    }

    public void ToggleError()
    {
        loginError.SetActive(!loginError.activeSelf);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            CPELoginControl.Api.Login("tester", "Aa123123!@#456");
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            CPELoginControl.Api.Login("goactive15", "Aa123123!@#G15");
        }
    }

#endif
}
