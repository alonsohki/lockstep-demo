using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellViewManager : MonoBehaviour
{
    [SerializeField] private ClientSync clientSync;
    [SerializeField] private SpellView spellPrefab;

    private Dictionary<string, SpellView> views = new Dictionary<string, SpellView>();

    void Start()
    {
        clientSync.onSync += OnSync;
    }

    private void OnSync(int step, GameState gameState) {
        var entities = gameState.GetAll();
        var spells = entities.Keys.Where(x => x.StartsWith("spell_towards_")).ToList();
        var deletedSpells = views.Keys.Except(spells).ToList();
        var newSpells = spells.Except(views.Keys).ToList();
        
        deletedSpells.ForEach(deletedSpell => {
            SpellView view;
            if (views.TryGetValue(deletedSpell, out view)) {
                Destroy(view.gameObject);
                views.Remove(deletedSpell);
                clientSync.RemoveSyncEntity(view);
            }
        });

        newSpells.ForEach(newSpell => {
            var view = Instantiate(spellPrefab);

            view.transform.SetParent(transform, false);
            view.spellName = newSpell;
            views.Add(newSpell, view);

            clientSync.AddSyncEntity(view);
        });
    }
}
