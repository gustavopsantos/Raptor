using System;
using System.Net;
using Raptor.Game.Client.Clock;
using Raptor.Game.Shared;
using UnityEngine;
using Timer = Raptor.Game.Shared.Timing.Timer;

namespace Raptor.Game.Client
{
    public class GameClientConnectedState : IGameClientState
    {
        private readonly TimeClient _timeClient;
        private readonly Timer _timer;

        public GameClientConnectedState(GameClient gameClient, TimeClient timeClient, DateTime serverTimerStartedAt)
        {
            _timeClient = timeClient;
            _timer = new Timer(t => Loop(t, gameClient), Configuration.TickInterval, () => timeClient.Time, serverTimerStartedAt - Configuration.CommandBuffer);
        }

        private void Loop(double tick, GameClient gameClient)
        {
            var server = new IPEndPoint(IPAddress.Loopback, Configuration.ServerPort);

            var command = new PlayerCommand(
                (int) tick,
                gameClient.InputStorage.Horizontal,
                gameClient.InputStorage.Vertical);

            gameClient.InputStorage.Consume();
            gameClient.Client.SendMessageUnreliable(command, server);
        }

        public void OnEnter(GameClient gameClient)
        {
        }

        public void Present(GameClient gameClient)
        {
            GUILayout.Label("Connected");
            GUILayout.Label($"Tick: {_timer.Tick}");
            GUILayout.Label($"Time: {_timeClient.Time.ToString("h:mm:ss.fff")}");
        }

        public void OnExit(GameClient gameClient)
        {
            _timer.Stop();
            gameClient.Client.Dispose();
        }
    }
}