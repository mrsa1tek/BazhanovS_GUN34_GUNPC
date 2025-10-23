using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float _radiusOfVisibility;
    [SerializeField, Range(0, 360)] private float _angle;

    [SerializeField] private Player _player;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstractMask;

    private bool _canSeePlayer;

    public bool CanSeePlayer => _canSeePlayer;
    public float Radius => _radiusOfVisibility;
    public float Angle => _angle;
    public Player Player => _player;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FovRountine());
    }

    // Update is called once per frame
    void Update()
    {

    }
    private IEnumerator FovRountine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        
        while(_player != null)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, _radiusOfVisibility,_targetMask);
        if(rangeChecks.Length>0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < _angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget,distanceToTarget, _obstractMask))
                {
                    _canSeePlayer = true;
                }
                else
                {
                    _canSeePlayer = false;
                }
            }
            else
            {
                _canSeePlayer = false;
            }
        }
        else if(_canSeePlayer)
        {
            _canSeePlayer = false;
        }
    }
}
