using _Scripts.Combat;
using _Scripts.Networking;
using Mirror;
using UnityEngine;

namespace _Scripts.Buildings
{
    public class ResourceGenerator : NetworkBehaviour
    {
        [SerializeField] private Health health = null;
        [SerializeField] private int resourcesPerInterval = 10;
        [SerializeField] private float interval = 2;

        private float timer;
        private RTSPlayer _player;

        public override void OnStartServer()
        {
            timer = interval;
            _player = connectionToClient.identity.GetComponent<RTSPlayer>();

            health.ServerOnDie += ServerHnadleDie;
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHnadleDie;
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [ServerCallback]
        private void Update()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                _player.SetResources(resourcesPerInterval);

                timer += interval;
            }
        }

        private void ServerHandleGameOver()
        {
            NetworkServer.Destroy(gameObject);
        }

        private void ServerHnadleDie()
        {
            enabled = false;
        }
    }
}