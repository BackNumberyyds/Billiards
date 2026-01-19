using System.Collections.Generic;
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

    // game object prefabs
    [Header("Game Object Prefabs")] [SerializeField]
    private GameObject ballPrefab;

    [SerializeField] private GameObject cueStickPrefab;

    // UI prefabs
    [Header("UI Prefabs")] [SerializeField]
    private HitController hitControllerPrefab;

    // spawning position
    [Header("Spawning Position")] [SerializeField]
    private Transform cueBallPosition;

    [SerializeField] private Transform headBallPosition;

    // ball meshes
    [Header("Ball Meshes")] [SerializeField]
    private Mesh[] ballMeshs;

    private readonly Ball[] _balls = new Ball[16];

    // game logic
    private readonly bool[] _isBallMoving = new bool[16];

    private float _ballRadius;

    private Camera _camera;
    // private int solidBallsRemaining = 7;
    // private int stripedBallsRemaining = 7;

    // game object props
    private Ball _cueBall;

    private Vector3 _cueBallPos;
    private CueStick _cueStick;
    private EventSystem _eventSystem;

    // UI props
    private HitController _hitController;

    private bool _isMovingCueBall;
    private int _movingCount;
    private Plane _mPlane;
    private Vector3 _prevMPos;
    private bool _rotatingCueStick;
    private bool _shotInProgress;

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
        // attach callback function to OnBallMoving/OnBallStopped event
        Ball.OnBallMoving += HandleBallMoving;
        Ball.OnBallStopped += HandleBallStopped;

        PlaceCueStick();
        _cueStick.SetCueBall(_cueBall);

        CreateHitController();
        _hitController.SetCueStick(_cueStick);
        _hitController.onHit.AddListener(HandleHit);
        // PlaceRandomBalls();
        // Debug.Break();

        // Create a new plane with normal (0,1,0) at the position away from the
        // camera you define in the Inspector
        // This is the plane that you can click so make sure it is reachable.
        _mPlane = new Plane(Vector3.up, new Vector3(0, _ballRadius, 0));
    }

    // Update is called once per frame
    private void Update()
    {
        MoveCueBall();
        if (!_isMovingCueBall) RotateCueStick();
    }

    private void FixedUpdate()
    {
    }

    private void OnDestroy()
    {
        Ball.OnBallMoving -= HandleBallMoving;
        Ball.OnBallStopped -= HandleBallStopped;
        _hitController.onHit.RemoveListener(HandleHit);
    }

    private void ResetCueStickPos()
    {
        // _cueStick.transform.position = _cueBall.transform.position;
        _cueStick.Move(_cueBall.transform.position);
    }

    private void MoveCueBall()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) &&
            Physics.Raycast(ray, out var hitInfo) &&
            hitInfo.collider.TryGetComponent<Ball>(out var ball))
        {
            _isMovingCueBall = true;
            _cueStick.Hide();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isMovingCueBall = false;
            ResetCueStickPos();
            _cueStick.Show();
        }

        if (!_isMovingCueBall) return;

        if (Input.GetMouseButton(0))
            if (_mPlane.Raycast(ray, out var distance))
            {
                var intersectionPoint = ray.GetPoint(distance);
                _cueBall.transform.position = intersectionPoint;
            }
    }

    private void HandleHit()
    {
        _cueStick.Hide();
    }

    private void HandleBallStopped(Ball ball)
    {
        var ballId = ball.BallId;
        if (ballId < 0 || ballId >= _isBallMoving.Length) return;
        if (!_isBallMoving[ballId]) return;
        _isBallMoving[ballId] = false;
        _movingCount--;
        if (_movingCount <= 0 && _shotInProgress)
        {
            _movingCount = 0;
            _shotInProgress = false;
            HandleAllBallsStopped();
        }
    }

    private void HandleBallMoving(Ball ball)
    {
        var ballId = ball.BallId;
        if (ballId < 0 || ballId >= _isBallMoving.Length) return;
        if (_isBallMoving[ballId]) return;
        _isBallMoving[ballId] = true;
        _movingCount++;
        _shotInProgress = true;
    }

    private void HandleAllBallsStopped()
    {
        Debug.Log("All balls stopped.");
        ResetCueStickPos();
        _cueStick.Show();
    }

    private void RotateCueStick()
    {
        if (_camera)
            _cueBallPos =
                _camera.WorldToScreenPoint(_cueBall.transform.position);
        if (Input.GetMouseButtonDown(0))
        {
            if (_eventSystem.IsPointerOverGameObject())
            {
                Debug.Log("On canvas");
                return;
            }

            _rotatingCueStick = true;
            _prevMPos = Input.mousePosition - _cueBallPos;

            return;
        }

        if (!_rotatingCueStick) return;

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))
        {
            var curr = Input.mousePosition - _cueBallPos;
            var angle = -Vector2.SignedAngle(_prevMPos, curr);

            _prevMPos = curr;

            _cueStick.Rotate(angle);
        }

        if (Input.GetMouseButtonUp(0))
            _rotatingCueStick = false;
    }

    private void PlaceAllBalls()
    {
        PlaceCueBall();
        PlaceRandomBalls();
    }

    private void PlaceCueBall()
    {
        var cueBallObj = Instantiate(ballPrefab, cueBallPosition.position,
            ballPrefab.transform.rotation);
        _cueBall = cueBallObj.GetComponent<Ball>();
        _cueBall.MakeCueBall(ballMeshs[0]);
        _balls[_cueBall.BallId] = _cueBall;
    }

    private void PlaceCueStick()
    {
        var cueBallPos = _cueBall.transform.position;
        var cueStickObj = Instantiate(cueStickPrefab,
            new Vector3(cueBallPos.x, _ballRadius, cueBallPos.z),
            ballPrefab.transform.rotation);
        _cueStick = cueStickObj.GetComponent<CueStick>();
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
        for (var j = 0; j <= i; j++, k++)
        {
            var ball = Instantiate(ballPrefab, new Vector3(
                    head.x + Mathf.Sqrt(3f) * _ballRadius * i, head.y,
                    head.z + (-1 * i + 2 * j) * _ballRadius),
                ballPrefab.transform.rotation).GetComponent<Ball>();
            switch (BallSpawningOrder[k])
            {
                // GameObject ball = Instantiate(ballPrefab, new Vector3(head.x + (-1 * i + 2 * j) * _ballRadius, head.y,
                //     head.z - Mathf.Sqrt(3f) * _ballRadius * i), ballPrefab.transform.rotation);
                case Ball.BallType.BlackBall:
                    ball.MakeBlackBall(ballMeshs[8]);
                    break;
                case Ball.BallType.SolidBall:
                    ball.MakeRandomBall(Ball.BallType.SolidBall,
                        solidOrder[solidIdx], ballMeshs[solidOrder[solidIdx]]);
                    solidIdx++;
                    break;
                default:
                    ball.MakeRandomBall(Ball.BallType.StripedBall,
                        stripedOrder[stripedIdx],
                        ballMeshs[stripedOrder[stripedIdx]]);
                    stripedIdx++;
                    break;
            }

            _balls[ball.BallId] = ball;
        }
    }

    private static void Shuffle(List<int> list)
    {
        var n = list.Count;

        for (var i = 0; i < n; i++)
        {
            var randomIndex = Random.Range(i, n);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void CreateHitController()
    {
        var canvasObj = GameObject.FindWithTag("Canvas");
        _hitController = Instantiate(hitControllerPrefab, canvasObj.transform);
    }
}