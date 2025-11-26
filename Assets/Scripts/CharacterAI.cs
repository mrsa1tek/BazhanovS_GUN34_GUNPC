// CharacterAI.cs
using UnityEngine;
using UnityEngine.AI;

public class CharacterAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float detectionRadius = 5f;
    public LayerMask collectableLayer = 1;

    [Header("Debug")]
    [SerializeField] private string currentStateName;

    private StateMachine stateMachine;
    private NavMeshAgent _agent;
    private Animator _animator;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _agent.speed = 3.5f;
        _agent.stoppingDistance = 0.5f;

        stateMachine = gameObject.AddComponent<StateMachine>();
        stateMachine.AddState("Idle", new IdleState(stateMachine, this));
        stateMachine.AddState("Search", new SearchState(stateMachine, this));
        stateMachine.AddState("Collect", new CollectState(stateMachine, this));
        stateMachine.ChangeState("Idle");
    }

    void Update()
    {
        if (stateMachine != null)
        {
            currentStateName = stateMachine.GetCurrentStateName();
        }

        // Всегда обновляем анимацию скорости
        _animator.SetFloat("Speed", _agent.velocity.magnitude);
    }

    public bool HasCollectableInRange()
    {
        Collider[] collectables = Physics.OverlapSphere(transform.position, detectionRadius, collectableLayer);
        return collectables.Length > 0;
    }

    public GameObject GetNearestCollectable()
    {
        Collider[] collectables = Physics.OverlapSphere(transform.position, detectionRadius, collectableLayer);
        if (collectables.Length == 0) return null;

        GameObject nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider collider in collectables)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = collider.gameObject;
            }
        }
        return nearest;
    }

    public void StopMovement()
    {
        _agent.isStopped = true;
    }

    public void StartMovement()
    {
        _agent.isStopped = false;
    }

    public void PlayCollectAnimation()
    {
        _animator.SetTrigger("Collect"); // Используем Trigger вместо Float
    }

    public void CollectItem(GameObject item)
    {
        Destroy(item);
    }
}