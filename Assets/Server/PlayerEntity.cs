public class PlayerEntity : IEntity, IStateful, IActionable, IStepUpdate
{
    private EntityManager entityManager;
    private string enemy;

    public int position { get; private set; }

    public PlayerEntity(string enemy, int position) {
        this.enemy = enemy;
        this.position = position;
    }

    private PlayerState state = new PlayerState(100, "idle");
    IState IStateful.state => state;

    void IEntity.Setup(EntityManager manager) {
        entityManager = manager;
    }

    private int goToIdleOnStep = int.MaxValue;

    void IActionable.ProcessAction(int step, string action)
    {
        if (action == "dodge") {
            state.action = "dodge";
            goToIdleOnStep = step + 2;
        }
        else if (action == "spell") {
            var currentSpell = entityManager.FindEntity("spell_towards_" + enemy);
            if (currentSpell == null) {
                var enemyEntity = entityManager.FindEntity(enemy) as PlayerEntity;
                var spell = new SpellEntity(this, enemyEntity, step, step + 5);
                entityManager.AddEntity("spell_towards_" + enemy, spell);
            }
        }
    }

    void IStepUpdate.Update(int step) {
        if (step >= goToIdleOnStep) {
            goToIdleOnStep = int.MaxValue;
            state.action = "idle";
        }
    }

    public void Damage(int hp) {
        state.hp -= hp;
    }
}
