using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace _Scripts.Buildings
{
    public class GameOverHandler : NetworkBehaviour
    {
        [SerializeField] private List<UnitBase> bases = new List<UnitBase>();

        public static event Action ServerOnGameOver;

        public static event Action<string> ClientOnGameOver;

        #region Server

        public override void OnStartServer()
        {
            UnitBase.serverOnBaseSpawned += ServerHandleBaseSpawned;
            UnitBase.serverOnBaseDespawned += ServerHandleBaseDesapwned;
        }

        public override void OnStopServer()
        {
            UnitBase.serverOnBaseSpawned -= ServerHandleBaseSpawned;
            UnitBase.serverOnBaseDespawned -= ServerHandleBaseDesapwned;
        }

        [Server]
        private void ServerHandleBaseSpawned(UnitBase unitBase)
        {
            bases.Add(unitBase);
        }

        [Server]
        private void ServerHandleBaseDesapwned(UnitBase unitBase)
        {
            bases.Remove(unitBase);

            if (bases.Count != 1) return;

            print("Game Is Over");

            int playerId = bases[0].connectionToClient.connectionId;

            RpcGameOver($"Player {playerId.ToString()}");

            ServerOnGameOver?.Invoke();
        }

        #endregion

        #region Client

        [ClientRpc]
        private void RpcGameOver(string winner)
        {
            ClientOnGameOver?.Invoke(winner);
        }

        #endregion
    }
}