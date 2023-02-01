using _Scripts.Buildings;
using _Scripts.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Units
{
    public class UnitCommander : MonoBehaviour
    {
        [SerializeField] private UnitSelectionManager unitSelectionManager = null;
        [SerializeField] private LayerMask groundMask;
        private Camera _mainCamera;

        void Start()
        {
            _mainCamera = Camera.main;
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;

            if (hit.collider.TryGetComponent(out Targetable target))
            {
                if (target.isOwned)
                {
                    TryToMoveUnits(hit.point);
                }

                TryTarget(target.gameObject);
                return;
            }

            TryToMoveUnits(hit.point);
        }

        private void TryTarget(GameObject target)
        {
            foreach (var unit in unitSelectionManager.SelectedUnits)
            {
                unit.GetComponent<Targeter>().CmdSetTarget(target);
            }
        }

        private void TryToMoveUnits(Vector3 destination)
        {
            foreach (var unit in unitSelectionManager.SelectedUnits)
            {
                unit.GetComponent<UnitMovement>().CmdMove(destination);
            }
        }

        private void ClientHandleGameOver(string obj)
        {
            enabled = false;
        }
    }
}