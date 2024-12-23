using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HitController : MonoBehaviour, IPointerUpHandler, IDragHandler
{
    private CueStick _cueStick;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void SetCueStick(CueStick cueStick)
    {
        _cueStick = cueStick;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var hitPower = _slider.value;
        _slider.value = 0;
        _cueStick.Pull(0);
        _cueStick.Hit(hitPower);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _cueStick.Pull(_slider.value);
    }
}
