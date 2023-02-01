using System;
using _Scripts.Combat;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Units
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField] private int resourceCost = 10;
        [SerializeField] private UnityEvent onSelected = null;
        [SerializeField] private UnityEvent onDeselected = null;
        [SerializeField] private Health health;


        public static event Action<Unit> onServerUnitSpawned;
        public static event Action<Unit> onServerUnitDespawned;

        public static event Action<Unit> onAuthorityUnitSpawned;
        public static event Action<Unit> onAuthorityUnitDespawned;


        public int GetResourceCost()
        {
            return resourceCost;
        }

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleDie;
            onServerUnitSpawned?.Invoke(this);
        }


        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleDie;
            onServerUnitDespawned?.Invoke(this);
        }

        [Server]
        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                transform.localScale = new Vector3(2, 2, 2);
            }
        }

        public override void OnStartAuthority()
        {
            onAuthorityUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!isOwned) return;

            onAuthorityUnitDespawned?.Invoke(this);
        }


        [Client]
        public void Select()
        {
            if (!isOwned) return;

            onSelected?.Invoke();
        }

        [Client]
        public void Deselect()
        {
            if (!isOwned) return;

            onDeselected?.Invoke();
        }

        #endregion
    }
}