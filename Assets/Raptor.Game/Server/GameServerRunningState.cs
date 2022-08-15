using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly Dictionary<IPEndPoint, ServerPlayer> _players = new();

        public void OnEnter(GameServer gameServer)
        {
            gameServer.Client = new RaptorClient(Configuration.ServerPort);
            gameServer.Client.RegisterRequestHandler<Ping>(ReplyWithPong);
            gameServer.Client.RegisterRequestHandler<GetServerTime>(ReplyWithServerTime);
            gameServer.Client.RegisterRequestHandler<GetServerTick>(ReplyWithServerTick);
            gameServer.Client.RegisterRequestHandler<GetPlayerInfo>(ReplyWithPlayerInfo);
            gameServer.Client.RegisterMessageHandler<PlayerCommand>(EnqueuePlayerCommand);
            _thread = new Thread(Loop);
            _thread.Start();
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

                foreach (var player in _players.Values)
                {
                    if (!player.CommandBuffer.TryGet(c => c.Tick == _tick, out var command))
                    {
                        Debug.LogWarning($"Theres no command for tick {_tick}");
                    }

                    player.CommandBuffer.RemoveAll(c => c.Tick <= _tick);
                    
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        var translation = new Vector3(command.Horizontal, command.Vertical, 0) * (float)Configuration.TickInterval.TotalSeconds * 4;
                        player.transform.Translate(translation);
                    });
                }
            }
        }
    }
}