// SearchState.cs
using UnityEngine;
using UnityEngine.AI;

public class SearchState : IState
{
    private StateMachine stateMachine;
    private CharacterAI character;
    private NavMeshAgent agent;
    private float searchTimer;

    public SearchState(StateMachine stateMachine, CharacterAI character)
    {
        this.stateMachine = stateMachine;
        this.character = character;
        this.agent = character.GetComponent<NavMeshAgent>();
    }

    public void Enter()
    {
        searchTimer = 0f;
        SetRandomDestination();
        Debug.Log("SearchState: Started searching");
    }

    public void Update()
    {
        searchTimer += Time.deltaTime;

        // Проверяем предметы в радиусе
        bool hasCollectables = character.HasCollectableInRange();
        Debug.Log($"SearchState: Has collectables in range = {hasCollectables}");

        if (hasCollectables)
        {
            Debug.Log("SearchState: Found collectable, switching to Collect");
            stateMachine.ChangeState("Collect");
            return;
        }

        // Меняем точку если достигли цели или прошло время
        if (searchTimer >= 3f || (agent.remainingDistance <= 0.5f && agent.hasPath))
        {
            SetRandomDestination();
            searchTimer = 0f;
        }
    }

    public void Exit()
    {
        Debug.Log("SearchState: Exiting search");
    }

    private void SetRandomDestination()
    {
        Vector2 randomCircle = Random.insideUnitCircle * 8f;
        Vector3 randomPosition = character.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPosition, out hit, 8f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            Debug.Log($"SearchState: New destination set to {hit.position}");
        }
    }
}