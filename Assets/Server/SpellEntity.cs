public class SpellEntity : IEntity, IStateful, IStepUpdate
{
    private EntityManager entityManager;

    public SpellEntity(PlayerEntity fromPlayer, PlayerEntity toPlayer, int startStep, int endStep) {
        state = new SpellState {
            fromPlayer = fromPlayer,
            toPlayer = toPlayer,
            startStep = startStep,
            endStep = endStep,
        };
    }

    private SpellState state;

    IState IStateful.state => state;

    void IEntity.Setup(EntityManager manager) {
        entityManager = manager;
    }

    void IStepUpdate.Update(int step) {
        if (step > state.endStep) {
            var toPlayer = state.toPlayer;
            var toPlayerState = (toPlayer as IStateful).state as PlayerState;
            if (toPlayerState.action != "dodge") {
                toPlayer.Damage(50);
            }
            entityManager.RemoveEntity(this);
        }
    }
}
