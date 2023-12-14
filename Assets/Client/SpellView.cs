using UnityEngine;

public class SpellView : MonoBehaviour, ISyncEntity
{
    [SerializeField] private RectTransform rectTransform;

    public string spellName;

    string ISyncEntity.name => spellName;

    void ISyncEntity.ReceiveSync(int step, IState state_)
    {
        var state = state_ as SpellState;
        float delta = 1f - (state.endStep - step) / (float)(state.endStep - state.startStep);
        float pos = state.fromPlayer.position + (state.toPlayer.position - state.fromPlayer.position) * delta;
        rectTransform.localPosition = new Vector3(pos, 0, 0);
    }
}
