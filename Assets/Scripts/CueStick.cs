using System;
using UnityEngine;

public class CueStick : MonoBehaviour
{
    [SerializeField] private float maxPullDistance;
    [SerializeField] private float powerScale = 10f;
    private LineRenderer _aimLine;
    private Ball _cueBall;
    private MeshRenderer _modelRenderer;
    private Transform _modelTransform;
    private Vector3 _originalPos;

    private void Awake()
    {
        _aimLine = GetComponentInChildren<LineRenderer>();
        _modelTransform = transform.Find("Cuestick");
        if (_modelTransform == null)
            throw new InvalidOperationException(
                "CueStick model can't be null!");
        _modelRenderer = _modelTransform.GetComponent<MeshRenderer>();

        _originalPos = _modelTransform.localPosition;
    }

    private void Start()
    {
        if (_aimLine != null)
            UpdateAimLine();
    }

    private void UpdateAimLine()
    {
        _aimLine.SetPosition(0, transform.position);
        _aimLine.SetPosition(1, transform.position + transform.right);
    }

    public void SetCueBall(Ball b)
    {
        _cueBall = b;
    }

    public void Rotate(float angle)
    {
        var rotation = Vector3.up * angle;
        transform.Rotate(rotation, Space.World);

        if (_aimLine != null)
            UpdateAimLine();
    }

    public void Pull(float power)
    {
        var pos = _originalPos - Vector3.right * power * maxPullDistance;
        _modelTransform.localPosition = pos;
    }

    public void Hit(float power)
    {
        var angle = transform.eulerAngles.y * Mathf.PI / 180;
        var direction = new Vector3(Mathf.Cos(angle), 0f, -Mathf.Sin(angle));
        _cueBall.GetComponent<Rigidbody>()
            .AddForce(powerScale * power * direction);
    }

    public void Hide()
    {
        _modelRenderer.enabled = false;
        if (_aimLine != null)
            _aimLine.enabled = false;
    }

    public void Show()
    {
        _modelRenderer.enabled = true;
        if (_aimLine != null)
            _aimLine.enabled = true;
    }

    public void Transform(Vector3 pos)
    {
        transform.position = pos;
        UpdateAimLine();
    }
}