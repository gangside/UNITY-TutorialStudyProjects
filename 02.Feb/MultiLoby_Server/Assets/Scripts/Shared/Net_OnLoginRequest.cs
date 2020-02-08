
[System.Serializable]
public class Net_OnLoginRequest : NetMsg {
    public Net_OnLoginRequest() {
        OP = NetOP.OnLoginRequest;
    }

    public byte Sucess { get; set; }
    public string Information { get; set; }

    public int ConnectionId { get; set; }
    public string Username { get; set; }
    public string Discriminator { get; set; }
    public string Token { get; set; }
}

