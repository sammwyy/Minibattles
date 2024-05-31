using System;

public class C1AuthPacket
{
    public string token { get; private set; }

    public C1AuthPacket(string token)
    {
        this.token = token ?? throw new ArgumentNullException(nameof(token));
    }
}