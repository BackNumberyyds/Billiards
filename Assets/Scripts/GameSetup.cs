using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSetup : MonoBehaviour
{
    private Ball.BallType[] ballSpawningOrder =
    {
        Ball.BallType.SOLIDBALL,
        Ball.BallType.SOLIDBALL,
        Ball.BallType.STRIPEDBALL,
        Ball.BallType.STRIPEDBALL,
        Ball.BallType.BLACKBALL,
        Ball.BallType.SOLIDBALL,
        Ball.BallType.STRIPEDBALL,
        Ball.BallType.SOLIDBALL,
        Ball.BallType.STRIPEDBALL,
        Ball.BallType.STRIPEDBALL,
        Ball.BallType.STRIPEDBALL,
        Ball.BallType.SOLIDBALL,
        Ball.BallType.STRIPEDBALL,
        Ball.BallType.SOLIDBALL,
        Ball.BallType.SOLIDBALL
    };
    private int solidBallsRemaining = 7;
    private int stripedBallsRemaining = 7;
    private float ballRadius;
    private float ballDiameter;
    private Plane m_Plane;
    private GameObject cueBallObj;
    private CueStick cueStick;
    private Vector3 prevMPos;

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform cueStickPrefab;
    [SerializeField] private Transform cueBallPosition;
    [SerializeField] private Transform headBallPosition;
    [SerializeField] private Mesh[] ballMeshs;
    // Start is called before the first frame update
    void Start()
    {
        ballRadius = ballPrefab.GetComponent<SphereCollider>().radius;
        ballDiameter = ballRadius * 2f;
        PlaceAllBalls();
        PlaceCueStick();
        // Debug.Break();
        
        //Create a new plane with normal (0,1,0) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
        m_Plane = new Plane(Vector3.up, Vector3.zero);
    }

    // Update is called once per frame
    private void Update()
    {
        RotateCueStick();
        // //Detect when there is a mouse click
        // if (Input.GetMouseButtonDown(0))
        // {
        //     //Create a ray from the Mouse click position
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //
        //     //Initialise the enter variable
        //     float enter = 0.0f;
        //
        //     if (m_Plane.Raycast(ray, out enter))
        //     {
        //         //Get the point that is clicked
        //         Vector3 hitPoint = ray.GetPoint(enter);
        //
        //         cueBallObj.GetComponent<Rigidbody>().AddForce(300f *  Vector3.Scale(new Vector3(1,0,1),(hitPoint - cueBallObj.transform.position)));
        //     }
        // }
    }

    private void RotateCueStick()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        if (Input.GetMouseButtonDown(0))
        {
            prevMPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 diff = Input.mousePosition - prevMPos;
            if (diff == Vector3.zero) return;
            
            prevMPos = Input.mousePosition;
            float angle = 1;
            
            if (Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
            {
                if (diff.y > 0)
                {
                    angle *= -1;
                }
            }
            else
            {
                if (diff.x < 0)
                {
                    angle *= -1;
                }
            }
            
            cueStick.Rotate(angle);
        }
    }

    private void PlaceAllBalls()
    {
        PlaceCueBall();
        PlaceRandomBalls();
    }

    private void PlaceCueBall()
    {
        cueBallObj = Instantiate(ballPrefab, cueBallPosition.position, ballPrefab.transform.rotation);
        cueBallObj.GetComponent<Ball>().MakeCueBall(ballMeshs[0]);
    }

    private void PlaceCueStick()
    {
        Vector3 cueBallPos = cueBallObj.transform.position;
        var cueStickObj = Instantiate(cueStickPrefab, new Vector3(cueBallPos.x, ballRadius, cueBallPos.z), ballPrefab.transform.rotation);
        cueStick = cueStickObj.GetChild(0).GetComponent<CueStick>();
    }

    void PlaceBlackBall(Vector3 position)
    {
        GameObject ball = Instantiate(ballPrefab, position, ballPrefab.transform.rotation);
        ball.GetComponent<Ball>().MakeBlackBall(ballMeshs[8]);
    }

    void PlaceRandomBalls()
    {
        List<int> solidOrder = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
        List<int> stripedOrder = new List<int> { 9, 10, 11, 12, 13, 14, 15 };
        int solidIdx = 0, stripedIdx = 0;
        
        Shuffle(solidOrder);
        Shuffle(stripedOrder);
        
        Vector3 head = headBallPosition.position;
        for (int i = 0, k = 0; i < 5; i++)
        {
            for (int j = 0; j <= i; j++, k++)
            {
                GameObject ball = Instantiate(ballPrefab, new Vector3(head.x + Mathf.Sqrt(3f) * ballRadius * i, head.y,
                    head.z + (-1 * i + 2 * j) * ballRadius), ballPrefab.transform.rotation);
                // GameObject ball = Instantiate(ballPrefab, new Vector3(head.x + (-1 * i + 2 * j) * ballRadius, head.y,
                //     head.z - Mathf.Sqrt(3f) * ballRadius * i), ballPrefab.transform.rotation);
                if (ballSpawningOrder[k] == Ball.BallType.BLACKBALL)
                    ball.GetComponent<Ball>().MakeBlackBall(ballMeshs[8]);
                else if (ballSpawningOrder[k] == Ball.BallType.SOLIDBALL)
                {
                    ball.GetComponent<Ball>()
                        .MakeRandomBall(Ball.BallType.SOLIDBALL, solidOrder[solidIdx], ballMeshs[solidOrder[solidIdx]]);
                    solidIdx++;
                }
                else
                {
                    ball.GetComponent<Ball>()
                        .MakeRandomBall(Ball.BallType.STRIPEDBALL, stripedOrder[stripedIdx],
                            ballMeshs[stripedOrder[stripedIdx]]);
                    stripedIdx++;
                }
            }
        }
    }
    
    private void Shuffle(List<int> list)
    {
        int n = list.Count;

        for (int i = 0; i < n; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, n);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
