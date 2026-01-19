using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HitController : MonoBehaviour, IPointerDownHandler,
    IPointerUpHandler, IDragHandler
{
    public UnityEvent onHit = new();
    public UnityEvent onDragStart = new();
    public UnityEvent onDragEnd = new();
    private CueStick _cueStick;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void Reset()
    {
        _slider.value = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _cueStick.Pull(_slider.value);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onDragStart.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var hitPower = _slider.value;
        Reset();
        _cueStick.Pull(0);
        if (hitPower < 0.05)
        {
            onDragEnd.Invoke();
            return;
        }

        _cueStick.Hit(hitPower);
        onHit.Invoke();
        onDragEnd.Invoke();
    }

    public void SetCueStick(CueStick cueStick)
    {
        _cueStick = cueStick;
    }
}