using System;
using System.Net;
using Raptor.Game.Client.Clock;
using Raptor.Game.Shared;
using UnityEngine;
using Input = Raptor.Game.Shared.GameInput.Input;
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
            var tickedInput = new Ticked<Input>((int) tick, gameClient.InputBuffer.Consume());
            gameClient.Client.SendMessageUnreliable(tickedInput, server);
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