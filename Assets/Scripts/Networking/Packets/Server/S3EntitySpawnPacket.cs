using UnityEngine;

public class S3EntitySpawnPacket
{
    public string label { get; set; }
    public int entityId { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public float rot { get; set; }
    public bool withFocus { get; set; }
    public bool withTakeControl { get; set; }
    public EntityType entityType { get; set; }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}