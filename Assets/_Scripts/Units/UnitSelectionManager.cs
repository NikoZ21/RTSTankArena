using System.Collections.Generic;
using _Scripts.Buildings;
using _Scripts.Networking;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Units
{
    public class UnitSelectionManager : MonoBehaviour
    {
        [SerializeField] private RectTransform unitSelectionArea = null;
        [SerializeField] private LayerMask layerMask;
        public List<Unit> SelectedUnits { get; } = new List<Unit>();
        private Vector2 startPos;

        private Camera _mainCamera;
        private RTSPlayer _player;

        private void Start()
        {
            _mainCamera = Camera.main;

            Unit.onAuthorityUnitDespawned += AuthorityHandleUnityDespawned;
        }

        private void OnDestroy()
        {
            Unit.onAuthorityUnitDespawned -= AuthorityHandleUnityDespawned;
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        void Update()
        {
            if (!_player)
            {
                _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelectionArea();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSeelctionArea();
            }
        }

        private void ClearSelectionArea()
        {
            unitSelectionArea.gameObject.SetActive(false);

            if (unitSelectionArea.sizeDelta.magnitude == 0)
            {
                Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

                if (!hit.collider.TryGetComponent(out Unit unit)) return;

                if (!unit.isOwned) return;

                SelectedUnits.Add(unit);

                foreach (Unit selectedUnit in SelectedUnits)
                {
                    selectedUnit.Select();
                }

                return;
            }

            Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
            Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);


            foreach (var unit in _player.GetMyUnits())
            {
                if (SelectedUnits.Contains(unit)) continue;

                Vector2 screenPosition = _mainCamera.WorldToScreenPoint(unit.transform.position);

                if (screenPosition.x > min.x &&
                    screenPosition.x < max.x &&
                    screenPosition.y > min.y &&
                    screenPosition.y < max.y)
                {
                    SelectedUnits.Add(unit);
                    unit.Select();
                }
            }
        }

        private void StartSelectionArea()
        {
            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                foreach (var selectedUnit in SelectedUnits)
                {
                    selectedUnit.Deselect();
                }

                SelectedUnits.Clear();
            }


            unitSelectionArea.gameObject.SetActive(true);

            startPos = Mouse.current.position.ReadValue();

            UpdateSeelctionArea();
        }

        private void UpdateSeelctionArea()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            float areaWidth = mousePos.x - startPos.x;
            float areaheight = mousePos.y - startPos.y;

            unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaheight));
            unitSelectionArea.anchoredPosition = startPos + new Vector2(areaWidth / 2, areaheight / 2);
        }

        private void AuthorityHandleUnityDespawned(Unit unit)
        {
            SelectedUnits.Remove(unit);
        }

        private void ClientHandleGameOver(string obj)
        {
            enabled = false;
        }
    }
}