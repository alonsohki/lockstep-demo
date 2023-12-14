using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    public delegate void OnSyncDelegate(int step, Dictionary<string, Dictionary<string, object>> gameState);
    public event OnSyncDelegate onSync;

    public EntityManager entityManager { get; private set;} = new EntityManager();

    private int step = 0;

    private Dictionary<IActionable, string> pendingActions = new Dictionary<IActionable, string>();
    private List<ISyncEntity> syncEntities = new List<ISyncEntity>();


    public void AddSyncEntity(ISyncEntity syncEntity, IEntity entity) {
        if (entity is IActionable) {
            syncEntity.SetupSync(action => {
                pendingActions.Add(entity as IActionable, action);
            });
        }
        else {
            syncEntity.SetupSync(action => {});
        }
        syncEntities.Add(syncEntity);
    }

    public void RemoveSyncEntity(ISyncEntity syncEntity) {
        syncEntities.Remove(syncEntity);
    }

    void FixedUpdate()
    {
        entityManager.Update(pendingActions, step);
        pendingActions.Clear();

        var gameState = entityManager.GetGameState();

        syncEntities.ForEach(delegate (ISyncEntity receiver) {
            Dictionary<string, object> state;
            if (gameState.TryGetValue(receiver.name, out state)) {
                receiver.ReceiveSync(step, state);
            }
        });

        if (onSync != null) {
            onSync(step, gameState);
        }

        ++step;
    }
}
