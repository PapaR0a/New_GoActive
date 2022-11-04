using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HAGOTweenUtils : MonoBehaviour
{
    /// <summary>
    /// Fade in main canvas and scale in rect content
    /// </summary>
    /// <param name="canvas">Background canvas (usually with blur material), use for fade in</param>
    /// <param name="content">Content rect of view, use for scale in</param>
    /// <param name="callback">callback event after FadeIn() tween complete</param>
    public static void ShowPopup(CanvasGroup canvas, Transform content, Action callback = null)
    {
        DOTween.Complete(canvas);
        DOTween.Complete(content);

        FadeIn(canvas);
        ScaleIn(content, callback);
    }

    /// <summary>
    /// Fade out main canvas and scale out rect content
    /// </summary>
    /// <param name="canvas">Background canvas (usually with blur material), use for fade out</param>
    /// <param name="content">Content rect of view, use for scale out</param>
    /// <param name="callback">callback event after FadeOut() tween complete</param>
    public static void HidePopup(CanvasGroup canvas, Transform content, Action callback = null, bool forceDelete = true)
    {
        DOTween.Complete(canvas);
        DOTween.Complete(content);

        FadeOut(canvas, null, forceDelete);
        ScaleOut(content, callback);
    }

    /// <summary>
    /// Scale out object 2D
    /// </summary>
    /// <param name="content">Content rect object, use for scale out</param>
    /// <param name="callback">callback event after ScaleOut tween complete</param>
    public static void ScaleOut(Transform content, Action callback = null)
    {
        content.transform.localScale = Vector3.one;
        content.DOScale(Vector3.one * 0.75f, 0.3f).SetEase(Ease.OutExpo).OnComplete(() => { callback?.Invoke(); });
    }

    /// <summary>
    /// Scale in object 2D
    /// </summary>
    /// <param name="content">Content rect object, use for scale in</param>
    /// <param name="callback">callback event after ScaleIn tween complete</param>
    public static void ScaleIn(Transform content, Action callback = null)
    {
        content.transform.localScale = Vector3.one * 0.75f;
        content.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutExpo).OnComplete(() => { callback?.Invoke(); });
    }

    /// <summary>
    /// Fade in canvasGroup
    /// </summary>
    /// <param name="content">CanvasGroup rect object, use for fade in</param>
    /// <param name="callback">callback event after FadeIn tween complete</param>
    public static void FadeIn(CanvasGroup canvas, Action callback = null)
    {
        canvas.alpha = 0f;
        canvas.gameObject.SetActive(true);
        canvas.interactable = false;

        canvas.DOFade(1f, 0.3f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            canvas.interactable = true;
            callback?.Invoke();
        });
    }

    /// <summary>
    /// Fade out canvasGroup
    /// </summary>
    /// <param name="content">CanvasGroup rect object, use for fade out</param>
    /// <param name="callback">callback event after FadeOut tween complete</param>
    public static void FadeOut(CanvasGroup canvas, Action callback = null, bool forceDelete = true)
    {
        canvas.alpha = 1f;
        canvas.gameObject.SetActive(true);
        canvas.interactable = false;

        canvas.DOFade(0f, 0.3f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            callback?.Invoke();
            canvas.gameObject.SetActive(false);
            if (forceDelete)
            {
                Destroy(canvas.gameObject);
            }
        });
    }

    /// <summary>
    /// Shaking position object's Transform
    /// </summary>
    /// <param name="tf">object's Transform, use for shaking position</param>
    public static void DOShakeError(Transform tf)
    {
        DOTween.Complete(tf);
        tf.DOShakePosition(0.6f, new Vector3(12f,0,0), 30);
    }

    /// <summary>
    /// Shaking position object's Transform
    /// </summary>
    /// <param name="tf">object's Transform, use for shaking position</param>
    public static void DOColorError(Text txtItem)
    {
        DOTween.Complete(txtItem);
        txtItem.DOColor(HAGOUtils.ParseColorFromString(HAGOConstant.COLOR_ERROR), 0.3f);
    }
}