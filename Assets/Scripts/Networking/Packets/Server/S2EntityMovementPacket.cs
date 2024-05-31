using UnityEngine;

public class S2EntityMovementPacket
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public float rot { get; set; }
    public int entityId { get; set; }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}