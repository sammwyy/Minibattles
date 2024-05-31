using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }

    [Header("Entity prefabs")]
    [SerializeField] private GameObject _playerEntityPrefab;

    private Dictionary<int, Entity> _entities;

    void Awake()
    {
        _entities = new Dictionary<int, Entity>();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (_playerEntityPrefab == null)
        {
            Debug.LogError("_playerEntityPrefab is not assigned in the inspector!");
        }
        else
        {
            Debug.Log("_playerEntityPrefab is assigned: " + _playerEntityPrefab.name);
        }
    }

    public GameObject GetPrefabFromEntityType(EntityType entityType)
    {
        switch (entityType)
        {
            case EntityType.PLAYER:
                return _playerEntityPrefab;
            default:
                Debug.LogWarning("Entity type " + entityType + " not recognized.");
                return null;
        }
    }

    public void RegisterEntity(Entity entity)
    {
        int id = entity.ID;
        _entities.Add(id, entity);
    }

    public Entity GetEntity(int entityId)
    {
        Entity value;
        if (_entities.TryGetValue(entityId, out value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }

    private void FocusEntitySync(int entityId, bool takeControl)
    {
        Entity entity = GetEntity(entityId);
        if (entity == null)
        {
            Debug.LogError("Attempt to focus an unknown entity (" + entityId + ")");
            return;
        }

        if (takeControl)
        {
            entity.isControllable = true;
        }

        CameraController.Instance.target = entity.transform;
    }

    public void FocusEntity(int entityId, bool takeControl)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => FocusEntitySync(entityId, takeControl));
    }

    private void RemoveEntity(int entityId)
    {
        Entity entity = GetEntity(entityId);
        if (entity == null)
        {
            Debug.LogError("Attempt to remove an unknown entity (" + entityId + ")");
            return;
        }

        entity.Remove();
    }

    public void RemoveEntitySync(int entityId)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => RemoveEntity(entityId));
    }

    public Entity SpawnEntity(EntityType entityType, int entityId, string label, Vector3 pos, Quaternion rot)
    {
        Debug.Log("Spawning entity of type " + entityType + " with ID " + entityId);
        GameObject prefab = GetPrefabFromEntityType(entityType);

        if (prefab == null)
        {
            Debug.LogError("Prefab for entity type " + entityType + " is null. Please check the prefab assignment in the inspector.");
            return null;
        }

        GameObject go = Instantiate(prefab, pos, rot);

        if (!go.TryGetComponent<Entity>(out var entity))
        {
            Debug.LogError("No Entity component found on instantiated prefab.");
            Destroy(go);
            return null;
        }

        entity.Init();
        entity.UpdateIdentity(entityType, entityId);
        entity.UpdateLabel(label);
        RegisterEntity(entity);
        return entity;
    }

    public void SpawnEntitySync(EntityType entityType, int entityId, string label, Vector3 pos, Quaternion rot)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Entity entity = SpawnEntity(entityType, entityId, label, pos, rot);
        });
    }


    public void UpdateEntityPosition(int entityId, Vector3 pos, Quaternion rot)
    {
        Entity entity = GetEntity(entityId);
        if (entity != null)
        {
            entity.Controller.UpdatePosition(pos);
            entity.Controller.UpdateRotation(rot);
        }
    }

    public void UpdateEntityPositionSync(int entityId, Vector3 pos, Quaternion rot)
    {

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UpdateEntityPosition(entityId, pos, rot);
        });
    }

    public void UpdateLocomotion(int entityId, EntityAnimator.LocomotionState locomotion)
    {
        Entity entity = GetEntity(entityId);
        if (entity != null)
        {
            entity.Animator.Locomotion = locomotion;
        }
    }

    public void UpdateLocomotionSync(int entityId, EntityAnimator.LocomotionState locomotion)
    {

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UpdateLocomotion(entityId, locomotion);
        });
    }
}
