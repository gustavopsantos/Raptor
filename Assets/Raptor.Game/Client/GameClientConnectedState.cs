using System;
using System.Net;
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

        public GameClientConnectedState(GameClient gameClient, TimeClient timeClient, int serverTick, TimeSpan rtt)
        {
            _tick = serverTick + (int)Math.Ceiling(Configuration.TickRate * (rtt + Configuration.CommandBuffer).TotalSeconds);
            _timeClient = timeClient;
            _thread = new Thread(() => Loop(gameClient));
            _thread.Start();
        }

        private void Loop(GameClient gameClient)
        {
            var server = new IPEndPoint(IPAddress.Loopback, Configuration.ServerPort);
            
            while (true)
            {
                Thread.Sleep(Configuration.TickInterval);
                _tick++;
                
                var command = new PlayerCommand(
                    _tick,
                    gameClient.InputStorage.Horizontal,
                    gameClient.InputStorage.Vertical);
                
                gameClient.InputStorage.Consume();
                gameClient.Client.SendMessageUnreliable(command, server);
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