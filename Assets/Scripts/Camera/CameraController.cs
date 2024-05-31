using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("Follow Settings")]
    [SerializeField] public Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private bool calculateOffsetAutomatically = true;

    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Ensure the target is assigned
        if (target == null)
        {
            Debug.LogError("Target is not assigned in CameraController.");
            return;
        }

        // Calculate offset if not set manually
        if (calculateOffsetAutomatically)
        {
            offset = transform.position - target.position;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate the desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera to the desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, followSpeed * Time.deltaTime);
    }
}