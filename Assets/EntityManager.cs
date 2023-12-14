using System.Collections.Generic;

public class EntityManager {
    private class Entity {
        public string name;
        public IEntity data;
    }

    private List<Entity> entities = new List<Entity>();
    private List<IActionable> actionables = new List<IActionable>();
    private List<IStepUpdate> stepUpdates = new List<IStepUpdate>();
    private List<Entity> stateful = new List<Entity>();
    
    public void AddEntity(string name, IEntity data) {
        var entity = new Entity {
            name = name,
            data = data,
        };

        entities.Add(entity);

        if (data is IActionable) {
            actionables.Add(data as IActionable);
        }

        if (data is IStepUpdate) {
            stepUpdates.Add(data as IStepUpdate);
        }

        if (data is IStateful) {
            stateful.Add(entity);
        }

        data.Setup(this);
    }

    public void RemoveEntity(IEntity data) {
        entities.RemoveAll(entity => entity.data == data);

        if (data is IActionable) {
            actionables.Remove(data as IActionable);
        }

        if (data is IStepUpdate) {
            stepUpdates.Remove(data as IStepUpdate);
        }

        if (data is IStateful) {
            stateful.RemoveAll(entity => entity.data == data);
        }
    }

    public IEntity FindEntity(string name) {
        return entities.Find(delegate (Entity entity) { return entity.name == name; })?.data;
    }

    public void Update(Dictionary<IActionable, string> pendingActions, int step) {
        foreach (var pair in pendingActions) {
            pair.Key.ProcessAction(pair.Value);
        }

        var stepUpdatesCopy = new List<IStepUpdate>(stepUpdates);
        stepUpdatesCopy.ForEach(delegate (IStepUpdate stepUpdate) {
            stepUpdate.Update(step);
        });
    }

    public Dictionary<string, Dictionary<string, object>> GetGameState() {
        Dictionary<string, Dictionary<string, object>> gameState = new Dictionary<string, Dictionary<string, object>>();
        stateful.ForEach(delegate(Entity entity) {
            var state = new Dictionary<string, object>();
            (entity.data as IStateful).WriteState(state);
            gameState.Add(entity.name, state);
        });
        return gameState;
    }
}
