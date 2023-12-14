using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class GameState {
    private static class Factory {
        private delegate IState Builder();
        private static Dictionary<string, Builder> builders = new Dictionary<string, Builder>() {
            { "PlayerState", () => { return new PlayerState(); } },
            { "SpellState", () => { return new SpellState(); } },
        };

        public static IState build(string type) {
            Builder builder;
            if (builders.TryGetValue(type, out builder)) {
                return builder();
            }
            return null;
        }
    }

    public struct SerializedEntry {
        public string type;
        public Dictionary<string, object> data;
    }

    public struct SerializableEntity {
        public string name;
        public IState state;
    }

    public static Dictionary<string, SerializedEntry> serialize(IEnumerable<SerializableEntity> entities) {
        var data =
            from entity in entities
            let dict = new Dictionary<string, object>()
            where entity.state.WriteState(dict)
            select new KeyValuePair<string, SerializedEntry>(entity.name, new SerializedEntry {
                type = entity.state.Type,
                data = dict,
            });
        return data.ToDictionary(x => x.Key, x => x.Value);
    }

    private Dictionary<string, IState> entities;

    private GameState(Dictionary<string, IState> entities) {
        this.entities = entities;
    }

    public IState Find(string name) {
        IState state;
        if (entities.TryGetValue(name, out state)) {
            return state;
        }
        return null;
    }

    public IReadOnlyDictionary<string, IState> GetAll() {
        return entities;
    }

    public static GameState deserialize(Dictionary<string, SerializedEntry> data) {
        var gameState =
            from pair in data
            let name = pair.Key
            let entry = pair.Value
            let state = Factory.build(entry.type)
            where state.ReadState(entry.data)
            select new KeyValuePair<string, IState>(name, state)
        ;
        return new GameState(gameState.ToDictionary(x => x.Key, x => x.Value));
    }
}
