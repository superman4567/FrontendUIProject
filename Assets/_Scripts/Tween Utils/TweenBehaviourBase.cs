using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class TweenBehaviourBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    public TweenTriggerEnums trigger = TweenTriggerEnums.OnEnable;
    public Transform target;
    public bool viaCodeOnly = false;
    [Min(0f)] public float delay = 0f;
    [Min(0.01f)] public float duration = 0.2f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool ignoreTimeScale = true;
    public bool autoReverse = false;
    [Min(0)] public int loopCount = 0;
    public bool resetOnDisable = true;
    public bool captureStartFromCurrent = false;
    public UnityEvent onTweenStart;
    public UnityEvent onTweenComplete;
    public UnityEvent onReverseComplete;

    protected Coroutine running;
    protected bool isPlaying;
    bool _playReverse = false;

    protected abstract void CaptureStart();
    protected abstract void ApplyEvaluated(float t01);
    protected abstract void ApplyRaw(float raw01);
    protected abstract void ResetToStart();

    protected virtual void Awake()
    {
        if (captureStartFromCurrent)
            CaptureStart();
        if (trigger.HasFlag(TweenTriggerEnums.OnAwake) && !viaCodeOnly)
            Play();
    }

    protected virtual void OnEnable()
    {
        if (!captureStartFromCurrent)
            CaptureStart();
        if (trigger.HasFlag(TweenTriggerEnums.OnEnable) && !viaCodeOnly)
            Play();
    }

    protected virtual void OnDisable()
    {
        if (resetOnDisable)
        {
            StopRunning();
            ResetToStart();
        }
    }

    public void Play() => StartTween(false);
    public void PlayReverse() => StartTween(true);
    public void StopTween() => StopRunning();
    public void PreviewHalfRaw() => ApplyRaw(0.5f);

    void StartTween(bool reverse)
    {
        _playReverse = reverse;
        if (!isActiveAndEnabled) return;
        StopRunning();
        running = StartCoroutine(CoRunTween());
    }

    void StopRunning()
    {
        if (running != null)
        {
            StopCoroutine(running);
            running = null;
        }
        isPlaying = false;
}

    IEnumerator CoRunTween()
    {
        isPlaying = true;
        if (delay > 0f)
        {
            float tDelay = 0f;
            while (tDelay < delay)
            {
                tDelay += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
        }

        onTweenStart?.Invoke();

        int loopsLeft = loopCount;
        bool firstLeg = true;

        do
        {
            float t = _playReverse ? 1f : 0f;
            float dir = _playReverse ? -1f : 1f;
            float dur = Mathf.Max(0.0001f, duration);

            while (0f <= t && t <= 1f)
            {
                float dt = (ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) / dur;
                t += dt * dir;
                float eased = curve.Evaluate(Mathf.Clamp01(t));
                ApplyEvaluated(eased);
                yield return null;
            }

            ApplyEvaluated(_playReverse ? curve.Evaluate(0f) : curve.Evaluate(1f));
            if (_playReverse) onReverseComplete?.Invoke(); else onTweenComplete?.Invoke();

            if (firstLeg && autoReverse)
            {
                firstLeg = false;
                _playReverse = !_playReverse;
            }
            else if (loopsLeft > 0)
            {
                loopsLeft--;
                _playReverse = !_playReverse;
            }
            else break;

        } while (true);

        isPlaying = false;
        running = null;
    }

    public void OnPointerEnter(PointerEventData eventData) { if (!viaCodeOnly && trigger.HasFlag(TweenTriggerEnums.OnPointerEnter)) Play(); }
    public void OnPointerExit(PointerEventData eventData) { if (!viaCodeOnly && trigger.HasFlag(TweenTriggerEnums.OnPointerExit)) PlayReverse(); }
    public void OnPointerDown(PointerEventData eventData) { if (!viaCodeOnly && trigger.HasFlag(TweenTriggerEnums.OnPointerDown)) Play(); }
    public void OnPointerUp(PointerEventData eventData) { if (!viaCodeOnly && trigger.HasFlag(TweenTriggerEnums.OnPointerUp)) PlayReverse(); }
    public void OnPointerClick(PointerEventData eventData) { if (!viaCodeOnly && trigger.HasFlag(TweenTriggerEnums.OnClick)) Play(); }
    public void OnSelect(BaseEventData eventData) { if (!viaCodeOnly && trigger.HasFlag(TweenTriggerEnums.OnSelect)) Play(); }
    public void OnDeselect(BaseEventData eventData) { if (!viaCodeOnly && trigger.HasFlag(TweenTriggerEnums.OnDeselect)) PlayReverse(); }
}

    
