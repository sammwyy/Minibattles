using UnityEngine;

public class PacketHandler : MonoBehaviour
{
    private NetworkManager _networkManager;

    private void Start()
    {
        _networkManager = NetworkManager.Instance;
        _networkManager.OnPacketReceived += HandlePacket;
    }

    private void OnDestroy()
    {
        _networkManager.OnPacketReceived -= HandlePacket;
    }

    private void HandlePacket(string packetId, object packet)
    {
        Debug.Log("Received packet with ID " + packetId);

        switch (packetId)
        {
            case nameof(S1AuthSuccessPacket):
                HandleAuthSuccessPacket((S1AuthSuccessPacket)packet);
                break;
            case nameof(S2EntityMovementPacket):
                HandleEntityMovementPacket((S2EntityMovementPacket)packet);
                break;
            case nameof(S3EntitySpawnPacket):
                HandleEntitySpawnPacket((S3EntitySpawnPacket)packet);
                break;
            case nameof(S4EntityFocusPacket):
                HandleEntityFocusPacket((S4EntityFocusPacket)packet);
                break;
            case nameof(S7EntityLocomotionUpdatePacket):
                HandleEntityLocomotionUpdate((S7EntityLocomotionUpdatePacket)packet);
                break;
            case nameof(S8EntityRemovePacket):
                HandleEntityRemovePacket((S8EntityRemovePacket)packet);
                break;
        }
    }

    private void HandleAuthSuccessPacket(S1AuthSuccessPacket packet)
    {
        Debug.Log("Authentication successful: " + packet);
    }

    private void HandleEntityMovementPacket(S2EntityMovementPacket packet)
    {
        EntityManager EntityManager = EntityManager.Instance;
        if (EntityManager == null)
        {
            Debug.LogError("Attempt to move an entity while EntityManager isn't initialized yet.");
            return;
        }

        int entityId = packet.entityId;
        Vector3 pos = packet.ToVector3();
        Quaternion rot = Quaternion.Euler(0, packet.rot, 0);
        EntityManager.UpdateEntityPositionSync(entityId, pos, rot);
    }

    private void HandleEntitySpawnPacket(S3EntitySpawnPacket packet)
    {
        EntityManager EntityManager = EntityManager.Instance;
        if (EntityManager == null)
        {
            Debug.LogError("Attempt to spawn an entity while EntityManager isn't initialized yet.");
            return;
        }

        EntityType entityType = packet.entityType;
        int entityId = packet.entityId;
        string label = packet.label;
        Vector3 pos = packet.ToVector3();
        Quaternion rot = Quaternion.Euler(0, packet.rot, 0);
        EntityManager.SpawnEntitySync(entityType, entityId, label, pos, rot);
        if (packet.withFocus)
        {
            EntityManager.FocusEntity(packet.entityId, packet.withTakeControl);
        }
    }

    private void HandleEntityFocusPacket(S4EntityFocusPacket packet)
    {
        EntityManager EntityManager = EntityManager.Instance;
        if (EntityManager == null)
        {
            Debug.LogError("Attempt to focus an entity while EntityManager isn't initialized yet.");
            return;
        }

        EntityManager.FocusEntity(packet.entityId, packet.takeControl);
    }

    private void HandleEntityLocomotionUpdate(S7EntityLocomotionUpdatePacket packet)
    {
        EntityManager EntityManager = EntityManager.Instance;
        if (EntityManager == null)
        {
            Debug.LogError("Attempt to update an entity locomotion while EntityManager isn't initialized yet.");
            return;
        }

        EntityManager.UpdateLocomotionSync(packet.entityId, packet.locomotion);
    }

    private void HandleEntityRemovePacket(S8EntityRemovePacket packet)
    {
        EntityManager EntityManager = EntityManager.Instance;
        if (EntityManager == null)
        {
            Debug.LogError("Attempt to remove an entity while EntityManager isn't initialized yet.");
            return;
        }

        EntityManager.RemoveEntitySync(packet.entityId);
    }
}