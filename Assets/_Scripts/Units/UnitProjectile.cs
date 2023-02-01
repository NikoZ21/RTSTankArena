using _Scripts.Combat;
using Mirror;
using UnityEngine;

namespace _Scripts.Units
{
    public class UnitProjectile : NetworkBehaviour
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _launchForce = 20f;
        [SerializeField] private float _lifeTime = 5f;
        [SerializeField] private int _damageToDeal = 20;

        private void Start()
        {
            _rb.velocity = transform.forward * _launchForce;
        }

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), _lifeTime);
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.connectionToClient == connectionToClient) return;
            }

            if (other.TryGetComponent(out Health health))
            {
                health.DealDamage(_damageToDeal);
            }

            DestroySelf();
        }

        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}