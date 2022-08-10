using System.Threading;
using UnityEngine;
using Raptor.Game.Shared;
using Raptor.Interface;
using Ping = Raptor.Game.Shared.Ping;

namespace Raptor.Game.Server
{
    public class GameServerRunningState : IGameServerState
    {
        private int _tick;
        private Thread _tickingThread;
        
        public void OnEnter(GameServer gameServer)
        {
            gameServer.Client = new RaptorClient(Configuration.ServerPort);
            gameServer.Client.RegisterRequestHandler<Ping>(ReplyWithPong);
            _tickingThread = new Thread(Tick);
            _tickingThread.Start();
        }

        private void ReplyWithPong(Sequence<Ping> pingRequest)
        {
            pingRequest.Reply(new Pong(), CancellationToken.None);
        }

        public void Present(GameServer gameServer)
        {
            GUILayout.Label("Running...");
            GUILayout.Label($"Tick: {_tick}");
            GUILayout.Label($"TickRate: {Configuration.TickRate}");
        }

        public void OnExit(GameServer gameServer)
        {
            _tickingThread.Abort();
            gameServer.Client.Dispose();
        }

        private void Tick()
        {
            while (true)
            {
                Thread.Sleep(Configuration.TickInterval);
                _tick++;
            }
        }
    }
}