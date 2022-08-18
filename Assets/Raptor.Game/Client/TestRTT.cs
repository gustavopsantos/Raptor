using System.Net;
using EasyButtons;
using Raptor.Game.Client.RTTMeasurement;
using Raptor.Game.Shared;
using UnityEngine;

namespace Raptor.Game.Client
{
    public class TestRTT : MonoBehaviour
    {
        [SerializeField] private GameClient _gameClient;

        [Button]
        private async void MeasureRTT()
        {
            var server = new IPEndPoint(IPAddress.Loopback, Configuration.ServerPort);
            await MeasureMedianRoundTripTime.Measure(server, _gameClient.Client);
        }
    }
}