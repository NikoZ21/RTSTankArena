using _Scripts.Buildings;
using Mirror;
using UnityEngine;

namespace _Scripts.Combat
{
    public class Targeter : NetworkBehaviour
    {
        [SerializeField] private float range;
        private Targetable target;

        public Targetable Target
        {
            get => target;
        }

        #region Server

        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [Server]
        private void ServerHandleGameOver()
        {
            ClearTarget();
        }

        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            if (!targetGameObject.TryGetComponent(out Targetable newTarget)) return;

            if (newTarget.connectionToClient == connectionToClient) return;

            target = newTarget;
        }

        [Server]
        public void ClearTarget()
        {
            target = null;
        }

        [Server]
        public bool IsInRange()
        {
            if (!target) return false;

            return Mathf.Abs(Vector3.Distance(transform.position, target.transform.position)) <= range;
        }

        #endregion
    }
}