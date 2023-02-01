using System;
using _Scripts.Combat;
using Mirror;
using UnityEngine;

namespace _Scripts.Buildings
{
    public class UnitBase : NetworkBehaviour
    {
        [SerializeField] private Health _health;

        public static event Action<int> ServerOnPlayerDie;
        public static event Action<UnitBase> serverOnBaseSpawned;
        public static event Action<UnitBase> serverOnBaseDespawned;

        #region Server

        public override void OnStartServer()
        {
            _health.ServerOnDie += HandleOnServerDeath;

            serverOnBaseSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            _health.ServerOnDie -= HandleOnServerDeath;

            serverOnBaseDespawned?.Invoke(this);
        }

        [Server]
        private void HandleOnServerDeath()
        {
            ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client

        #endregion
    }
}