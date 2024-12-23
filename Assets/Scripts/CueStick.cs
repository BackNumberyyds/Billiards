using UnityEngine;

public class CueStick : MonoBehaviour
{
    [SerializeField] private float maxPullDistance;
    [SerializeField] private float powerScale = 10f;
    private Ball _cueBall;
    private Transform tParent;
    private Vector3 _originalPos;

    private void Awake()
    {
        tParent = transform.parent;
        _originalPos = transform.localPosition;
    }

    public void SetCueBall(Ball b)
    {
        _cueBall = b;
    }

    public void Rotate(float angle)
    {
        Vector3 rotation = Vector3.up * angle;
        tParent.Rotate(rotation, Space.World);
    }

    public void Pull(float power)
    {
        var pos = _originalPos - Vector3.right * power * maxPullDistance;
        transform.localPosition = pos;
    }

    public void Hit(float power)
    {
        float angle = transform.eulerAngles.y * Mathf.PI / 180;
        Vector3 direction = new Vector3(-Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        _cueBall.GetComponent<Rigidbody>().AddForce(powerScale * power * direction);
    }
}
