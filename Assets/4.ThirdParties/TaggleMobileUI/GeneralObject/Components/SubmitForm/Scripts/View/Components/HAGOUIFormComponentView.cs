using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Honeti;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HAGOUIFormComponentView : MonoBehaviour
{
    public bool isInitByUser = false; //using for dynamic init
    [Space(12)] //blanck space on inspector

    private CanvasGroup m_canvas;
    private ScrollRect m_scrollRect;
    private Transform m_content;
    private Button m_btnDelete;
    private Button m_btnSubmit;
    [HideInInspector]
    public Text txtError;
    //
    private GameObject m_prefDateTimeComp;
    private GameObject m_prefDateComp;
    private GameObject m_prefTimeComp;
    private GameObject m_prefDropdownComp;
    private GameObject m_prefCheckListComp;
    private GameObject m_prefToggleListComp;
    private GameObject m_prefToggleComp;
    private GameObject m_prefInputfieldComp;
    private GameObject m_prefInputfieldMultilineComp;
    private GameObject m_prefAttachmentComp;

    //param
    private HAGOUIFormComponentDTO m_data;
    private bool m_isInitComplete = false;
    private bool m_isEditMode;

    void Start()
    {
        if (!isInitByUser)
        {
            Init(true, null);
        }
    }

    void OnEnable()
    {
        //register event
        HAGOSubmitFormControl.Api.OnFormErrorEvent += OnFormErrorHandler;
    }

    void OnDisable()
    {
        //unregister event
        HAGOSubmitFormControl.Api.OnFormErrorEvent -= OnFormErrorHandler;
    }

    public void Init(bool isEditMode, HAGOUIFormComponentDTO data, ScrollRect overrideScrollRect = null)
    {
        m_data = data;
        m_isEditMode = isEditMode;

        if (!m_isInitComplete)
        {
            m_isInitComplete = true;

            //find reference
            m_canvas = GetComponent<CanvasGroup>();
            m_scrollRect = overrideScrollRect ?? GetComponent<ScrollRect>();
            m_content = transform.Find("Viewport/Content");
            m_btnSubmit = transform.Find("BtnSubmit")?.GetComponent<Button>();
            m_btnDelete = transform.Find("BtnDelete")?.GetComponent<Button>();
            txtError = transform.Find("TxtError").GetComponent<Text>();
            //
            m_prefDateTimeComp = Resources.Load<GameObject>(HAGOConstant.PREFAB_COMPONENT_DATETIME_ITEM);
            m_prefDateComp = Resources.Load<GameObject>(HAGOConstant.PREFAB_COMPONENT_DATE_ITEM);
            m_prefTimeComp = Resources.Load<GameObject>(HAGOConstant.PREFAB_COMPONENT_TIME_ITEM);
            m_prefDropdownComp = Resources.Load<GameObject>(HAGOConstant.PREFAB_COMPONENT_DROPDOWN_ITEM);
            m_prefCheckListComp = Resources.Load<GameObject>(HAGOConstant.PREFAB_COMPONENT_CHECK_LIST_ITEM);
            m_prefToggleListComp = Resources.Load<GameObject>(HAGOConstant.PREFAB_COMPONENT_TOGGLE_LIST_ITEM);
            m_prefToggleComp = Resources.Load<GameObject>(HAGOConstant.PREFAB_COMPONENT_TOGGLE_ITEM);
            m_prefInputfieldComp = Resources.Load<GameObject>(HAGOConstant.PREFAB_COMPONENT_INPUTFIELD_ITEM);
            m_prefInputfieldMultilineComp = Resources.Load<GameObject>(HAGOConstant.PREFAB_COMPONENT_INPUTFIELD_MULTILINE_ITEM);
            m_prefAttachmentComp = Resources.Load<GameObject>(HAGOConstant.PREFAB_COMPONENT_ATTACHMENT_ITEM);

            //handle view
            SetTextError(string.Empty);
            txtError.gameObject.SetActive(m_isEditMode);
            m_btnSubmit?.gameObject.SetActive(m_isEditMode);
            m_btnDelete?.gameObject.SetActive(m_isEditMode);

            //register event
            if (m_isEditMode)
            {
                m_btnSubmit?.onClick.AddListener(SubmitOnClick);
                m_btnDelete?.onClick.AddListener(DeleteOnClick);
            }
            else
            {
                Destroy(txtError.gameObject);
                Destroy(m_btnDelete?.gameObject);
            }
        }

        if (m_data != null) //handle dynamic UI value
        {
            InitDynamicComponents();
        }
    }

    private void DeleteOnClick()
    {
        HAGOTweenUtils.HidePopup(m_canvas, m_content, () =>
        {
            Debug.Log("//TODO: call action to update horizontal scroll snap child items");
        });
    }

    private void SubmitOnClick()
    {
        //TODO: handle submit report here
    }

    //using for get data for other custom form view (not from report scene)
    public HAGOUIFormDataResultDTO GetFormDataResult()
    {
        if (IsValid())
        {
            SetTextError(string.Empty);
            return new HAGOUIFormDataResultDTO(true, GetFormData());
        }
        else
        {
            SetTextError(I18N.instance.getValue(HAGOLangConstant.INVALID_FORM_SUBMISSION));
            return new HAGOUIFormDataResultDTO(false, null);
        }
    }

    private void OnFormErrorHandler(long formID, string error, Transform tfComp)
    {
        if (!m_isInitComplete)
        {
            return;
        }

        if (m_data.ID != formID)
        {
            return;
        }


        SetTextError(error);

        if (tfComp != null && gameObject.activeInHierarchy)
        {
            HAGOUtils.ScrollVerticalTo(m_scrollRect, m_scrollRect.content, tfComp as RectTransform);
        }
    }

    private void SetTextError(string error)
    {
        txtError.text = error;
    }

    private void InitDynamicComponents()
    {
        if (Application.isEditor)
        {
            while (m_content.childCount > 0)
            {
                DestroyImmediate(m_content.GetChild(0).gameObject);
            }
        }
        else
        {
            foreach (Transform tf in m_content)
            {
                if (m_btnSubmit != null && tf.gameObject != m_btnSubmit.gameObject)
                {
                    Destroy(tf.gameObject);
                }
            }
        }

        foreach (var formData in m_data.Data)
        {
            GameObject goPref = GetPrefComponent(formData.Type);

            if (goPref == null)
            {
                Debug.LogError("inst null " + formData.Type);
            }
            else
            {
                GameObject go = Instantiate(goPref, m_content);
                go.GetComponent<HAGOUIIComponent>().Init(formData.Data, m_isEditMode);
            }
        }

        if (m_isEditMode)
        {
            m_btnSubmit?.transform.SetAsLastSibling();
        }
    }

    private GameObject GetPrefComponent(string type)
    {
        switch (type)
        {
            case HAGOServiceKey.PARAM_DATETIME_COMPONENT:
                return m_prefDateTimeComp;

            case HAGOServiceKey.PARAM_DATE_COMPONENT:
                return m_prefDateComp;

            case HAGOServiceKey.PARAM_TIME_COMPONENT:
                return m_prefTimeComp;

            case HAGOServiceKey.PARAM_DROPDOWN_COMPONENT:
                return m_prefDropdownComp;

            case HAGOServiceKey.PARAM_CHECK_LIST_COMPONENT:
                return m_prefCheckListComp;

            case HAGOServiceKey.PARAM_TOGGLE_LIST_COMPONENT:
                return m_prefToggleListComp;

            case HAGOServiceKey.PARAM_TOGGLE_COMPONENT:
                return m_prefToggleComp;

            case HAGOServiceKey.PARAM_INPUTFIELD_COMPONENT:
                return m_prefInputfieldComp;

            case HAGOServiceKey.PARAM_INPUTFIELD_MULTILINE_COMPONENT:
                return m_prefInputfieldMultilineComp;

            case HAGOServiceKey.PARAM_ATTACHMENT_COMPONENT:
                return m_prefAttachmentComp;

            default:
                return null;
        }
    }

    public HAGOUIFormComponentDTO ExportView(int id = 1)
    {
        List<HAGOUIFormComponentItemDataDTO> rs = new List<HAGOUIFormComponentItemDataDTO>();

        int childID = 1;

        if (m_content == null)
        {
            m_content = transform.Find("Viewport/Content");
        }

        foreach (Transform tf in m_content)
        {
            if (m_btnSubmit != null && tf.gameObject == m_btnSubmit)
            {
                continue;
            }

            if (!tf.gameObject.activeSelf)
            {
                continue;
            }

            string typeStr = string.Empty;
            var formComp = tf.gameObject.GetComponent<HAGOUIIComponent>();
            if (formComp != null)
            {
                HAGOUIFormComponentItemDataDTO formDataDTO = new HAGOUIFormComponentItemDataDTO(formComp.GetKeyForm(), formComp.GetFormType(), formComp.ExportView(childID));
                rs.Add(formDataDTO);
                childID++;
            }
        }

        return new HAGOUIFormComponentDTO(id, rs);
    }

    private bool IsValid()
    {
        bool isValid = true;

        foreach (Transform tf in m_content)
        {
            var formComp = tf.gameObject.GetComponent<HAGOUIIComponent>();
            GameObject go = tf.gameObject;
            if (formComp != null)
            {
                switch (formComp.GetFormType())
                {
                    case HAGOServiceKey.PARAM_INPUTFIELD_COMPONENT:
                        isValid = go.GetComponent<HAGOUIInputFieldComponentView>().CheckValid();
                        HAGOUIIComponent iComp = go.GetComponent<HAGOUIIComponent>();
                        if (!isValid)
                        {
                            HAGOSubmitFormControl.Api.ActiveError(m_data.ID, string.Empty, iComp.GetKeyForm(), tf);
                            return isValid;
                        }
                        break;

                    case HAGOServiceKey.PARAM_DATETIME_COMPONENT:
                    case HAGOServiceKey.PARAM_DATE_COMPONENT:
                    case HAGOServiceKey.PARAM_TIME_COMPONENT:
                    case HAGOServiceKey.PARAM_DROPDOWN_COMPONENT:
                    case HAGOServiceKey.PARAM_CHECK_LIST_COMPONENT:
                    case HAGOServiceKey.PARAM_TOGGLE_LIST_COMPONENT:
                    case HAGOServiceKey.PARAM_TOGGLE_COMPONENT:
                    case HAGOServiceKey.PARAM_INPUTFIELD_MULTILINE_COMPONENT:
                    case HAGOServiceKey.PARAM_ATTACHMENT_COMPONENT:
                    default:
                        break;
                }
            }
        }

        return isValid;
    }

    private HAGOUIFormSubmitDataDTO GetFormData()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        foreach (Transform tf in m_content)
        {
            var formComp = tf.gameObject.GetComponent<HAGOUIIComponent>();
            GameObject go = tf.gameObject;
            if (formComp != null)
            {
                data.Add(formComp.GetKeyForm(), formComp.GetJsonValue());
            }
        }

        return new HAGOUIFormSubmitDataDTO(
            m_data.ID,
            data
        );
    }

    public void SetValue(string keyForm, string content)
    {
        HAGOUIIComponent ihaComp = m_content.GetComponentsInChildren<HAGOUIIComponent>().Where(x => x.GetKeyForm().Equals(keyForm)).FirstOrDefault();
        if (ihaComp != null)
        {
            ihaComp.SetValue(content);
        }
    }
}
