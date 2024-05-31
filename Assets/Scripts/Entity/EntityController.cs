using System.Threading.Tasks;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    [Header("References")]
    [SerializeField] private Transform model;

    private CharacterController _controller;
    private Entity _entity;
    private Vector3 _velocity;
    private bool _isGrounded;
    private NetworkManager _networkManager;
    private Vector3 _lastSentPosition;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _entity = GetComponent<Entity>();
        _networkManager = NetworkManager.Instance;
        _lastSentPosition = transform.position;
    }

    private void Update()
    {
        if (_entity.isControllable)
        {
            HandleMovement();
            HandleRotation();
            UpdateLocomotionState();
            _ = SendMovementUpdateIfNeeded();
        }

        if (_controller.enabled != _entity.isControllable)
        {
            _controller.enabled = _entity.isControllable;
        }
    }

    private void HandleMovement()
    {
        _isGrounded = _controller.isGrounded;
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // Ensure the player sticks to the ground.
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(horizontal, 0, vertical).normalized;
        _controller.Move(move * movementSpeed * Time.deltaTime);

        if (_isGrounded && Input.GetButtonDown("Jump"))
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            model.rotation = Quaternion.Lerp(model.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateLocomotionState()
    {
        if (_isGrounded)
        {
            float horizontal = Mathf.Abs(Input.GetAxis("Horizontal"));
            float vertical = Mathf.Abs(Input.GetAxis("Vertical"));
            bool moving = horizontal > 0.2f || vertical > 0.2f;

            if (moving)
            {
                _entity.Animator.Locomotion = EntityAnimator.LocomotionState.RUNNING;
            }
            else
            {
                _entity.Animator.Locomotion = EntityAnimator.LocomotionState.IDLE;
            }

            if (Input.GetButtonDown("Jump"))
            {
                _entity.Animator.Locomotion = EntityAnimator.LocomotionState.JUMPING;
            }
        }
        else
        {
            if (_velocity.y > 0)
            {
                _entity.Animator.Locomotion = EntityAnimator.LocomotionState.JUMPING;
            }
            else
            {
                _entity.Animator.Locomotion = EntityAnimator.LocomotionState.FALLING;
            }
        }
    }

    private async Task SendMovementUpdateIfNeeded()
    {
        Vector3 currentPosition = transform.position;
        if (Vector3.Distance(currentPosition, _lastSentPosition) > 0.01f)
        {
            _lastSentPosition = currentPosition;
            float rotation = model.eulerAngles.y;
            C2MovementPacket packet = new C2MovementPacket(currentPosition, rotation);
            await _networkManager.SendPacket(packet);
        }
    }

    public void UpdatePosition(Vector3 position)
    {
        transform.position = position;
    }

    public void UpdateRotation(Quaternion rotation)
    {
        model.rotation = rotation;
    }
}
