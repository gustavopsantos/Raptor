using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Raptor.Game.Server.Clock;
using Raptor.Game.Server.GameInput;
using Raptor.Game.Server.RTTMeasurement;
using Raptor.Game.Server.Timing;
using Raptor.Game.Shared;
using Raptor.Interface;
using UnityEngine;
using Input = Raptor.Game.Shared.GameInput.Input;
using Object = UnityEngine.Object;
using Timer = Raptor.Game.Shared.Timing.Timer;

namespace Raptor.Game.Server.StateMachine
{
    public class GameServerRunningState : IGameServerState
    {
        private Timer _timer;
        private readonly Dictionary<IPEndPoint, ServerPlayer> _players = new();

        public void OnEnter(GameServer gameServer)
        {
            _timer = new Timer(Tick, Configuration.TickInterval, () => DateTime.UtcNow);
            gameServer.Client = new RaptorClient(Configuration.ServerPort);
            gameServer.Client.RegisterHandler(new TimeServer());
            gameServer.Client.RegisterHandler(new TimingServer(_timer));
            gameServer.Client.RegisterHandler(new PingRequestHandler());
            gameServer.Client.RegisterHandler(new InputMessageHandler(_players));
            gameServer.Client.RegisterRequestHandler<GetPlayerInfo>(ReplyWithPlayerInfo);
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
                serverPlayer.CommandBuffer = new List<Ticked<Input>>();
                _players.Add(getPlayerInfoRequest.Source, serverPlayer);
            });

            await getPlayerInfoRequest.Reply(playerInfo, CancellationToken.None);
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
                    var input = new Vector2(command.Value.Horizontal, command.Value.Vertical);
                    var translation = (float )Configuration.TickInterval.TotalSeconds * 4 * input;
                    player.transform.Translate(translation);
                });
            }
        }
    }
}