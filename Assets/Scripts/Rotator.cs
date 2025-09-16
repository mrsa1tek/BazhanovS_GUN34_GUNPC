using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 _rotate = new Vector3(0, 30, 0);

    private Rigidbody _rigidbody;

    private IEnumerator Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        while (true)
        {
            if (_rigidbody != null )
            {
                Quaternion deltaRotation = Quaternion.Euler(_rotate * Time.fixedDeltaTime);
                _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
