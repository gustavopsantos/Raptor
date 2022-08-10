using System;
using System.Threading;
using Raptor.Game.Shared;
using Raptor.Interface;
using UnityEngine;

namespace Raptor.Game.Client
{
    public class GameClientConnectedState : IGameClientState
    {
        private int _tick;
        private Thread _thread;
        private readonly TimeClient _timeClient;

        public GameClientConnectedState(TimeClient timeClient, int serverTick, TimeSpan rtt)
        {
            _tick = serverTick + (int)Math.Ceiling(Configuration.TickRate * (rtt + Configuration.CommandBuffer).TotalSeconds);
            _timeClient = timeClient;
            _thread = new Thread(Loop);
            _thread.Start();
        }

        private void Loop()
        {
            while (true)
            {
                Thread.Sleep(Configuration.TickInterval);
                _tick++;
            }
        }

        public void OnEnter(GameClient gameClient)
        {
        }

        public void Present(GameClient gameClient)
        {
            GUILayout.Label("Connected");
            GUILayout.Label($"Tick: {_tick}");
            GUILayout.Label($"Time: {_timeClient.Time.Print()}");
        }

        public void OnExit(GameClient gameClient)
        {
            _thread.Abort();
            gameClient.Client.Dispose();
        }
    }
}