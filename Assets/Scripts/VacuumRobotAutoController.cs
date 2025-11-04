using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class VacuumRobotAutoController : MonoBehaviour
{
    [Header("Clean Settings")]
    public float cleanBoxOffset = 1f;
    public Vector3 cleanBoxSize = new Vector3(2f, 1f, 1.5f);

    [Header("Detection Settings")]
    public LayerMask trashMask;
    public float detectionRadius = 10f;
    public float searchModeRadius = 20f;

    [Header("Audio Settings")]
    public AudioClip searchModeSound;
    public AudioClip trashFoundSound;

    private AudioSource audioSource;
    private NavMeshAgent agent;
    private GameObject currentTarget;
    private bool isInSearchMode = false;
    private float searchModeTimer = 0f;
    private float timeBetweenSearchPoints = 3f;
    private bool hasFoundTrash = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        FindNearestTrash();
    }

    void Update()
    {
        // Если в режиме поиска - обновляем таймер
        if (isInSearchMode)
        {
            searchModeTimer += Time.deltaTime;

            if (!audioSource.isPlaying && searchModeSound != null)
            {
                audioSource.clip = searchModeSound;
                audioSource.loop = true;
                audioSource.Play();
            }

            // Меняем точку поиска каждые N секунд
            if (searchModeTimer >= timeBetweenSearchPoints ||
                (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance))
            {
                SetRandomSearchPoint();
                searchModeTimer = 0f;
            }

            // Периодически проверяем, не появился ли мусор nearby
            if (searchModeTimer % 3f >= Time.deltaTime) // Каждые 3 секунды
            {
                CheckForTrashNearby();
            }
        }
        else
        {
            if (audioSource.isPlaying && audioSource.clip == searchModeSound)
            {
                audioSource.Stop();
            }
            // Режим охоты за мусором
            if (currentTarget == null ||
                (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance))
            {
                FindNearestTrash();
            }
        }

        CleanTrash();
    }

    void FindNearestTrash()
    {
        // Ищем мусор в радиусе обнаружения
        Collider[] trashInRadius = Physics.OverlapSphere(transform.position, detectionRadius, trashMask);

        if (trashInRadius.Length == 0)
        {
            Debug.Log("Мусора не найдено. Включаю режим поиска");
            SwitchToSearchMode();
            return;
        }

        // Находим ближайший мусор
        currentTarget = trashInRadius
            .OrderBy(trash => Vector3.Distance(transform.position, trash.transform.position))
            .Select(trash => trash.gameObject)
            .FirstOrDefault();

        if (currentTarget != null)
        {
            isInSearchMode = false;
            hasFoundTrash = true;
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            agent.SetDestination(currentTarget.transform.position);
            Debug.Log($"Цель: {currentTarget.name}, расстояние: {distance:F2}m");
            PlaySound(trashFoundSound, false);
            Invoke("StartCleaningSound", trashFoundSound.length);
        }
    }

    void SwitchToSearchMode()
    {
        isInSearchMode = true;
        currentTarget = null;
        hasFoundTrash = false;
        searchModeTimer = 0f;
        SetRandomSearchPoint();
        Debug.Log("Включен режим поиска");
    }

    void SetRandomSearchPoint()
    {
        // Генерируем случайную точку в большем радиусе
        Vector3 randomDirection = Random.insideUnitSphere * searchModeRadius;
        randomDirection.y = 0; // Оставляем только горизонтальное направление

        Vector3 randomPoint = transform.position + randomDirection;

        // Ищем валидную позицию на NavMesh
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, searchModeRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            Debug.Log($"Поиск: еду в случайную точку ({hit.position.x:F1}, {hit.position.z:F1})");
        }
        else
        {
            // Если не нашли валидную точку, пробуем еще раз в следующем кадре
            Debug.Log("Не удалось найти валидную точку для поиска");
        }
    }

    void CheckForTrashNearby()
    {
        // Быстрая проверка на мусор в радиусе
        Collider[] trashInRadius = Physics.OverlapSphere(transform.position, detectionRadius, trashMask);
        if (trashInRadius.Length > 0 && !hasFoundTrash)
        {
            Debug.Log("Обнаружен мусор! Возвращаюсь в режим охоты");
            isInSearchMode = false;
            FindNearestTrash();
        }
    }

    void CleanTrash()
    {
        Vector3 boxPosition = transform.position + transform.forward * cleanBoxOffset;
        Collider[] hitColliders = Physics.OverlapBox(boxPosition, cleanBoxSize * 0.5f, transform.rotation, trashMask);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject == currentTarget)
            {
                currentTarget = null;
            }
            Destroy(collider.gameObject);

            int trashLeft = GameObject.FindGameObjectsWithTag("Trash").Length;
            Debug.Log($"Мусор убран! Осталось: {trashLeft}");


            // Если убрали мусор и были в режиме поиска - переключаемся обратно
            if (isInSearchMode)
            {
                isInSearchMode = false;
                FindNearestTrash();
            }

            if (GameObject.FindGameObjectsWithTag("Trash").Length == 0)
            {
                audioSource.Stop();
                hasFoundTrash = false;
                SwitchToSearchMode();
            }
        }
    }

    private void PlaySound(AudioClip clip, bool loop)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.Play();
        }
    }

    void OnDrawGizmos()
    {
        // Радиус обнаружения мусора (желтый)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Радиус поискового режима (синий) - только в поисковом режиме
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, searchModeRadius);

        // Зона уборки (зеленый)
        Gizmos.color = Color.green;
        Vector3 boxPosition = transform.position + transform.forward * cleanBoxOffset;
        Gizmos.matrix = Matrix4x4.TRS(boxPosition, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, cleanBoxSize);
        Gizmos.matrix = Matrix4x4.identity;

        // Линия к текущей цели
        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);
            Gizmos.DrawSphere(currentTarget.transform.position, 0.3f);
        }

        // Индикатор режима над роботом
        Vector3 labelPos = transform.position + Vector3.up * 2f;
#if UNITY_EDITOR
        string modeText = isInSearchMode ? "ПОИСК" : "ОХОТА";
        UnityEditor.Handles.Label(labelPos, modeText);
#endif
    }
}