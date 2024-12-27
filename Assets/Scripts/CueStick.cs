using System;
using UnityEngine;

public class CueStick : MonoBehaviour
{
    [SerializeField] private float maxPullDistance;
    [SerializeField] private float powerScale = 10f;
    [SerializeField] private LineRenderer aimLine;
    private Ball _cueBall;
    private Transform _tParent;
    private Vector3 _originalPos;

    private void Awake()
    {
        _tParent = transform.parent;
        _originalPos = transform.localPosition;
    }

    private void Start()
    {
        DisplayAimLine();
    }

    private void DisplayAimLine()
    {
        aimLine.SetPosition(0, _tParent.position);
        aimLine.SetPosition(1, _tParent.position + _tParent.right);
    }
    
    public void SetCueBall(Ball b)
    {
        _cueBall = b;
    }

    public void Rotate(float angle)
    {
        var rotation = Vector3.up * angle;
        _tParent.Rotate(rotation, Space.World);
        DisplayAimLine();
    }

    public void Pull(float power)
    {
        var pos = _originalPos - Vector3.right * power * maxPullDistance;
        transform.localPosition = pos;
    }

    public void Hit(float power)
    {
        var angle = transform.eulerAngles.y * Mathf.PI / 180;
        var direction = new Vector3(Mathf.Cos(angle), 0f, -Mathf.Sin(angle));
        _cueBall.GetComponent<Rigidbody>().AddForce(powerScale * power * direction);
    }
}
