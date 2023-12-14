using System.Collections.Generic;

public interface ISyncEntity {
    public delegate void SendSyncDelegate(string action);

    string name { get; }

    void ReceiveSync(int step, Dictionary<string, object> state);
    void SetupSync(SendSyncDelegate sendSync);
}
