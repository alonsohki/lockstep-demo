using System.Collections.Generic;
using UnityEngine;

public class SpellView : MonoBehaviour, ISyncEntity
{
    [SerializeField] private RectTransform rectTransform;

    public string spellName;
    private SpellEntity spellEntity;

    string ISyncEntity.name => spellName;

    public void SetEntity(SpellEntity entity, int step) {
        spellEntity = entity;
        UpdatePosition(step);
    }

    void ISyncEntity.ReceiveSync(int step, Dictionary<string, object> state)
    {
        UpdatePosition(step);
    }

    private void UpdatePosition(int step) {
        var state = spellEntity.state;
        float delta = 1f - (state.endStep - step) / (float)(state.endStep - state.startStep);
        float pos = state.fromPlayer.position + (state.toPlayer.position - state.fromPlayer.position) * delta;
        rectTransform.localPosition = new Vector3(pos, 0, 0);
    }

    void ISyncEntity.SetupSync(ISyncEntity.SendSyncDelegate sendSync)
    {
    }
}
