using System;
using System.Threading;
using UnityEngine;
using Raptor.Game.Shared;
using Raptor.Interface;
using Object = UnityEngine.Object;
using Ping = Raptor.Game.Shared.Ping;

namespace Raptor.Game.Server
{
    public class GameServerRunningState : IGameServerState
    {
        private int _tick;
        private Thread _thread;
        
        public void OnEnter(GameServer gameServer)
        {
            gameServer.Client = new RaptorClient(Configuration.ServerPort);
            gameServer.Client.RegisterRequestHandler<Ping>(ReplyWithPong);
            gameServer.Client.RegisterRequestHandler<GetServerTime>(ReplyWithServerTime);
            gameServer.Client.RegisterRequestHandler<GetServerTick>(ReplyWithServerTick);
            gameServer.Client.RegisterRequestHandler<GetPlayerInfo>(ReplyWithPlayerInfo);
            _thread = new Thread(Loop);
            _thread.Start();
        }

        private async void ReplyWithPlayerInfo(Sequence<GetPlayerInfo> getPlayerInfoRequest)
        {
            var playerInfo = new PlayerInfo(Guid.NewGuid());

            await UnityMainThreadDispatcher.Instance().EnqueueAsync(() =>
            {
                var serverPlayerPrefab = Resources.Load<ServerPlayer>("ServerPlayer");
                var serverPlayer = Object.Instantiate(serverPlayerPrefab);
                serverPlayer.Setup(playerInfo);
            });
            
            await getPlayerInfoRequest.Reply(playerInfo, CancellationToken.None);
        }

        private void ReplyWithServerTick(Sequence<GetServerTick> getServerTickRequest)
        {
            getServerTickRequest.Reply(_tick, CancellationToken.None);
        }

        private void ReplyWithServerTime(Sequence<GetServerTime> getServerTimeRequest)
        {
            getServerTimeRequest.Reply(DateTime.UtcNow, CancellationToken.None);
        }

        private void ReplyWithPong(Sequence<Ping> pingRequest)
        {
            pingRequest.Reply(new Pong(), CancellationToken.None);
        }

        public void Present(GameServer gameServer)
        {
            GUILayout.Label("Running...");
            GUILayout.Label($"Tick: {_tick}");
            GUILayout.Label($"Time: {DateTime.UtcNow.Print()}");
        }

        public void OnExit(GameServer gameServer)
        {
            gameServer.Client.Dispose();
        }
        
        private void Loop()
        {
            while (true)
            {
                Thread.Sleep(Configuration.TickInterval);
                _tick++;
            }
        }
    }
}