using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour, ISyncEntity
{
    [SerializeField] private Server server;
    [SerializeField] private string entityName;
    [SerializeField] private string enemyName;
    [SerializeField] private int position;
    [SerializeField] private TextMeshProUGUI hp;
    [SerializeField] private TextMeshProUGUI action;

    [SerializeField] private Button dodge;
    [SerializeField] private Button spell;

    [SerializeField] private RectTransform box;

    [SerializeField] private float minFakeLag = 0;
    [SerializeField] private float maxFakeLag = 0;

    private ISyncEntity.SendSyncDelegate sendSync;


    string ISyncEntity.name => entityName;

    void ISyncEntity.SetupSync(ISyncEntity.SendSyncDelegate sendSync)
    {
        this.sendSync = sendSync;
    }

    void ISyncEntity.ReceiveSync(int step, Dictionary<string, object> state)
    {
        var action = state["action"].ToString();
        var hp = state["hp"].ToString();

        this.hp.text = hp;
        this.action.text = action;

        if (action == "dodge") {
            box.anchoredPosition = new Vector2(0, 200);
        }
        else {
            box.anchoredPosition = Vector2.zero;
        }
    }

    void Start() {
        dodge.onClick.AddListener(OnDodge);
        spell.onClick.AddListener(OnSpell);

        var playerEntity = new PlayerEntity(enemyName, position);
        server.entityManager.AddEntity(entityName, playerEntity);
        server.AddSyncEntity(this, playerEntity);
    }

    private void OnDodge() {
        StartCoroutine(sendDelayed("dodge"));
    }

    private void OnSpell() {
        StartCoroutine(sendDelayed("spell"));
    }

    IEnumerator sendDelayed(string action) {
        var lag = Random.Range(minFakeLag, maxFakeLag);
        yield return new WaitForSecondsRealtime(lag);
        sendSync(action);
    }
}
