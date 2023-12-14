using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour, ISyncEntity, IAutoCreateServerEntity
{
    [SerializeField] private ClientSync clientSync;
    [SerializeField] private string entityName;
    [SerializeField] private string enemyName;
    [SerializeField] private int position;
    [SerializeField] private TextMeshProUGUI hp;
    [SerializeField] private TextMeshProUGUI action;

    [SerializeField] private Button dodge;
    [SerializeField] private Button spell;

    [SerializeField] private RectTransform box;

    string ISyncEntity.name => entityName;

    void ISyncEntity.ReceiveSync(int step, IState state_)
    {
        var state = state_ as PlayerState;

        hp.text = state.hp.ToString();
        action.text = state.action;

        if (state.action == "dodge") {
            box.anchoredPosition = new Vector2(0, 200);
        }
        else {
            box.anchoredPosition = Vector2.zero;
        }
    }

    void Start() {
        dodge.onClick.AddListener(OnDodge);
        spell.onClick.AddListener(OnSpell);
        clientSync.AddSyncEntity(this);
    }

    private void OnDodge() {
        clientSync.SendAction(this, "dodge");
    }

    private void OnSpell() {
        clientSync.SendAction(this, "spell");
    }

    IEntity IAutoCreateServerEntity.createEntity()
    {
        return new PlayerEntity(enemyName, position);
    }
}
