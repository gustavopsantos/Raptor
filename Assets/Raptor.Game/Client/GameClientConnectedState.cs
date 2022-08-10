using Raptor.Game.Shared;
using UnityEngine;

namespace Raptor.Game.Client
{
    public class GameClientConnectedState : IGameClientState
    {
        private readonly TimeClient _timeClient;

        public GameClientConnectedState(TimeClient timeClient)
        {
            _timeClient = timeClient;
        }

        public void OnEnter(GameClient gameClient)
        {
        }

        public void Present(GameClient gameClient)
        {
            GUILayout.Label("Connected");
            GUILayout.Label($"Time: {_timeClient.Time.Print()}");
        }

        public void OnExit(GameClient gameClient)
        {
            gameClient.Client.Dispose();
        }
    }
}