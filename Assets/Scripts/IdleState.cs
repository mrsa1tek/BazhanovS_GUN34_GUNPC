// IdleState.cs
using UnityEngine;

public class IdleState : IState
{
    private StateMachine stateMachine;
    private CharacterAI character;
    private float idleTimer;

    public IdleState(StateMachine stateMachine, CharacterAI character)
    {
        this.stateMachine = stateMachine;
        this.character = character;
    }

    public void Enter()
    {
        idleTimer = 0f;
    }

    public void Update()
    {
        idleTimer += Time.deltaTime;

        // Сразу проверяем предметы
        if (character.HasCollectableInRange())
        {
            stateMachine.ChangeState("Collect");
            return;
        }

        // Через 1 секунду идем искать
        if (idleTimer >= 1f)
        {
            stateMachine.ChangeState("Search");
        }
    }

    public void Exit()
    {
    }
}