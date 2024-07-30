using UnityEngine;

public class CueStick : MonoBehaviour
{
    private Transform tParent;

    private void Awake()
    {
        tParent = transform.parent;
    }

    public void Rotate(float angle)
    {
        Vector3 rotation = Vector3.up * angle;
        tParent.Rotate(rotation, Space.World);
    }
}
