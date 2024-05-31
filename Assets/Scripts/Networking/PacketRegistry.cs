using System;
using System.Collections.Generic;

public class PacketRegistry
{
    private readonly Dictionary<string, Type> _registeredPackets = new();

    public PacketRegistry(bool defaultRegisters)
    {
        if (!defaultRegisters)
        {
            return;
        }

        RegisterPacket<S1AuthSuccessPacket>();
        RegisterPacket<S2EntityMovementPacket>();
        RegisterPacket<S3EntitySpawnPacket>();
        RegisterPacket<S4EntityFocusPacket>();
        RegisterPacket<S5PlayerConnectPacket>();
        RegisterPacket<S6PlayerDisconnectPacket>();
        RegisterPacket<S7EntityLocomotionUpdatePacket>();
        RegisterPacket<S8EntityRemovePacket>();
    }

    public void RegisterPacket<T>()
    {
        string id = typeof(T).Name;
        if (!_registeredPackets.ContainsKey(id))
        {
            _registeredPackets.Add(id, typeof(T));
        }
    }

    public Type GetPacketType(string id)
    {
        _registeredPackets.TryGetValue(id, out var type);
        return type;
    }

    public IEnumerable<string> GetPacketTypes()
    {
        return _registeredPackets.Keys;
    }
}