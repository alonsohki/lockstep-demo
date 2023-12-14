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
    private int lastStep = 0;

    void IActionable.ProcessAction(string action)
    {
        if (action == "dodge") {
            state.action = "dodge";
            goToIdleOnStep = lastStep + 2;
        }
        else if (action == "spell") {
            var currentSpell = entityManager.FindEntity("spell_towards_" + enemy);
            if (currentSpell == null) {
                var enemyEntity = entityManager.FindEntity(enemy) as PlayerEntity;
                var spell = new SpellEntity(this, enemyEntity, lastStep + 1, lastStep + 5);
                entityManager.AddEntity("spell_towards_" + enemy, spell);
            }
        }
    }

    void IStepUpdate.Update(int step) {
        if (step >= goToIdleOnStep) {
            goToIdleOnStep = int.MaxValue;
            state.action = "idle";
        }
        lastStep = step;
    }

    public void Damage(int hp) {
        state.hp -= hp;
    }
}
