using System.Collections.Generic;

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

    public SpellState state { get; private set; }

    void IEntity.Setup(EntityManager manager) {
        entityManager = manager;
    }

    void IStateful.WriteState(Dictionary<string, object> state) {
        state.Add("fromPlayer", this.state.fromPlayer);
        state.Add("toPlayer", this.state.toPlayer);
        state.Add("startStep", this.state.startStep);
        state.Add("endStep", this.state.endStep);
    }

    void IStateful.ReadState(Dictionary<string, object> state) {
        this.state = new SpellState {
            fromPlayer = (PlayerEntity)state["fromPlayer"],
            toPlayer = (PlayerEntity)state["toPlayer"],
            startStep = (int)state["startStep"],
            endStep = (int)state["endStep"],
        };
    }

    void IStepUpdate.Update(int step) {
        if (step == state.endStep && state.toPlayer.state.action != "dodge") {
            state.toPlayer.Damage(50);
        }

        if (step > state.endStep) {
            entityManager.RemoveEntity(this);
        }
    }
}
