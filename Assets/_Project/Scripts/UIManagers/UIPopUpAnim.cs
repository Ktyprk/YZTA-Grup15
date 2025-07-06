using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopUpAnim : MonoBehaviour
{
    [SerializeField] private Vector3 startVector = Vector3.zero, endVector = Vector3.one;

    [SerializeField] private float time = 1f;
    [SerializeField] private AnimationCurve curve;

    [SerializeField] private bool reverseAfterPop = false, useDefaultAsEndVector = false;
    [SerializeField] private float waitTime = 0;

    [SerializeField] private Transform targetOther;

    [HideInInspector] public bool isInAnim;

    public System.Action OnUnpopFinished;

    private void Awake()
    {
        if(useDefaultAsEndVector)
        {
            endVector = transform.localScale;
        }
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(Pop());
    }

    private IEnumerator Pop()
    {
        float step = 0;
        float maxStep = time;

        Vector3 start = startVector;
        Vector3 end = endVector;

        Transform target = transform;
        if (targetOther != null) target = targetOther;

        isInAnim = true;

        while (step < maxStep)
        {
            step += Time.fixedDeltaTime;

            target.localScale = Vector3.LerpUnclamped(start, end, curve.Evaluate(step / maxStep));

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        isInAnim = false;

        if (reverseAfterPop)
        {
            yield return new WaitForSecondsRealtime(waitTime);

            Unpop();
        }
    }

    public void Unpop(float time = -1)
    {
        if (gameObject == null) return;

        if (time == -1) time = this.time;

        StopAllCoroutines();

        Transform target = transform;
        if (targetOther != null) target = targetOther;

        if (target.gameObject.activeInHierarchy)
            StartCoroutine(ReversePop(time));
        else
            InstantUnpop();
    }

    public void InstantUnpop()
    {
        Transform target = transform;
        if (targetOther != null) target = targetOther;

        target.localScale = startVector;

        OnUnpopFinished?.Invoke();

        gameObject.SetActive(false);
    }

    private IEnumerator ReversePop(float duration)
    {
        isInAnim = true;

        float step = 0;
        float maxStep = duration;

        Vector3 start = endVector;
        Vector3 end = startVector;

        Transform target = transform;
        if (targetOther != null) target = targetOther;

        while (step < maxStep)
        {
            step += Time.fixedDeltaTime;

            target.localScale = Vector3.LerpUnclamped(start, end, curve.Evaluate(step / maxStep));

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        OnUnpopFinished?.Invoke();

        isInAnim = false;

        gameObject.SetActive(false);
    }
}
