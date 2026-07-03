using UnityEngine;
using UnityEngine.Events;

public class BallPaddleManager : MonoBehaviour
{
    public static BallPaddleManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform paddleHold;
    [SerializeField] private Transform ball;
    [SerializeField] private BallController[] ballControllers;
    [SerializeField] private BallController ballController;

    [Header("Events")]
    [SerializeField] private UnityEvent onFailure;

    public enum BallState
    {
        onPaddle,
        gaming
    }

    public BallState currentState;

    private bool failureTriggered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogError("InputManager.Instance is NULL!");
            return;
        }

        InputManager.Instance.OnTap += HandleTap;
        InputManager.Instance.OnPointerDrag += HandlePointerDrag;
        InputManager.Instance.OnKeyboardMove += HandleKeyboardMove;
        InputManager.Instance.OnDragEnd += HandleDragEnd;
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null)
            return;

        InputManager.Instance.OnTap -= HandleTap;
        InputManager.Instance.OnPointerDrag -= HandlePointerDrag;
        InputManager.Instance.OnKeyboardMove -= HandleKeyboardMove;
        InputManager.Instance.OnDragEnd -= HandleDragEnd;
    }

    private void Update()
    {
        if (currentState == BallState.onPaddle)
        {
            failureTriggered = false;
            ball.position = paddleHold.position;
        }
        else if (currentState == BallState.gaming)
        {
            if (!failureTriggered && !HasActiveBall())
            {
                failureTriggered = true;
                currentState = BallState.onPaddle;
                onFailure?.Invoke();
            }
        }
    }

    private bool HasActiveBall()
    {
        foreach (BallController controller in ballControllers)
        {
            if (controller.gameObject.activeInHierarchy)
                return true;
        }

        return false;
    }

    private void HandleTap()
    {
        if (currentState != BallState.onPaddle)
            return;

        LaunchBall();
    }

    private void HandlePointerDrag(Vector2 screenPosition)
    {
        if (Paddle.Instance != null)
            Paddle.Instance.Drag(screenPosition);
    }

    private void HandleKeyboardMove(float horizontal)
    {
        if (Paddle.Instance != null)
            Paddle.Instance.KeyboardMove(horizontal);
    }

    private void HandleDragEnd()
    {
        if (Paddle.Instance != null)
            Paddle.Instance.EndDrag();
    }

    private void LaunchBall()
    {
        currentState = BallState.gaming;
        failureTriggered = false;
        ballController.PaddlePush(0f);
    }

    public void BallOnPaddle()
    {
        ballController.gameObject.SetActive(true);
        currentState = BallState.onPaddle;
        failureTriggered = false;
    }

    [ContextMenu("Launch Multiple Balls")]
    public void LaunchMultipleBalls()
    {
        int count = ballControllers.Length;

        if (count == 0)
            return;

        Vector3 spawnPosition = Vector3.zero;

        foreach (BallController controller in ballControllers)
        {
            if (controller.gameObject.activeInHierarchy)
            {
                spawnPosition = controller.transform.position;
                break;
            }
        }

        currentState = BallState.gaming;
        failureTriggered = false;

        for (int i = 0; i < count; i++)
        {
            BallController controller = ballControllers[i];

            controller.transform.position = spawnPosition;
            controller.gameObject.SetActive(true);

            float hitX = (count == 1)
                ? 0f
                : Mathf.Lerp(-1f, 1f, (float)i / (count - 1));

            controller.PaddlePush(hitX);
        }
    }
}