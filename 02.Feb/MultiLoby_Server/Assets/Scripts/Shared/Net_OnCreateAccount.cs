
[System.Serializable]
public class Net_OnCreateAccount : NetMsg {
    public Net_OnCreateAccount() {
        OP = NetOP.OnCreateAccount;
    }
    
    public byte Sucess { get; set; }
    public string Information { get; set; }
}
