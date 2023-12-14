using System.Collections.Generic;

public interface IStateful
{
    void WriteState(Dictionary<string, object> state);

    void ReadState(Dictionary<string, object> state);
}
