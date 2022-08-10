using UnityEngine;

namespace Raptor.Game.Server
{
    public class GameServerStoppedState : IGameServerState
    {
        public void OnEnter(GameServer gameServer)
        {
        }

        public void Present(GameServer gameServer)
        {
            if (GUILayout.Button("Start"))
            {
                gameServer.SwitchState(new GameServerRunningState());
            }
        }

        public void OnExit(GameServer gameServer)
        {
        }
    }
}