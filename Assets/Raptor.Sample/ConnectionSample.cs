using System;
using System.Net;
using EasyButtons;
using UnityEngine;

namespace Raptor.Sample
{
    public class ConnectionSample : MonoBehaviour
    {
        [Button]
        private async void Execute()
        {
            TimeProfiler.Start();
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 12345);
            var clientAddress = new IPEndPoint(IPAddress.Loopback, 54321);

            using var server = new RaptorClient(serverAddress.Port);
            using var client = new RaptorClient(clientAddress.Port);

            try
            {
                await client.ConnectAsync(serverAddress);
                Debug.Log("Client connected");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}