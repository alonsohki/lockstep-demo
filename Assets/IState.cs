using System.Collections.Generic;

public interface IState
{
    string Type { get; }

    bool WriteState(Dictionary<string, object> state);

    bool ReadState(Dictionary<string, object> state);
}
