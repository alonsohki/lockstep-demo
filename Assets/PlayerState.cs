using System.Collections.Generic;

public class PlayerState : IState
{
    public PlayerState() {
        
    }
    public PlayerState(int hp, string action) {
        this.hp = hp;
        this.action = action;
    }

    public int hp { get; set; }
    public string action { get; set; }

    string IState.Type => "PlayerState";

    bool IState.ReadState(Dictionary<string, object> state)
    {
        hp = (int)state["hp"];
        action = (string)state["action"];
        return true;
    }

    bool IState.WriteState(Dictionary<string, object> state)
    {
        state.Add("action", action);
        state.Add("hp", hp);
        return true;
    }
}
