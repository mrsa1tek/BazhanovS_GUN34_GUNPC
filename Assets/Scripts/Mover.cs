using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{
    [SerializeField] private Vector3 _start;
    [SerializeField] private Vector3 _end;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _delay = 1f;

    private Rigidbody _rigidbody;
    private Vector3 _initialPosition;
    private Vector3 _targetStart;
    private Vector3 _targetEnd;

    private IEnumerator Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _initialPosition = transform.position;

        // –ассчитываем мировые координаты относительно начальной позиции
        _targetStart = _initialPosition + _start;
        _targetEnd = _initialPosition + _end;

        while (true)
        {
            yield return StartCoroutine(MoveToPosition(_targetStart, _targetEnd));
            yield return new WaitForSeconds(_delay);
            yield return StartCoroutine(MoveToPosition(_targetEnd, _targetStart));
            yield return new WaitForSeconds(_delay);
        }
    }

    private IEnumerator MoveToPosition(Vector3 from, Vector3 to)
    {
        float distance = Vector3.Distance(from, to);
        float duration = distance / _speed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 newPosition = Vector3.Lerp(from, to, t);

            if (_rigidbody != null)
            {
                _rigidbody.MovePosition(newPosition);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDrawGizmos()
    {
        // ƒл€ визуализации в мировых координатах
        Vector3 currentPosition = Application.isPlaying ? _initialPosition : transform.position;
        Vector3 worldStart = currentPosition + _start;
        Vector3 worldEnd = currentPosition + _end;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(worldStart, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(worldEnd, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(worldStart, worldEnd);
    }
}