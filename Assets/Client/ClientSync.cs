using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSync : MonoBehaviour {
    public delegate void OnSyncDelegate(int step, GameState gameState);
    public event OnSyncDelegate onSync;

    [SerializeField] private Server server;
    [SerializeField] private float minFakeLag = 0;
    [SerializeField] private float maxFakeLag = 0;

    private List<ISyncEntity> syncEntities = new List<ISyncEntity>();

    public void AddSyncEntity(ISyncEntity syncEntity) {
        syncEntities.Add(syncEntity);
        var existingData = gameState?.Find(syncEntity.name);
        if (existingData != null) {
            syncEntity.ReceiveSync(step, existingData);
        }

        if (syncEntity is IAutoCreateServerEntity) {
            var createdEntity = (syncEntity as IAutoCreateServerEntity).createEntity();
            server.entityManager.AddEntity(syncEntity.name, createdEntity);
        }
    }

    public void RemoveSyncEntity(ISyncEntity syncEntity) {
        syncEntities.Remove(syncEntity);
    }

    public void SendAction(ISyncEntity from, string action) {
        StartCoroutine(sendDelayed(from.name, action));
    }

    IEnumerator sendDelayed(string fromName, string action) {
        var lag = Random.Range(minFakeLag, maxFakeLag);
        yield return new WaitForSecondsRealtime(lag);
        server.ReceiveAction(fromName, action);
    }


    public GameState gameState { get; private set; }
    public int step { get; private set; }

    void Start() {
        server.onSync += OnSync;
    }

    private void OnSync(int step, Dictionary<string, GameState.SerializedEntry> data) {
        this.step = step;
        gameState = GameState.deserialize(data);

        syncEntities.ForEach(receiver => {
            var state = gameState.Find(receiver.name);
            if (state != null) {
                receiver.ReceiveSync(step, state);
            }
        });

        if (onSync != null) {
            onSync(step, gameState);
        }
    }
}
