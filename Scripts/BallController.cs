using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float speed = 8f;

    [Header("Bounce")]
    [SerializeField] private float angleThreshold = 0.1f;
    [SerializeField] private float randomOffset = 0.4f;

     public UnityEvent onCollision;

    private Vector3 direction;

    public Vector3 Direction
    {
        get => direction;
        set => direction = value.normalized;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        MoveBall();
    }

    private void MoveBall()
    {
        if (BallPaddleManager.Instance.currentState ==
            BallPaddleManager.BallState.onPaddle)
            return;

        rb.MovePosition(rb.position + Direction * speed * Time.fixedDeltaTime);
    }

    public void PaddlePush(float hitX)
    {
        // Clamp so the ball can never launch at an extreme angle
        hitX = Mathf.Clamp(hitX, -1f, 1f);

        Direction = new Vector3(hitX, 1f, 0f);
    }

    private void OnCollisionEnter(Collision collision)
        {
            // Paddle controls the outgoing angle
            if (collision.gameObject.CompareTag("Paddle"))
            {
                float hitX =
                    (transform.position.x - collision.collider.bounds.center.x) /
                    (collision.collider.bounds.size.x * 0.5f);

                PaddlePush(hitX);
                return;
            }

            // Reflect off walls and bricks
            Vector3 normal = collision.GetContact(0).normal;
            Vector3 before = Direction;

            Direction = Vector3.Reflect(Direction, normal).normalized;

            /* Debug.Log($"[{collision.gameObject.name}] tag={collision.gameObject.tag} " +
                    $"contactCount={collision.contactCount} normal={normal} " +
                    $"before={before} after={Direction}"); */

            // Log all contact points if there's more than one — helps spot bad-normal cases
            if (collision.contactCount > 1)
            {
                for (int i = 0; i < collision.contactCount; i++)
                {
                    Debug.Log($"  contact[{i}] normal={collision.GetContact(i).normal} point={collision.GetContact(i).point}");
                }
            }

            // Prevent almost vertical loops
            if (Mathf.Abs(Direction.x) < angleThreshold)
            {
                Direction += new Vector3(
                    Mathf.Sign(Direction.x == 0 ? Random.value - 0.5f : Direction.x) * randomOffset,
                    0f,
                    0f);
            }

            // Prevent almost horizontal loops
            if (Mathf.Abs(Direction.y) < angleThreshold)
            {
                Direction += new Vector3(
                    0f,
                    Mathf.Sign(Direction.y == 0 ? Random.value - 0.5f : Direction.y) * randomOffset,
                    0f);
            }

            // Normalize again
            Direction = Direction.normalized;

            if (collision.gameObject.CompareTag("Brick"))
            {
                Brick brick = collision.gameObject.GetComponent<Brick>();
                brick.Damage();
                
            }
        }
    

    private void OnTriggerEnter(Collider other)
    {
         if (other.CompareTag("Failure"))
        {
            gameObject.SetActive(false);
        }
    }
}