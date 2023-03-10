using System;
using System.Security.Cryptography.X509Certificates;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Cameras
{
    public class CameraController : NetworkBehaviour
    {
        [SerializeField] private Transform playerCameraTransform = null;
        [SerializeField] private float speed = 20f;
        [SerializeField] private float screenBorderThickness = 10f;
        [SerializeField] private Vector2 screenXLimits = Vector2.zero;
        [SerializeField] private Vector2 screenZLimits = Vector2.zero;

        private Controls _controls;
        private Vector2 _previousInput;

        public override void OnStartAuthority()
        {
            playerCameraTransform.gameObject.SetActive(true);

            _controls = new Controls();

            _controls.Player.MoveCamera.performed += SetPreviousInput;
            _controls.Player.MoveCamera.canceled += SetPreviousInput;

            _controls.Enable();
        }

        [ClientCallback]
        private void Update()
        {
            if (!isOwned || !Application.isFocused) return;

            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            Vector3 pos = playerCameraTransform.position;

            // if (_previousInput == Vector2.zero)
            // {
            //     Vector3 cursorMovement = Vector3.zero;
            //
            //     Vector2 cursorPosition = Mouse.current.position.ReadValue();
            //
            //     if (cursorPosition.y >= Screen.height - screenBorderThickness)
            //     {
            //         cursorMovement.z += 1;
            //     }
            //     else if (cursorPosition.y <= screenBorderThickness)
            //     {
            //         cursorMovement.z -= 1;
            //     }
            //
            //     if (cursorPosition.x >= Screen.height - screenBorderThickness)
            //     {
            //         cursorMovement.x += 1;
            //     }
            //     else if (cursorPosition.x <= screenBorderThickness)
            //     {
            //         cursorMovement.x -= 1;
            //     }
            //
            //     pos += cursorMovement.normalized * speed * Time.deltaTime;
            // }
            // else
            // {
            pos += new Vector3(_previousInput.x, 0, _previousInput.y) * speed * Time.deltaTime;
            //}

            pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
            pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

            playerCameraTransform.position = pos;
        }

        private void SetPreviousInput(InputAction.CallbackContext ctx)
        {
            _previousInput = ctx.ReadValue<Vector2>();
        }
    }
}