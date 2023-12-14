using UnityEngine;

public class SpellView : MonoBehaviour, ISyncEntity
{
    [SerializeField] private RectTransform rectTransform;

    public string spellName;
    private bool hasData = false;
    private float startPosition;
    private float targetPosition;
    private float startTime;
    private float targetTime;

    string ISyncEntity.name => spellName;

    void ISyncEntity.ReceiveSync(int step, IState state_)
    {
        var state = state_ as SpellState;
        startPosition = state.fromPlayer.position;
        targetPosition = state.toPlayer.position;
        startTime = state.startStep * Time.fixedDeltaTime;
        targetTime = state.endStep * Time.fixedDeltaTime;
        hasData = true;
    }

    void Update() {
        if (hasData) {
            float delta = (Time.time - startTime) / (targetTime - startTime);
            float pos = startPosition + (targetPosition - startPosition) * delta;
            rectTransform.localPosition = new Vector3(pos, 0, 0);
        }
    }
}
