using System;
using UnityEngine;

public class SpellView : MonoBehaviour, ISyncEntity
{
    [SerializeField] private RectTransform interpolatedTransform;
    [SerializeField] private RectTransform fixedTransform;

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

        // Fixed
        float delta = 1f - (state.endStep - step) / (float)(state.endStep - state.startStep);
        float pos = state.fromPlayer.position + (state.toPlayer.position - state.fromPlayer.position) * Math.Clamp(delta, 0, 1);
        fixedTransform.localPosition = new Vector3(pos, 0, 0);
    }

    void Update() {
        if (hasData) {
            // Interpolated
            float delta = (Time.time - startTime - Time.fixedDeltaTime) / (targetTime - startTime);
            float pos = startPosition + (targetPosition - startPosition) * Math.Clamp(delta, 0, 1);
            interpolatedTransform.localPosition = new Vector3(pos, 0, 0);
        }
    }
}
