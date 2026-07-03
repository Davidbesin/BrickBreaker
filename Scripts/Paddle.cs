 using UnityEngine;
using UnityEngine.UI;

public class Paddle : MonoBehaviour
{
    public static Paddle Instance { get; private set; }

    [SerializeField] private GameObject rocketBullet;
    [SerializeField] private float sensitivity = 0.01f;
    [SerializeField] private float minX = -9f;
    [SerializeField] private float maxX = 9f;

    int bullet;
    [SerializeField] private float rocketSpeed = 15f;


    private Vector2 dragStart;
    private Vector3 paddleStart;
    private bool dragging;

    public void AddBullet()
    {
        bullet++;
    }
    private void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnTap += Fire;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnTap -= Fire;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }
    public void Fire()
    {
        if (bullet <= 0)
            return;

        bullet--;

        GameObject rocket = Instantiate(rocketBullet, transform.position, Quaternion.identity);

        Rigidbody rb = rocket.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.up * rocketSpeed;
        }
    }
    public void Drag(Vector2 screenPosition)
    {
        if (!dragging)
        {
            dragging = true;
            dragStart = screenPosition;
            paddleStart = transform.position;
        }

        float deltaX = screenPosition.x - dragStart.x;

        Vector3 newPosition = paddleStart + Vector3.right * deltaX * sensitivity;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        transform.position = newPosition;
    }

    public void EndDrag()
    {
        dragging = false;
    }

    // Called by the Slider's On Value Changed event
    public void SetSensitivity(float value)
    {
        sensitivity = value;
    }
    [SerializeField] float moveSpeed;
    public void KeyboardMove(float horizontal)
    {
        if (horizontal == 0f)
            return;

        Vector3 position = transform.position;
        position.x += horizontal * sensitivity* moveSpeed * Time.deltaTime;
        transform.position = position;
    }
}