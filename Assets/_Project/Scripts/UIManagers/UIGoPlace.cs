using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGoPlace : MonoBehaviour
{
    [SerializeField] private bool isWorldPos;
    [SerializeField] private float time = 0.15f;
    [SerializeField] private Vector3 startPos, endPos;
    [SerializeField] private AnimationCurve xCurve, yCurve;

    private RectTransform rect;

    private void OnEnable()
    {
        if (rect == null) rect = GetComponent<RectTransform>();

        StopAllCoroutines();
        StartCoroutine(GoPlace());
    }

    private IEnumerator GoPlace()
    {
        float step = 0;
        float maxStep = time;

        while (step < maxStep)
        {
            step += Time.deltaTime;

            Vector3 targetPos = new Vector3();
            targetPos.x = Mathf.LerpUnclamped(startPos.x, endPos.x, xCurve.Evaluate(step / maxStep));
            targetPos.y = Mathf.LerpUnclamped(startPos.y, endPos.y, yCurve.Evaluate(step / maxStep));
            targetPos.z = endPos.z;

            if (!isWorldPos)
                rect.anchoredPosition = targetPos;
            else
                transform.position = targetPos;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
