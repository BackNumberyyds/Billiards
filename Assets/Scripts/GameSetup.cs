using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSetup : MonoBehaviour
{
    private static readonly Ball.BallType[] BallSpawningOrder =
    {
        Ball.BallType.SolidBall,
        Ball.BallType.SolidBall,
        Ball.BallType.StripedBall,
        Ball.BallType.StripedBall,
        Ball.BallType.BlackBall,
        Ball.BallType.SolidBall,
        Ball.BallType.StripedBall,
        Ball.BallType.SolidBall,
        Ball.BallType.StripedBall,
        Ball.BallType.StripedBall,
        Ball.BallType.StripedBall,
        Ball.BallType.SolidBall,
        Ball.BallType.StripedBall,
        Ball.BallType.SolidBall,
        Ball.BallType.SolidBall
    };
    // private int solidBallsRemaining = 7;
    // private int stripedBallsRemaining = 7;

    // game object props
    private Ball _cueBall;
    private CueStick _cueStick;
    private Ball[] _balls = new Ball[16];
    private Camera _camera;
    private Plane _mPlane;
    private EventSystem _eventSystem;

    // UI props
    private HitController _hitController;

    // game object prefabs
    [Header("Game Object Prefabs")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform cueStickPrefab;

    // UI prefabs
    [Header("UI Prefabs")]
    [SerializeField] private HitController hitControllerPrefab;

    // spawning position
    [Header("Spawning Position")]
    [SerializeField] private Transform cueBallPosition;
    [SerializeField] private Transform headBallPosition;

    // ball meshes
    [Header("Ball Meshes")]
    [SerializeField] private Mesh[] ballMeshs;

    // game logic
    private bool[] _isBallMoving = new bool[16];
    private float _ballRadius;

    private Vector3 _cueBallPos;
    private bool _rotateCueStick;
    private Vector3 _prevMPos;

    private bool _isMovingCueBall;

    // private bool _hasMovingBall
    // {
    //     get
    //     {
    //         for ()
    //     }
    // }

    // Start is called before the first frame update
    private void Awake()
    {
        _eventSystem = EventSystem.current;
    }

    private void Start()
    {
        _camera = Camera.main;
        _ballRadius = ballPrefab.GetComponent<SphereCollider>().radius;
        PlaceAllBalls();
        // attach callback function to OnBallStopped event
        Ball.OnBallStopped += HandleBallStopped;

        PlaceCueStick();
        _cueStick.SetCueBall(_cueBall);
        CreateHitController();
        _hitController.SetCueStick(_cueStick);
        // PlaceRandomBalls();
        // Debug.Break();

        // Create a new plane with normal (0,1,0) at the position away from the camera you define in the Inspector
        // This is the plane that you can click so make sure it is reachable.
        _mPlane = new Plane(Vector3.up, new Vector3(0, _ballRadius, 0));
    }

    // Update is called once per frame
    private void Update()
    {
        // //Detect when there is a mouse click
        // if (Input.GetMouseButtonDown(0))
        // {
        //     //Create a ray from the Mouse click position
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //
        //     //Initialise the enter variable
        //     float enter = 0.0f;
        //
        //     if (_mPlane.Raycast(ray, out enter))
        //     {
        //         //Get the point that is clicked
        //         Vector3 hitPoint = ray.GetPoint(enter);
        //
        //         cueBallObj.GetComponent<Rigidbody>().AddForce(300f *  Vector3.Scale(new Vector3(1,0,1),(hitPoint - cueBallObj.transform.position)));
        //     }
        // }
    }

    private void FixedUpdate()
    {
        MoveCueBall();
        if (!_isMovingCueBall) RotateCueStick();
    }

    private void ResetCueStickPos()
    {
        _cueStick.transform.parent.position = _cueBall.transform.position;
    }

    private void HideCueStick()
    {
        _cueStick.GetComponent<MeshRenderer>().enabled = false;
    }

    private void ShowCueStick()
    {
        _cueStick.GetComponent<MeshRenderer>().enabled = true;
    }

    private void MoveCueBall()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            _isMovingCueBall = hitInfo.collider.GetComponent<Ball>();

            if (_isMovingCueBall)
            {
                HideCueStick();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isMovingCueBall = false;
            ResetCueStickPos();
            ShowCueStick();
        }

        if (!_isMovingCueBall) return;

        if (Input.GetMouseButton(0))
        {
            if (_mPlane.Raycast(ray, out var distance))
            {
                var intersectionPoint = ray.GetPoint(distance);
                _cueBall.transform.position = intersectionPoint;
            }
        }
    }

    private void HandleBallStopped(Ball ball)
    {
        _isBallMoving[ball.BallId] = true;
        // if (_hasMovingBall == false) {}
    }

    private void RotateCueStick()
    {
        if (_camera) _cueBallPos = _camera.WorldToScreenPoint(_cueBall.transform.position);
        if (Input.GetMouseButtonDown(0))
        {
            if (_eventSystem.IsPointerOverGameObject()) return;

            _rotateCueStick = true;
            _prevMPos = Input.mousePosition - _cueBallPos;
        }

        if (!_rotateCueStick) return;

        if (Input.GetMouseButtonUp(0))
        {
            _rotateCueStick = false;
        }

        if (!Input.GetMouseButton(0)) return;

        var curr = Input.mousePosition - _cueBallPos;
        var angle = -Vector2.SignedAngle(_prevMPos, curr);

        _prevMPos = curr;

        _cueStick.Rotate(angle);
    }

    private void PlaceAllBalls()
    {
        PlaceCueBall();
        PlaceRandomBalls();
    }

    private void PlaceCueBall()
    {
        var cueBallObj = Instantiate(ballPrefab, cueBallPosition.position, ballPrefab.transform.rotation);
        _cueBall = cueBallObj.GetComponent<Ball>();
        _cueBall.MakeCueBall(ballMeshs[0]);
        _balls[_cueBall.BallId] = _cueBall;
    }

    private void PlaceCueStick()
    {
        Vector3 cueBallPos = _cueBall.transform.position;
        var cueStickObj = Instantiate(cueStickPrefab, new Vector3(cueBallPos.x, _ballRadius, cueBallPos.z), ballPrefab.transform.rotation);
        _cueStick = cueStickObj.GetChild(0).GetComponent<CueStick>();
    }

    private void PlaceRandomBalls()
    {
        var solidOrder = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
        var stripedOrder = new List<int> { 9, 10, 11, 12, 13, 14, 15 };
        int solidIdx = 0, stripedIdx = 0;

        Shuffle(solidOrder);
        Shuffle(stripedOrder);

        var head = headBallPosition.position;
        for (int i = 0, k = 0; i < 5; i++)
        {
            for (var j = 0; j <= i; j++, k++)
            {
                var ball = Instantiate(ballPrefab, new Vector3(head.x + Mathf.Sqrt(3f) * _ballRadius * i, head.y,
                    head.z + (-1 * i + 2 * j) * _ballRadius), ballPrefab.transform.rotation).GetComponent<Ball>();
                switch (BallSpawningOrder[k])
                {
                    // GameObject ball = Instantiate(ballPrefab, new Vector3(head.x + (-1 * i + 2 * j) * _ballRadius, head.y,
                    //     head.z - Mathf.Sqrt(3f) * _ballRadius * i), ballPrefab.transform.rotation);
                    case Ball.BallType.BlackBall:
                        ball.MakeBlackBall(ballMeshs[8]);
                        break;
                    case Ball.BallType.SolidBall:
                        ball.MakeRandomBall(Ball.BallType.SolidBall, solidOrder[solidIdx], ballMeshs[solidOrder[solidIdx]]);
                        solidIdx++;
                        break;
                    default:
                        ball.MakeRandomBall(Ball.BallType.StripedBall, stripedOrder[stripedIdx],
                                ballMeshs[stripedOrder[stripedIdx]]);
                        stripedIdx++;
                        break;
                }

                _balls[ball.BallId] = ball;
            }
        }
    }

    private static void Shuffle(List<int> list)
    {
        var n = list.Count;

        for (var i = 0; i < n; i++)
        {
            var randomIndex = UnityEngine.Random.Range(i, n);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void CreateHitController()
    {
        var canvasObj = GameObject.FindWithTag("Canvas");
        _hitController = Instantiate(hitControllerPrefab, canvasObj.transform);
    }
}
