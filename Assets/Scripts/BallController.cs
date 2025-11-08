using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Ball Properties")]
    public float forceMultiplier = 100f;
    public float torqueMultiplier = 50f;
    public Transform throwPoint;

    private Rigidbody ballRb;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private bool isDragging = false;
    private bool canControl = true;
    private Vector3 initialPosition;

    void Start()
    {
        ballRb = GetComponent<Rigidbody>();
        initialPosition = transform.position;

        if (ballRb == null)
        {
            ballRb = gameObject.AddComponent<Rigidbody>();
            ballRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            ballRb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        ballRb.drag = 0.5f;
        ballRb.angularDrag = 0.5f;

        ResetBall();
    }

    void Update()
    {
        if (!canControl) return;

        if (Input.GetMouseButtonDown(0))
        {
            StartDragging();
        }
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            ReleaseBall();
        }
    }

    void StartDragging()
    {
        if (throwPoint == null)
        {
            Debug.LogError("ThrowPoint not set!");
            return;
        }

        startPoint = GetMouseWorldPosition();
        isDragging = true;
    }

    void ReleaseBall()
    {
        if (!canControl) return;

        endPoint = GetMouseWorldPosition();
        isDragging = false;

        Vector3 force = (endPoint - startPoint) * forceMultiplier;
        force.y = 0;

        ballRb.AddForce(force);
        AddBallSpin(force);
    }

    private void AddBallSpin(Vector3 forceDirection)
    {
        Vector3 torque = Vector3.zero;
        torque += Vector3.right * forceDirection.magnitude * torqueMultiplier * 0.5f;
        torque += Vector3.forward * -forceDirection.x * torqueMultiplier;
        ballRb.AddTorque(torque);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "ControlDisableTrigger")
        {
            canControl = false;
            isDragging = false;
            StartCoroutine(CalculateScoreAfterDelay());
        }
    }

    IEnumerator CalculateScoreAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CalculateScore();
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        if (throwPoint == null) return Vector3.zero;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(throwPoint.position).z;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    public void ResetBall()
    {
        if (ballRb != null)
        {
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }

        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
        canControl = true;
        isDragging = false;
    }

    public bool CanControl()
    {
        return canControl;
    }
}