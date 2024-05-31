using System.Threading.Tasks;
using UnityEngine;

public class EntityAnimator : MonoBehaviour
{

    public enum LocomotionState
    {
        IDLE = 0,
        WALKING = 1,
        RUNNING = 2,
        FALLING = 3,
        JUMPING = 4
    }

    // References.
    [Header("References")]
    [SerializeField] private Entity _entity;
    [SerializeField] private Animator _animator;
    [SerializeField] private NetworkManager _networkManager;

    // State.
    [Header("State")]
    [SerializeField] private LocomotionState _previousLocomotionState;
    public LocomotionState Locomotion;

    async Task SendLocomotionUpdateIfNeeded(LocomotionState prev, LocomotionState curr)
    {
        if (_entity.isControllable)
        {
            C3LocomotionUpdatePacket packet = new(prev, curr);
            await _networkManager.SendPacket(packet);
        }
    }

    void HandleLocomotionUpdate()
    {

        if (_animator != null)
        {
            if (_previousLocomotionState != Locomotion)
            {
                _animator.SetInteger("Locomotion", (int)Locomotion);
                var _ = SendLocomotionUpdateIfNeeded(_previousLocomotionState, Locomotion);
                _previousLocomotionState = Locomotion;
            }
        }
    }

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _entity = GetComponent<Entity>();
        _networkManager = NetworkManager.Instance;

        if (_animator == null)
        {
            Debug.LogError("Animator not found on the GameObject or its children.");
        }

        _previousLocomotionState = LocomotionState.IDLE;
        Locomotion = LocomotionState.IDLE;
    }

    void Update()
    {
        HandleLocomotionUpdate();
    }
}