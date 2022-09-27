using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Taggle.HealthApp.Others;
using UnityEngine;

namespace Taggle.HealthApp.Components
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PanelCommon : MonoBehaviour
    {
        public CanvasGroup PanelCanvas { get { return GetComponent<CanvasGroup>(); } }
        public CanvasGroup PanelContent;
        // Use this for initialization
        void Start()
        {
        }

        public PanelCommon ScaleInPnl(){
            CanvasGroup panel = (PanelContent != null? PanelContent: PanelCanvas);
            panel.gameObject.transform.DOScale(Vector3.one, GP.TIME_FADE_ALPHA).SetEase(Ease.OutExpo);
            return this;
        }

        public PanelCommon ScaleOutPnl(){
            CanvasGroup panel = (PanelContent != null? PanelContent: PanelCanvas);
            panel.gameObject.transform.DOScale(new Vector3(0.5f,0.5f,0.5f), GP.TIME_FADE_ALPHA).SetEase(Ease.OutExpo);
            return this;
        }

        public PanelCommon ShakePnl(){
            CanvasGroup panel = (PanelContent != null? PanelContent: PanelCanvas);
            panel.gameObject.transform.DOShakePosition(1f,3,10,90);
            return this;
        }

        public void FadeOutPnl(Action callback, float duration = 0.25f)
        {
            PanelCanvas.interactable = false;
            CanvasGroup panel = (PanelContent != null? PanelContent: PanelCanvas);
            panel.DOFade(0f, duration).SetEase(Ease.OutExpo).OnComplete(() =>
            {
                PanelCanvas.gameObject.SetActive(false);
                callback?.Invoke();
            });
        }

        public void FadeInPnl(Action callback, float duration = 0.25f)
        {
            PanelCanvas.interactable = false;
            PanelCanvas.gameObject.SetActive(true);
            (PanelContent != null?PanelContent:PanelCanvas).DOFade(1f, duration).SetEase(Ease.OutExpo).OnComplete(() =>
            {
                PanelCanvas.interactable = true;
                callback?.Invoke();
            });
        }

        public void FadeOutPnl(Action callback = null)
        {
            PanelCanvas.interactable = false;
            PanelCanvas.DOFade(0f, GP.TIME_FADE_ALPHA / 2f).OnComplete(() =>
            {
                PanelCanvas.gameObject.SetActive(false);
                callback?.Invoke();
            });
        }

        public void FadeInPnl(Action callback = null)
        {
            PanelCanvas.interactable = false;
            PanelCanvas.gameObject.SetActive(true);
            PanelCanvas.DOFade(1f, GP.TIME_FADE_ALPHA / 2f).OnComplete(() =>
            {
                PanelCanvas.interactable = true;
                callback?.Invoke();
            });
        }
    }
}