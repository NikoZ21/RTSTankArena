using System;
using _Scripts.Networking;
using Mirror;
using TMPro;
using UnityEngine;

namespace _Scripts.Resources
{
    public class ResourceDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text resourcesText = null;

        private RTSPlayer _player;

        private void Update()
        {
            if (!_player)
            {
                _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

                if (_player != null)
                {
                    ClientHandleResourcesUpdated(_player.GetResources());
                    
                    _player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
                }
            }
        }

        private void OnDestroy()
        {
            _player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
        }

        private void ClientHandleResourcesUpdated(int resources)
        {
            resourcesText.text = $"Resources: {resources.ToString()}";
        }
    }
}