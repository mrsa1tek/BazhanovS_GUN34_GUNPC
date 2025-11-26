using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName = "Collectable";
    public int value = 1;

    void Start()
    {
        // Убедимся, что объект находится на правильном слое
        if (gameObject.layer != LayerMask.NameToLayer("Collectable"))
        {
            Debug.LogWarning($"CollectableItem {name} is not on Collectable layer! Please set it to Collectable layer.");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
    }
}