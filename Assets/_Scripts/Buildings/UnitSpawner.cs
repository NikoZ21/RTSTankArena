using System;
using _Scripts.Combat;
using _Scripts.Networking;
using _Scripts.Units;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Unit = _Scripts.Units.Unit;

namespace _Scripts.Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private Unit unitPrefab = null;
        [SerializeField] private Transform unitSpawnPoint;
        [SerializeField] private Health health;
        [SerializeField] private TMP_Text remainingUnitsText = null;
        [SerializeField] private Image unitProgressImage = null;
        [SerializeField] private int maxUnitQueue = 5;
        [SerializeField] private float spawnMoveRange = 7;
        [SerializeField] private float unitSpawnDuration = 5;

        [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
        private int _queuedUnits;

        [SyncVar]
        private float _unitTimer;

        private float _progressImageVelocity;

        private void Start()
        {
            remainingUnitsText.text = _queuedUnits.ToString();
        }

        private void Update()
        {
            if (isServer)
            {
                ProduceUnits();
            }

            if (isClient)
            {
                UpdateTimerDisplay();
            }
        }

        #region Server

        [Server]
        private void ProduceUnits()
        {
            if (_queuedUnits == 0) return;

            _unitTimer += Time.deltaTime;

            if (_unitTimer < unitSpawnDuration) return;

            GameObject unitInstance =
                Instantiate(unitPrefab.gameObject, unitSpawnPoint.position, unitSpawnPoint.rotation);
            NetworkServer.Spawn(unitInstance, connectionToClient);

            Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
            spawnOffset.y = unitSpawnPoint.position.y;

            UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();

            unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);

            _queuedUnits--;
            _unitTimer = 0;
        }

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleDie;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleDie;
        }

        [Server]
        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Command]
        private void CmdSpawnUnit()
        {
            if (_queuedUnits == maxUnitQueue) return;

            var player = connectionToClient.identity.GetComponent<RTSPlayer>();

            if (player.GetResources() < unitPrefab.GetResourceCost()) return;

            _queuedUnits++;

            player.SetResources(-unitPrefab.GetResourceCost());
        }

        #endregion

        #region Client

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (!isOwned) return;

            CmdSpawnUnit();
        }

        private void UpdateTimerDisplay()
        {
            float newProgess = _unitTimer / unitSpawnDuration;

            if (newProgess < unitProgressImage.fillAmount)
            {
                unitProgressImage.fillAmount = newProgess;
            }
            else
            {
                unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, newProgess,
                    ref _progressImageVelocity, 0.1f);
            }
        }

        private void ClientHandleQueuedUnitsUpdated(int oldQueuedUnits, int newQueuedUnits)
        {
            remainingUnitsText.text = newQueuedUnits.ToString();
        }

        #endregion
    }
}