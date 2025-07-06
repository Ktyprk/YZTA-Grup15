using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static float scaleTime = 0.15f;
    private static Vector3 targetScale = Vector3.one * 1.15f;

    [SerializeField] private AnimationCurve animCurve;

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleAnim(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleAnim(false));
    }

    private IEnumerator ScaleAnim(bool toggle)
    {
        float step = 0;
        float maxStep = scaleTime;

        Vector3 start = toggle ? Vector3.one : targetScale;
        Vector3 end = toggle ? targetScale : Vector3.one;

        while (step < maxStep)
        {
            step += Time.deltaTime;

            transform.localScale = Vector3.LerpUnclamped(start, end, animCurve.Evaluate(step / maxStep));

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
