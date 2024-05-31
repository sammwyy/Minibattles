using System;
using SocketIO.Serializer.NewtonsoftJson;
using UnityEngine;
using System.Threading.Tasks;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    public PacketDecoder PacketDecoder { get; private set; }
    public PacketRegistry PacketRegistry { get; private set; }

    [Header("Server connection")]
    public string Token = "";
    public string Server = "127.0.0.1:18412";
    [SerializeField] private bool _useSSL = false;
    [SerializeField] private bool _autoConnect = false;

    [Header("Network state")]
    [SerializeField] private bool _connected;
    [SerializeField] private bool _authenticated;

    private SocketIOClient.SocketIO _client;

    public event Action<string, object> OnPacketReceived;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PacketRegistry = new PacketRegistry(true);
            PacketDecoder = new PacketDecoder(PacketRegistry);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async Task SendPacket(object packet)
    {
        if (_client == null || !_connected)
        {
            Debug.LogWarning("Attempt to send a packet while client socket isn't connected. Will be ignored.");
            return;
        }

        string id = packet.GetType().Name;
        await _client.EmitAsync(id, packet);
        Debug.Log("Sending packet " + id + " to server.");
    }

    public void HandlePacketReceived(string id, object packet)
    {
        if (id == nameof(S1AuthSuccessPacket))
        {
            _authenticated = true;
        }

        OnPacketReceived?.Invoke(id, packet);
    }

    public void Connect()
    {
        string protocol = _useSSL ? "https://" : "http://";
        _client = new SocketIOClient.SocketIO(protocol + Server)
        {
            Serializer = new NewtonsoftJsonSerializer()
        };

        _client.OnConnected += async (sender, e) =>
        {
            _connected = true;
            await SendPacket(new C1AuthPacket(Token));
        };

        PacketDecoder.ListenForPackets(_client);
        PacketDecoder.OnPacketDecoded += HandlePacketReceived;
        _client.ConnectAsync();
    }

    private void Start()
    {
        if (_autoConnect)
        {
            Connect();
        }
    }

    private void OnDisable()
    {
        _client?.DisconnectAsync();
        PacketDecoder.OnPacketDecoded -= HandlePacketReceived;
    }
}