using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public enum BallType
    {
        SOLIDBALL,
        STRIPEDBALL,
        CUEBALL,
        BLACKBALL
    }
    private int _ballId = 0;
    private BallType _ballType;
    // Start is called before the first frame update
    void Start()
    {
        // GetComponent<Rigidbody>().velocity += Vector3.forward * 2;
        // GetComponent<Rigidbody>().velocity += Vector3.left * 1;
        // Debug.Log(GetComponent<Rigidbody>().velocity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeCueBall(Mesh mesh)
    {
        _ballType = BallType.CUEBALL;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void MakeBlackBall(Mesh mesh)
    {
        _ballType = BallType.BLACKBALL;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void MakeRandomBall(BallType ballType, int ballId, Mesh mesh)
    {
        _ballId = ballId;
        _ballType = ballType;
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
