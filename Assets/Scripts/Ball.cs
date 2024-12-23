using System;
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
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        // GetComponent<Rigidbody>().velocity += Vector3.forward * 2;
        // GetComponent<Rigidbody>().velocity += Vector3.left * 1;
        // Debug.Log(GetComponent<Rigidbody>().velocity);
    }

    // Update is called once per frame
    void Update()
    {
        if (_rb.velocity.magnitude < 0.01f)
        {
            OnBallStopped?.Invoke(this);
        }
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
