using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        return entities.Find(entity => entity.name == name)?.data;
    }

    public void Update(Dictionary<IActionable, string> pendingActions, int step) {
        foreach (var pair in pendingActions) {
            pair.Key.ProcessAction(pair.Value);
        }

        var stepUpdatesCopy = new List<IStepUpdate>(stepUpdates);
        stepUpdatesCopy.ForEach(stepUpdate => stepUpdate.Update(step));
    }

    public Dictionary<string, GameState.SerializedEntry> GetGameState() {
        var serializableEntities =
            from entity in stateful
            let statefulEntity = entity.data as IStateful
            where statefulEntity != null
            select new GameState.SerializableEntity {
                name = entity.name,
                state = statefulEntity.state,
            }
        ;
        return GameState.serialize(serializableEntities);
    }
}
