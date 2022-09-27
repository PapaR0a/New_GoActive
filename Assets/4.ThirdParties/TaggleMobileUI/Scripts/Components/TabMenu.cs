using System.Collections;
using System.Collections.Generic;
using SevenMins;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Taggle.HealthApp.Others;
using UnityEngine.Events;

namespace Taggle.HealthApp.Components
{
	public class TabMenu : MonoBehaviour {
		public CanvasGroup mainCanvas;
		public RectTransform rect;
		public GameObject prefBtnTab;
		public bool showTitle = false;
		/// <param name="colors">0: normal, 1: color ative</param>
		public Color[] colors;

		[SerializeField]
		public List<TabMenuItem> menuItems;
		public int curPanel = 0;
		public List<Button> btnTabMenus;
		public bool isHomeScene = false;

		// Use this for initialization
		void Start () {
			if(mainCanvas == null)
				mainCanvas = gameObject.GetComponent<CanvasGroup>();

			if(rect == null)
				rect = gameObject.GetComponent<RectTransform>();
				
			InitTabMenu();
		}

		public void InitTabMenu(){
			for(int i=0; i<menuItems.Count;i++){
				GameObject goItem = (GameObject)Instantiate(prefBtnTab);
				goItem.transform.SetParent(rect);
				goItem.name = i.ToString();

				Image icon = goItem.transform.Find("icon").GetComponent<Image>();
				icon.sprite = menuItems[int.Parse(goItem.name)].icon;
				icon.color = colors[0];

				Text title = goItem.transform.Find("title").GetComponent<Text>();
				title.enabled = showTitle;
				title.text = menuItems[int.Parse(goItem.name)].title;
				title.color = colors[0];

				if(menuItems[i].pnlContent != null){
					goItem.GetComponent<Button>().onClick.AddListener(()=>{
						OnClickBtnChangeTab(int.Parse(goItem.name));
					});
				}

				btnTabMenus.Add(goItem.GetComponent<Button>());
			}
			
			curPanel = -1;
			btnTabMenus[isHomeScene?GP.curTab:0].onClick.Invoke();
		}

		public void OnClickBtnChangeTab(int index){
			if(curPanel == index && curPanel >= 0)
				return;
			
			int prevPanel = curPanel<0?0:curPanel;
			curPanel = index;
			menuItems[prevPanel].pnlContent.FadeOutPnl(()=>{
				menuItems[curPanel].pnlContent.FadeInPnl(()=>{
					GP.curTab = index;
					menuItems[curPanel].OnClick.Invoke();
				}, GP.TIME_CHANGE_TAB);
			}, GP.TIME_CHANGE_TAB);

			btnTabMenus[prevPanel].transform.Find("icon").GetComponent<Image>().DOColor(colors[0], GP.TIME_FADE_ALPHA).OnComplete(()=>{
				btnTabMenus[curPanel].transform.Find("icon").GetComponent<Image>().DOColor(colors[1], GP.TIME_FADE_ALPHA);
			});

			btnTabMenus[prevPanel].transform.Find("title").GetComponent<Text>().DOColor(colors[0], GP.TIME_FADE_ALPHA).OnComplete(()=>{
				btnTabMenus[curPanel].transform.Find("title").GetComponent<Text>().DOColor(colors[1], GP.TIME_FADE_ALPHA);
			});
		}

		public void UpdateColorTab(int index, bool active){
			Image imgIcon = btnTabMenus[curPanel].transform.Find("icon").GetComponent<Image>();
			
		}
	}
}