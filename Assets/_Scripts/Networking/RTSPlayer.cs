using System;
using System.Collections.Generic;
using _Scripts.Buildings;
using _Scripts.Units;
using Mirror;
using UnityEngine;

namespace _Scripts.Networking
{
    public class RTSPlayer : NetworkBehaviour
    {
        [SerializeField] private Transform cameraTransfrom;
        [SerializeField] private LayerMask buildingBlockLayer;
        [SerializeField] private Building[] buildings;
        [SerializeField] private float buildingRangelimit = 5;

        private Color teamColor = new Color();
        private List<Unit> _myUnits = new List<Unit>();
        private List<Building> _myBuildings = new List<Building>();

        [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
        private int _resources = 500;

        public event Action<int> ClientOnResourcesUpdated;

        public Transform GetCameraTransform()
        {
            return cameraTransfrom;
        }

        public Color GetTeamColor()
        {
            return teamColor;
        }

        public int GetResources()
        {
            return _resources;
        }

        public List<Unit> GetMyUnits()
        {
            return _myUnits;
        }

        public List<Building> GetBuildings()
        {
            return _myBuildings;
        }

        public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
        {
            if (Physics.CheckBox(point + buildingCollider.center,
                    buildingCollider.size / 2,
                    Quaternion.identity,
                    buildingBlockLayer))
            {
                print("I am returning false because we are colliding with something else");
                return false;
            }

            foreach (var building in _myBuildings)
            {
                if (Vector3.Distance(point, building.transform.position) <= buildingRangelimit)
                {
                    print("Distance between towers is: " +
                          Vector3.Distance(point, building.transform.position).ToString());
                    return true;
                }
            }

            print("we are just not letting you build");
            return false;
        }

        #region Server

        public override void OnStartServer()
        {
            Unit.onServerUnitSpawned += ServerHandleUnitSpawned;
            Unit.onServerUnitDespawned += ServerHandleUnitDespawned;

            Building.onServerBuilidingSpawned += ServerHandleBuildingSpawned;
            Building.onServerBuildingDespawned += ServerHandleBuildingDespawned;
        }

        public override void OnStopServer()
        {
            Unit.onServerUnitSpawned -= ServerHandleUnitSpawned;
            Unit.onServerUnitDespawned -= ServerHandleUnitDespawned;

            Building.onServerBuilidingSpawned -= ServerHandleBuildingSpawned;
            Building.onServerBuildingDespawned -= ServerHandleBuildingDespawned;
        }

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

            _myUnits.Add(unit);
        }

        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

            _myUnits.Remove(unit);
        }

        private void ServerHandleBuildingSpawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;

            _myBuildings.Add(building);
        }

        private void ServerHandleBuildingDespawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;

            _myBuildings.Remove(building);
        }

        [Command]
        public void CmdPlaceBuilding(int buildingId, Vector3 point)
        {
            Building buildingToPlace = null;

            foreach (var building in buildings)
            {
                if (building.GetId() == buildingId)
                {
                    buildingToPlace = building;
                    break;
                }
            }

            if (!buildingToPlace) return;

            if (_resources < buildingToPlace.GetPrice()) return;

            BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

            if (!CanPlaceBuilding(buildingCollider, point)) return;

            SetResources(-buildingToPlace.GetPrice());

            var buildingInstance = Instantiate(buildingToPlace.gameObject, point, Quaternion.identity);
            NetworkServer.Spawn(buildingInstance, connectionToClient);
        }

        [Server]
        public void SetTeamColor(Color color)
        {
            teamColor = color;
        }

        [Server]
        public void SetResources(int resources)
        {
            _resources += resources;
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) return;

            Unit.onAuthorityUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.onAuthorityUnitDespawned += AuthorityHandleUnitDespawned;

            Building.onAuthorityBuildingSpawned += AuthorityHandleBuildingSpawned;
            Building.onAuthorityBuildingDespawned += AuthorityHandleBuildingDespawned;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !isOwned) return;

            Unit.onAuthorityUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.onAuthorityUnitDespawned -= AuthorityHandleUnitDespawned;


            Building.onAuthorityBuildingSpawned -= AuthorityHandleBuildingSpawned;
            Building.onAuthorityBuildingDespawned -= AuthorityHandleBuildingDespawned;
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            _myUnits.Remove(unit);
        }

        private void AuthorityHandleUnitSpawned(Unit unit)
        {
            _myUnits.Add(unit);
        }


        private void AuthorityHandleBuildingDespawned(Building building)
        {
            _myBuildings.Remove(building);
        }

        private void AuthorityHandleBuildingSpawned(Building building)
        {
            _myBuildings.Add(building);
        }

        private void ClientHandleResourcesUpdated(int oldResources, int newResources)
        {
            ClientOnResourcesUpdated?.Invoke(newResources);
        }

        #endregion
    }
}