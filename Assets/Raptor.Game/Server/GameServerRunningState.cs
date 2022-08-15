using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using Raptor.Game.Shared;
using Raptor.Interface;
using Object = UnityEngine.Object;
using Ping = Raptor.Game.Shared.Ping;
using Timer = Raptor.Game.Shared.Timer;

namespace Raptor.Game.Server
{
    public class GameServerRunningState : IGameServerState
    {
        private Timer _timer;
        private readonly Dictionary<IPEndPoint, ServerPlayer> _players = new();

        public void OnEnter(GameServer gameServer)
        {
            gameServer.Client = new RaptorClient(Configuration.ServerPort);
            gameServer.Client.RegisterRequestHandler<Ping>(ReplyWithPong);
            gameServer.Client.RegisterRequestHandler<GetServerTime>(ReplyWithServerTime);
            gameServer.Client.RegisterRequestHandler<GetServerTick>(ReplyWithServerTick);
            gameServer.Client.RegisterRequestHandler<GetPlayerInfo>(ReplyWithPlayerInfo);
            gameServer.Client.RegisterMessageHandler<PlayerCommand>(EnqueuePlayerCommand);
            gameServer.Client.RegisterRequestHandler<GetServerInfo>(ReplyWithServerInfo);
            _timer = new Timer(Tick, Configuration.TickInterval, () => DateTime.UtcNow);
        }

        private void ReplyWithServerInfo(Sequence<GetServerInfo> getServerInfoRequest)
        {
            getServerInfoRequest.Reply(new ServerInfo(_timer.StartedAt), CancellationToken.None);
        }

        private void EnqueuePlayerCommand(Message<PlayerCommand> msg)
        {
            var serverPlayer = _players[msg.Source];
            serverPlayer.CommandBuffer.Add(msg.Payload);
        }

        private async void ReplyWithPlayerInfo(Sequence<GetPlayerInfo> getPlayerInfoRequest)
        {
            var playerInfo = new PlayerInfo(Guid.NewGuid());

            await UnityMainThreadDispatcher.Instance().EnqueueAsync(() =>
            {
                var serverPlayerPrefab = Resources.Load<ServerPlayer>("ServerPlayer");
                var serverPlayer = Object.Instantiate(serverPlayerPrefab);
                serverPlayer.Id = playerInfo.PlayerId.ToString();
                serverPlayer.EndPoint = getPlayerInfoRequest.Source;
                serverPlayer.CommandBuffer = new List<PlayerCommand>();
                _players.Add(getPlayerInfoRequest.Source, serverPlayer);
            });

            await getPlayerInfoRequest.Reply(playerInfo, CancellationToken.None);
        }

        private void ReplyWithServerTick(Sequence<GetServerTick> getServerTickRequest)
        {
            getServerTickRequest.Reply(_timer.Tick, CancellationToken.None);
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
            GUILayout.Label($"Tick: {_timer.Tick}");
            GUILayout.Label($"Time: {DateTime.UtcNow.ToString("h:mm:ss.fff")}");
        }

        public void OnExit(GameServer gameServer)
        {
            _timer.Stop();
            gameServer.Client.Dispose();
        }

        private void Tick(double tick)
        {
            foreach (var player in _players.Values)
            {
                if (!player.CommandBuffer.TryGet(c => c.Tick == _timer.Tick, out var command))
                {
                    Debug.LogWarning($"Theres no command for tick {_timer.Tick}");
                }

                player.CommandBuffer.RemoveAll(c => c.Tick <= _timer.Tick);

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    var translation = new Vector3(command.Horizontal, command.Vertical, 0) * (float) Configuration.TickInterval.TotalSeconds * 4;
                    player.transform.Translate(translation);
                });
            }
        }
    }
}