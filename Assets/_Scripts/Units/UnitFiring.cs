using _Scripts.Combat;
using Mirror;
using UnityEngine;

namespace _Scripts.Units
{
    public class UnitFiring : NetworkBehaviour
    {
        [SerializeField] private Targeter _targeter = null;
        [SerializeField] private GameObject _projectilePrefab = null;
        [SerializeField] private Transform _projectileSpawnPoint = null;
        [SerializeField] private float _fireRate = 1;
        [SerializeField] private float _rotationSpeed = 20f;
        private float lastFireTime;


        [ServerCallback]
        void Update()
        {
            if (!CanFireAtTarget()) return;

            Quaternion targetRotation
                = Quaternion.LookRotation(_targeter.Target.transform.position - transform.position);

            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            if (Time.time > (1 / _fireRate) + lastFireTime)
            {
                Quaternion projectileRotation =
                    Quaternion.LookRotation(_targeter.Target.AimAtPoint.position - transform.position);

                GameObject projectileInstance =
                    Instantiate(_projectilePrefab, _projectileSpawnPoint.position, projectileRotation);

                NetworkServer.Spawn(projectileInstance, connectionToClient);

                lastFireTime = Time.time;
            }
        }

        [Server]
        private bool CanFireAtTarget()
        {
            return _targeter.IsInRange();
        }
    }
}