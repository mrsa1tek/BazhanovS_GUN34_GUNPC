// CollectState.cs
using UnityEngine;
using UnityEngine.AI;

public class CollectState : IState
{
    private StateMachine stateMachine;
    private CharacterAI character;
    private NavMeshAgent agent;
    private GameObject targetCollectable;
    private bool hasCollected = false;
    private bool animationStarted = false;
    private float animationTimer = 0f;
    private const float ANIMATION_DURATION = 2f; // Длительность анимации сбора

    public CollectState(StateMachine stateMachine, CharacterAI character)
    {
        this.stateMachine = stateMachine;
        this.character = character;
        this.agent = character.GetComponent<NavMeshAgent>();
    }

    public void Enter()
    {
        targetCollectable = character.GetNearestCollectable();
        hasCollected = false;
        animationStarted = false;
        animationTimer = 0f;

        if (targetCollectable != null)
        {
            character.StartMovement();
            agent.SetDestination(targetCollectable.transform.position);
        }
        else
        {
            stateMachine.ChangeState("Idle");
        }
    }

    public void Update()
    {
        if (hasCollected)
        {
            // Ждем завершения анимации
            animationTimer += Time.deltaTime;
            if (animationTimer >= ANIMATION_DURATION)
            {
                stateMachine.ChangeState("Idle");
            }
            return;
        }

        if (targetCollectable == null)
        {
            stateMachine.ChangeState("Idle");
            return;
        }

        float distanceToTarget = Vector3.Distance(character.transform.position, targetCollectable.transform.position);

        if (distanceToTarget <= 1.5f)
        {
            CollectItem();
        }
    }

    public void Exit()
    {
        character.StartMovement();
    }

    private void CollectItem()
    {
        hasCollected = true;
        character.StopMovement();
        character.PlayCollectAnimation();
        character.CollectItem(targetCollectable);

        // Не переходим сразу в Idle, ждем анимацию
        // stateMachine.ChangeState("Idle") теперь в Update после таймера
    }
}