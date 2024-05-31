using System;
using UnityEngine;

public class PacketDecoder
{
    public event Action<string, object> OnPacketDecoded;
    private PacketRegistry _registry;

    public PacketDecoder(PacketRegistry registry)
    {
        _registry = registry;
    }

    private object DeserializePacket(SocketIOClient.SocketIOResponse response, Type packetType)
    {
        var method = typeof(SocketIOClient.SocketIOResponse).GetMethod("GetValue").MakeGenericMethod(packetType);
        return method.Invoke(response, new object[] { 0 });
    }

    private void HandlePacketResponse(string packetId, SocketIOClient.SocketIOResponse response)
    {
        try
        {
            var packetType = _registry.GetPacketType(packetId);
            var packet = DeserializePacket(response, packetType);
            Debug.Log("Received raw packet with ID " + packetId + " of type " + packet.GetType().Name);
            OnPacketDecoded?.Invoke(packetId, packet);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error handling packet " + packetId + ": " + ex);
        }
    }

    public void ListenForPackets(SocketIOClient.SocketIO client)
    {
        foreach (var packetId in _registry.GetPacketTypes())
        {
            Debug.Log("Registered packet ID " + packetId);
            client.On(packetId, response =>
            {
                HandlePacketResponse(packetId, response);
            });
        }
    }
}