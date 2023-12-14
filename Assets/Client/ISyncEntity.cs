public interface ISyncEntity {
    string name { get; }

    void ReceiveSync(int step, IState state);
}
