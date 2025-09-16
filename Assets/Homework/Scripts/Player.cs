using System.Collections;
using UnityEngine;

namespace Netologia.Homework
{
    public class Player : MonoBehaviour
    {
        private bool _ready;
        private Rigidbody _ball;
        
        [SerializeField]
        private Rigidbody _ballPrefab;
        [SerializeField]
        private float _startVelocity = 15f;
        [SerializeField]
        private float _lifetime = 5f;
        [SerializeField]
        private float _respawnDelay = 1f;

        [SerializeField]
        private Transform _spawnPoint; // Добавьте эту ссылку!

        private void Update()
        {
            if (!_ready) return;
            if (Input.GetKeyDown(KeyCode.Space)) // Изменили на GetKeyDown
            {
                StartCoroutine(Reloader());
                _ball.isKinematic = false;
                _ball.transform.parent = null;
                _ball.velocity = transform.forward * _startVelocity;
                Destroy(_ball.gameObject, _lifetime);
            }
        }

        private IEnumerator Reloader()
        {
            _ready = false;
            yield return new WaitForSeconds(_respawnDelay);
            Spawn();
        }

        private void Spawn()
        {
            Vector3 spawnPosition = _spawnPoint != null ? _spawnPoint.position : transform.position;
            Quaternion spawnRotation = _spawnPoint != null ? _spawnPoint.rotation : transform.rotation;
            
            _ball = Instantiate(_ballPrefab, spawnPosition, spawnRotation);
            _ball.transform.parent = transform;
            _ball.isKinematic = true;
            _ready = true;
        }

        private void Start()
        {
            Spawn();
        }
    }
}