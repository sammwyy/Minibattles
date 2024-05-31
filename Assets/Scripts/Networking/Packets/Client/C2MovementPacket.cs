using UnityEngine;

public class C2MovementPacket
{
    public float x { get; private set; }
    public float y { get; private set; }
    public float z { get; private set; }
    public float rot { get; private set; }

    public C2MovementPacket(Vector3 position, float rotation)
    {
        x = position.x;
        y = position.y;
        z = position.z;
        rot = rotation;
    }
}