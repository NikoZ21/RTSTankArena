using _Scripts.Buildings;
using _Scripts.Combat;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Units
{
    public class UnitMovement : NetworkBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Targeter _targeter;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _targeter = GetComponent<Targeter>();
        }


        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        private void ServerHandleGameOver()
        {
            _navMeshAgent.ResetPath();
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [ServerCallback]
        private void Update()
        {
            Targetable target = _targeter.Target;

            if (target != null)
            {
                if (!_targeter.IsInRange())
                {
                    _navMeshAgent.SetDestination(target.transform.position);
                }
                else if (_navMeshAgent.hasPath)
                {
                    _navMeshAgent.ResetPath();
                }

                return;
            }

            if (!_navMeshAgent.hasPath) return;

            if (_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance) return;

            _navMeshAgent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 newPosition)
        {
            ServerMove(newPosition);
        }

        [Server]
        public void ServerMove(Vector3 newPosition)
        {
            _targeter.ClearTarget();

            if (!NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 0.5f, NavMesh.AllAreas)) return;

            _navMeshAgent.SetDestination(newPosition);
        }
    }
}