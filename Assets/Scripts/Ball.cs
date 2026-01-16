using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    public enum BallType
    {
        SolidBall,
        StripedBall,
        CueBall,
        BlackBall
    }
    
    private int _ballId;
    private BallType _ballType;
    private Rigidbody _rb;
    [SerializeField] private float stopSpeed = 0.02f;
    [SerializeField] private float stopAngularSpeed = 0.2f;
    [SerializeField] private float settleTime = 0.2f;
    private bool _isMoving;
    private float _stillTime;

    public int BallId
    {
        get => _ballId;
        set
        {
            if (value is >= 0 and <= 15)
            {
                _ballId = value;
            }
        }
    }
    
    public static UnityEngine.Events.UnityAction<Ball> OnBallStopped;
    public static UnityEngine.Events.UnityAction<Ball> OnBallMoving;
    
    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        // GetComponent<Rigidbody>().velocity += Vector3.forward * 2;
        // GetComponent<Rigidbody>().velocity += Vector3.left * 1;
        // Debug.Log(GetComponent<Rigidbody>().velocity);
    }

    private void FixedUpdate()
    {
        var moving = _rb.velocity.sqrMagnitude > stopSpeed * stopSpeed ||
                     _rb.angularVelocity.sqrMagnitude >
                     stopAngularSpeed * stopAngularSpeed;

        if (moving)
        {
            _stillTime = 0f;
            if (_isMoving) return;
            _isMoving = true;
            OnBallMoving?.Invoke(this);
            return;
        }

        _stillTime += Time.fixedDeltaTime;
        if (!_isMoving || _stillTime < settleTime) return;
        _isMoving = false;
        OnBallStopped?.Invoke(this);
    }

    public void MakeCueBall(Mesh mesh)
    {
        _ballType = BallType.CueBall;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void MakeBlackBall(Mesh mesh)
    {
        _ballType = BallType.BlackBall;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void MakeRandomBall(BallType ballType, int ballId, Mesh mesh)
    {
        _ballId = ballId;
        _ballType = ballType;
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
