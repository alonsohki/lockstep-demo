using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellViewManager : MonoBehaviour
{
    [SerializeField] private Server server;
    [SerializeField] private SpellView spellPrefab;

    private Dictionary<string, SpellView> views = new Dictionary<string, SpellView>();

    void Start()
    {
        server.onSync += OnSync;
    }

    private void OnSync(int step, Dictionary<string, Dictionary<string, object>> gameState) {
        var spells = gameState.Keys.Where(x => x.StartsWith("spell_towards_")).ToList();
        var deletedSpells = views.Keys.Except(spells).ToList();
        var newSpells = spells.Except(views.Keys).ToList();
        
        deletedSpells.ForEach(deletedSpell => {
            SpellView view;
            if (views.TryGetValue(deletedSpell, out view)) {
                Destroy(view.gameObject);
                views.Remove(deletedSpell);
                server.RemoveSyncEntity(view);
            }
        });

        newSpells.ForEach(newSpell => {
            var view = Instantiate(spellPrefab);
            var entity = server.entityManager.FindEntity(newSpell) as SpellEntity;

            view.transform.SetParent(transform, false);
            view.spellName = newSpell;
            view.SetEntity(entity, step);
            views.Add(newSpell, view);

            server.AddSyncEntity(view, entity);
        });
    }
}
