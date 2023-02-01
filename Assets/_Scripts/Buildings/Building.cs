using System;
using Mirror;
using UnityEngine;

namespace _Scripts.Buildings
{
    public class Building : NetworkBehaviour
    {
        [SerializeField] private GameObject buildingPreview;
        [SerializeField] Sprite icon = null;
        [SerializeField] int price = 100;
        [SerializeField] int Id = -1;

        public static event Action<Building> onServerBuilidingSpawned;
        public static event Action<Building> onServerBuildingDespawned;

        public static event Action<Building> onAuthorityBuildingSpawned;
        public static event Action<Building> onAuthorityBuildingDespawned;


        public GameObject GetBuildingReview()
        {
            return buildingPreview;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public int GetId()
        {
            return Id;
        }

        public int GetPrice()
        {
            return price;
        }

        #region Server

        public override void OnStartServer()
        {
            onServerBuilidingSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            onServerBuildingDespawned?.Invoke(this);
        }

        #endregion


        #region Client

        public override void OnStartAuthority()
        {
            onAuthorityBuildingSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!isOwned) return;

            onAuthorityBuildingDespawned?.Invoke(this);
        }

        #endregion
    }
}