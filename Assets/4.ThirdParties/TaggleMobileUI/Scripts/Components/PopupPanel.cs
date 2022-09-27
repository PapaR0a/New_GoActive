using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Taggle.HealthApp.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Taggle.HealthApp.Others;

namespace Taggle.HealthApp.Components
{
    public class PopupPanel : MonoBehaviour {
        // this popup types
        public PopupType Type = PopupType.OK;
        public IconType IType = IconType.normal;

        // game objects + unity components
        public Image IconImg;
        public Text PopupText;
        public PanelCommon Pnl { get { return GetComponent<PanelCommon>(); } }
        public Button btnYes;
        public Button btnNo;
        public Action actionYes;

        // extras for popup with check box
        public Toggle tglAgree;
        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        void InitIcon()
        {
            if (IType == IconType.normal)
            {
                IconImg.gameObject.SetActive(false);
            }
            else
            {
                IconImg.sprite = PopupController.Instance.arrIcon[(int)IType];
            }
        }

        void InitBtns ()
        {
            if (Type==PopupType.OK)
            {
                btnYes?.gameObject.SetActive(false);
                Utils.UpdateLocalisedText(btnYes.transform.Find("Text").GetComponent<Text>(), "^ok");
            }
            else if (Type == PopupType.Update)
            {
                btnYes?.gameObject.SetActive(false);
                Utils.UpdateLocalisedText(btnYes.transform.Find("Text").GetComponent<Text>(), "^update");
            }

            // default to auto close
            btnNo?.onClick.AddListener(() =>
            {
                ClosePopup();
            });
        }

        void SetCallback(Action callback)
        {
            actionYes = callback;
            btnYes.onClick.RemoveAllListeners();
            btnYes.onClick.AddListener((UnityEngine.Events.UnityAction)(() => 
            {
                if(Type == PopupType.Disclaim)
                {
                    if(IsDisclaimError())
                    {
                        DisclaimError();
                        return;
                    } 
                }
                ClosePopup();
                actionYes?.Invoke();
            }));
        }

        bool IsDisclaimError()
        {
            if(tglAgree != null)
            {
                return !tglAgree.isOn;
            }
            else
            {
                return false;
            }
        }

        #region Public
        public void InitPanel(IconType iconType, string content, Action callback)
        {
            IType = iconType;
            PopupText.text = content;
            SetCallback(callback);
            InitIcon();
            InitBtns();
        }

        public void DisclaimError()
        {
            Pnl.ShakePnl();
		}

        public void ShowPopUp()
        {
            Pnl.ScaleInPnl().FadeInPnl();
		}
		
		public void ClosePopup()
        {
            Pnl.ScaleOutPnl().FadeOutPnl(() =>
            {
                Destroy(gameObject);
            });
        }
        #endregion
    }
}
