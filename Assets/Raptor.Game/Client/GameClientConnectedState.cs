using UnityEngine;

namespace Raptor.Game.Client
{
    public class GameClientConnectedState : IGameClientState
    {
        public void OnEnter(GameClient gameClient)
        {
        }

        public void Present(GameClient gameClient)
        {
            GUILayout.Label("Connected");
        }

        public void OnExit(GameClient gameClient)
        {
            gameClient.Client.Dispose();
        }
    }
}