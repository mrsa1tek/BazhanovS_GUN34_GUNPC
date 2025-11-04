using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumRobotController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float raycastDistance = 2f; // дальность
    public float timeBetweenTurns = 3f;
    public float cleanRadius = 1.5f;

    public Vector3 cleanBoxSize = new Vector3(2f, 1f, 1.5f);
    public float cleanBoxOffset = 1f;

    [SerializeField] private LayerMask _trashMask;
    private float turnTimer;

  

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);

        bool hitForward = Physics.Raycast(transform.position, transform.forward, raycastDistance);
        bool hitLeft = Physics.Raycast(transform.position, -transform.right, raycastDistance);
        bool hitRight = Physics.Raycast(transform.position, transform.right, raycastDistance);

        turnTimer += Time.deltaTime;

        if (hitForward || turnTimer >= timeBetweenTurns)
        {
            if (hitForward) // Поворот при препятствии
            {
                if (!hitRight) transform.Rotate(0, 90, 0);
                else if (!hitLeft) transform.Rotate(0, -90, 0);
                else transform.Rotate(0, 180, 0);
            }
            else // Случайный поворот
            {
                int rndChoiceDir = UnityEngine.Random.Range(0, 3);
                if (rndChoiceDir == 0) transform.Rotate(0, 90, 0);
                else if (rndChoiceDir == 1) transform.Rotate(0, -90, 0);
                else transform.Rotate(0, 180, 0);
            }

            turnTimer = 0f;
        }

        CleanTrash();
    
    }

    void CleanTrash()
    {
        Vector3 boxPosition = transform.position + transform.forward * cleanBoxOffset;

        Collider[] hitColliders = Physics.OverlapBox(boxPosition, cleanBoxSize * 0.5f, transform.rotation, _trashMask);
        
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Trash"))
            {
                Destroy(collider.gameObject);
                Debug.Log("Мусор убран! Осталось: " + GameObject.FindGameObjectsWithTag("Trash").Length);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * raycastDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, -transform.right * raycastDistance);
        Gizmos.DrawRay(transform.position, transform.right * raycastDistance);

        Gizmos.color = Color.green;
        Vector3 boxPosition = transform.position + transform.forward * cleanBoxOffset;
        Gizmos.matrix = Matrix4x4.TRS(boxPosition, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, cleanBoxSize);
    }
}
