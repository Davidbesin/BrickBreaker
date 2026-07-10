using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class BallController : MonoBehaviour
{
    private Rigidbody rb;
    private SphereCollider sphere;

    [Header("Movement")]
    [SerializeField] private float speed = 4.5f;

    [Header("Bounce")]
    [SerializeField] private float minAngle = 0.2f;

    [Header("Collision")]
    [SerializeField] private float skinWidth = 0.001f;
    [SerializeField] private LayerMask collisionMask;

    public UnityEvent onCollision;

    private Vector3 direction;


    public Vector3 Direction
    {
        get => direction;
        set => direction = value.normalized;
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphere = GetComponent<SphereCollider>();
    }


    private void Start()
    {
        rb.useGravity = false;
        rb.isKinematic = true;
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


        float distance = speed * Time.fixedDeltaTime;


        RaycastHit hit;


        bool hasHit = Physics.SphereCast(
            rb.position,
            sphere.radius * transform.lossyScale.x,
            Direction,
            out hit,
            distance,
            collisionMask
        );


        // Nothing hit
        if (!hasHit)
        {
            rb.MovePosition(
                rb.position + Direction * distance
            );

            return;
        }


        // Move exactly to impact point
        Vector3 impactPoint =
            rb.position + Direction * hit.distance;


        HandleHit(hit, impactPoint);
    }



    private void HandleHit(RaycastHit hit, Vector3 impactPoint)
    {
        // Move to collision point first
        rb.MovePosition(
            impactPoint + hit.normal * skinWidth
        );


        // Paddle controls angle
        if (hit.collider.CompareTag("Paddle"))
        {
            float hitX =
                (hit.point.x - hit.collider.bounds.center.x) /
                (hit.collider.bounds.size.x * 0.5f);


            PaddlePush(hitX);

            onCollision?.Invoke();
            return;
        }



        // Brick damage
        if (hit.collider.CompareTag("Brick"))
        {
            Brick brick = hit.collider.GetComponent<Brick>();

            if (brick != null)
                brick.Damage();
        }



        // Bounce
        Direction =
            Vector3.Reflect(Direction, hit.normal)
            .normalized;



        // Prevent dead angles
        PreventDeadAngles();



        onCollision?.Invoke();
    }



    private void PreventDeadAngles()
    {
        Vector3 newDirection = Direction;


        // Prevent horizontal movement only
        if (Mathf.Abs(newDirection.y) < minAngle)
        {
            newDirection.y =
                Mathf.Sign(newDirection.y == 0 ? 1 : newDirection.y)
                * minAngle;
        }


        // Prevent vertical movement only
        if (Mathf.Abs(newDirection.x) < minAngle)
        {
            newDirection.x =
                Mathf.Sign(newDirection.x == 0 ? 1 : newDirection.x)
                * minAngle;
        }


        Direction = newDirection.normalized;
    }



    public void PaddlePush(float hitX)
    {
        hitX = Mathf.Clamp(hitX, -1f, 1f);

        Direction = new Vector3(hitX, 1f, 0f);

        PreventDeadAngles();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Failure"))
        {
            gameObject.SetActive(false);
        }
    }
}