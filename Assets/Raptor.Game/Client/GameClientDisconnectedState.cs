using System;
using System.Net;
using System.Linq;
using UnityEngine;
using System.Net.Sockets;
using Raptor.Game.Client.NetworkStatistics;
using Raptor.Game.Shared;

namespace Raptor.Game.Client
{
    public class GameClientDisconnectedState : IGameClientState
    {
        private string _chatServerAddress = "localhost";

        public void OnEnter(GameClient gameClient)
        {
        }

        public void Present(GameClient gameClient)
        {
            using (new GUILayout.HorizontalScope())
            {
                _chatServerAddress = GUILayout.TextField(_chatServerAddress);

                if (GUILayout.Button("Join"))
                {
                    Join(gameClient);
                }
            }
        }

        public void OnExit(GameClient gameClient)
        {
        }

        private static IPAddress ResolveHost(string host)
        {
            var entry = Dns.GetHostEntry(host);
            return entry.AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork);
        }

        private async void Join(GameClient gameClient)
        {
            gameClient.Client = new RaptorClient(0);
            var address = new IPEndPoint(ResolveHost(_chatServerAddress), Configuration.ServerPort);

            try
            {
                await gameClient.Client.ConnectAsync(address);
                await MeasureMedianRoundTripTime.Measure(address, gameClient.Client);
                gameClient.SwitchState(new GameClientConnectedState());
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not connect to chat server hosted at {address} {e}");
            }
        }
    }
}