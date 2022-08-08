using System;
using System.Net;
using EasyButtons;
using UnityEngine;
using System.Threading;

namespace Raptor.Sample
{
    public class ReliableMessageSample : MonoBehaviour
    {
        [Button]
        private async void Execute()
        {
            TimeProfiler.Start();
            var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(128));
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 12345);
            var clientAddress = new IPEndPoint(IPAddress.Loopback, 54321);

            using var server = new RaptorClient(serverAddress.Port);
            using var client = new RaptorClient(clientAddress.Port);

            server.RegisterMessageHandler<string>(stringMessage =>
            {
                Debug.Log($"Server received message: {stringMessage.Payload}");
            });

            await client.ConnectAsync(serverAddress);
            await client.SendMessageReliable("This is a reliable message!", serverAddress, timeout.Token);
            Debug.Log("Client returned");
        }
    }
}