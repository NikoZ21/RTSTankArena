using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Networking;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform miniMapRect = null;
    [SerializeField] private float mapScale = 20;
    [SerializeField] private float offset = -5;

    private Transform _playerCameraTransform;

    private void Update()
    {
        if (_playerCameraTransform != null) return;

        if (NetworkClient.connection.identity == null) return;

        _playerCameraTransform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().GetCameraTransform();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        Vector2 mouspos = Input.mousePosition;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapRect,
                mouspos,
                null,
                out Vector2 localPoint)) return;


        Vector2 lerp = new Vector2(
            (localPoint.x - miniMapRect.rect.x) / miniMapRect.rect.width,
            (localPoint.y - miniMapRect.rect.y) / miniMapRect.rect.height);


        Vector3 newCameraPos = new Vector3(Mathf.Lerp(-mapScale, mapScale, lerp.x),
            _playerCameraTransform.position.y,
            Mathf.Lerp(-mapScale, mapScale, lerp.y)
        );

        _playerCameraTransform.position = newCameraPos + new Vector3(0, 0, offset);
    }
}