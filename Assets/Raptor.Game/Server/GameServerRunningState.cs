using System.Threading;
using UnityEngine;
using Raptor.Game.Shared;
using Raptor.Interface;
using Ping = Raptor.Game.Shared.Ping;

namespace Raptor.Game.Server
{
    public class GameServerRunningState : IGameServerState
    {
        public void OnEnter(GameServer gameServer)
        {
            gameServer.Client = new RaptorClient(Configuration.ServerPort);
            gameServer.Client.RegisterRequestHandler<Ping>(ReplyWithPong);
        }

        private void ReplyWithPong(Sequence<Ping> pingRequest)
        {
            pingRequest.Reply(new Pong(), CancellationToken.None);
        }

        public void Present(GameServer gameServer)
        {
            GUILayout.Label("Running...");
        }

        public void OnExit(GameServer gameServer)
        {
            gameServer.Client.Dispose();
        }
    }
}