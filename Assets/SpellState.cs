using System.Collections.Generic;

public class SpellState : IState {
    public PlayerEntity fromPlayer;
    public PlayerEntity toPlayer;
    public int startStep;
    public int endStep;

    string IState.Type => "SpellState";

    bool IState.ReadState(Dictionary<string, object> state)
    {
        fromPlayer = (PlayerEntity)state["fromPlayer"];
        toPlayer = (PlayerEntity)state["toPlayer"];
        startStep = (int)state["startStep"];
        endStep = (int)state["endStep"];
        return true;
    }

    bool IState.WriteState(Dictionary<string, object> state)
    {
        state.Add("fromPlayer", fromPlayer);
        state.Add("toPlayer", toPlayer);
        state.Add("startStep", startStep);
        state.Add("endStep", endStep);
        return true;
    }
}
