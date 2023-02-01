using _Scripts.Buildings;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject _unitSpawnerPrefab = null;
        [SerializeField] private GameOverHandler _gameOverHandlerPrefab;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            var player = conn.identity.GetComponent<RTSPlayer>();

            player.SetTeamColor(new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            ));

            GameObject unitSpawner = Instantiate(_unitSpawnerPrefab, conn.identity.transform.position,
                conn.identity.transform.rotation);
            NetworkServer.Spawn(unitSpawner, conn);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                GameOverHandler gameOverHandler = Instantiate(_gameOverHandlerPrefab);
                NetworkServer.Spawn(gameOverHandler.gameObject);
            }
        }
    }
}