using _Scripts.Networking;
using Mirror;
using Mirror.Examples.Basic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace _Scripts.Buildings
{
    public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Building building;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private LayerMask floorMask;

        private Camera _mainCamera;
        private BoxCollider _buildingCollider;
        private RTSPlayer _player;
        [SerializeField] private GameObject _buildingPreviewInstance;
        private Renderer _buildingRendererInstance;

        private void Start()
        {
            _mainCamera = Camera.main;

            iconImage.sprite = building.GetIcon();
            priceText.text = building.GetPrice().ToString();

            _buildingCollider = building.GetComponent<BoxCollider>();
        }

        private void Update()
        {
            if (!_player)
            {
                _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            }

            if (!_buildingPreviewInstance) return;

            UpdateBuildingPreview();
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (_player.GetResources() < building.GetPrice()) return;

            _buildingPreviewInstance = Instantiate(building.GetBuildingReview());
            _buildingRendererInstance = _buildingPreviewInstance.GetComponentInChildren<Renderer>();

            _buildingPreviewInstance.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_buildingPreviewInstance) return;

            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
            {
                _player.CmdPlaceBuilding(building.GetId(), hit.point);
            }

            Destroy(_buildingPreviewInstance);
        }

        private void UpdateBuildingPreview()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) return;

            _buildingPreviewInstance.transform.position = hit.point;

            if (!_buildingPreviewInstance.activeSelf)
            {
                _buildingPreviewInstance.SetActive(true);
            }


            Color color = _player.CanPlaceBuilding(_buildingCollider, hit.point) ? Color.green : Color.red;

            _buildingRendererInstance.material.SetColor("_Color", color);
        }
    }
}