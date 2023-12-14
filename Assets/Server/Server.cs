using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    public delegate void OnSyncDelegate(int step, Dictionary<string, GameState.SerializedEntry> gameState);
    public event OnSyncDelegate onSync;

    public EntityManager entityManager { get; private set;} = new EntityManager();

    private int step = 0;

    private Dictionary<IActionable, string> pendingActions = new Dictionary<IActionable, string>();

    public void ReceiveAction(string entityName, string action) {
        var entity = entityManager.FindEntity(entityName);
        if (entity != null && entity is IActionable) {
            pendingActions.Add(entity as IActionable, action);
        }
    }

    void FixedUpdate()
    {
        entityManager.Update(pendingActions, step);
        pendingActions.Clear();

        if (onSync != null) {
            onSync(step, entityManager.GetGameState());
        }

        ++step;
    }
}
