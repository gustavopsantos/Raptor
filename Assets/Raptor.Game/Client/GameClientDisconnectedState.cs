using System;
using System.Net;
using System.Linq;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using Raptor.Game.Client.Clock;
using Raptor.Game.Client.RTTMeasurement;
using Raptor.Game.Shared;
using Raptor.Game.Shared.Timing;

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
            gameClient.Client.RegisterHandler(new SnapshotHandler());

            var address = new IPEndPoint(ResolveHost(_chatServerAddress), Configuration.ServerPort);

            try
            {
                await gameClient.Client.ConnectAsync(address);
                var rtt = await MeasureMedianRoundTripTime.Measure(address, gameClient.Client);
                var timeClient = await TimeClient.Synchronized(address, gameClient.Client, rtt);
                var playerInfo = await gameClient.Client.Request<GetPlayerInfo, PlayerInfo>(new GetPlayerInfo(), address, CancellationToken.None);
                var timingInfo = await gameClient.Client.Request<GetTimingInfo, TimingInfo>(new GetTimingInfo(), address, CancellationToken.None);
                var localPlayerPrefab = Resources.Load<LocalPlayer>("LocalPlayer");
                var localPlayer = UnityEngine.Object.Instantiate(localPlayerPrefab);
                localPlayer.Setup(playerInfo.Payload);
                gameClient.SwitchState(new GameClientConnectedState(localPlayer, gameClient, timeClient, timingInfo.Payload.TimerStartedAt, rtt));
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not connect to chat server hosted at {address} {e}");
            }
        }
    }
}