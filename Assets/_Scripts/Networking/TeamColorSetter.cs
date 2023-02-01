using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace _Scripts.Networking
{
    public class TeamColorSetter : NetworkBehaviour
    {
        [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

        [SyncVar(hook = nameof(HandleTeamColorUpdated))]
        private Color _teamColor = new Color();

        #region Server

        public override void OnStartServer()
        {
            RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

            _teamColor = player.GetTeamColor();
        }

        #endregion

        #region Client

        private void HandleTeamColorUpdated(Color oldColor, Color newColor)
        {
            foreach (var renderer in colorRenderers)
            {
                var listMaterials = new List<Material>();
                renderer.GetMaterials(listMaterials);
                foreach (var material in listMaterials)
                {
                    material.color = newColor;
                }
            }
        }

        #endregion
    }
}