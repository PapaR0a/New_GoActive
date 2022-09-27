using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Taggle.HealthApp.Components;
using Taggle.HealthApp.Others;
using UnityEngine;
using UnityEngine.UI;

namespace Taggle.HealthApp.Controllers {

    public class PopupController : MonoBehaviour {
        //public PopupPanel curPanel;
		public Sprite[] arrIcon;
        public string pnlOKPref= "prefPopup";
        public string pnlYesNoPref= "prefPopup";
        public string pnlUpdatePref = "prefPopup";
		public string pnlDisclaimPref = "prefPopupDisclaim";
		static PopupController instance = null;
		public static PopupController Instance
		{
			get
			{
				if(instance == null)
				{
					instance = UnityEngine.Object.FindObjectOfType<PopupController>();
				}
				return instance;
			}
		}

		// Use this for initialization
		void Start ()
		{
		}
		
		public void ShowPopUp(string content, PopupType typePopup, IconType type, Action callback)
		{
            // instanstiate based on type
            var prefType = typePopup == PopupType.OK ? pnlOKPref : (typePopup == PopupType.YesNo ? pnlYesNoPref : (typePopup == PopupType.Disclaim ? pnlDisclaimPref : pnlUpdatePref));
            GameObject goPnlReward = Instantiate(Resources.Load("Prefabs/Components/"+prefType)) as GameObject;
            goPnlReward.transform.SetParent(GameObject.FindGameObjectWithTag("bootCanvas").transform,false);
			goPnlReward.transform.localScale = Vector3.one;
			PopupPanel curPanel = goPnlReward.GetComponent<PopupPanel>();
            curPanel.InitPanel(type, content, callback);
            curPanel.ShowPopUp();
		}
	}
}

