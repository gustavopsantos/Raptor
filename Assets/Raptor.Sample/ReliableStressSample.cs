using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EasyButtons;
using UnityEngine;

namespace Raptor.Sample
{
    public class ReliableStressSample : MonoBehaviour
    {
        [Button]
        private async void Execute()
        {
            TimeProfiler.Start();
            var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 12345);
            var clientAddress = new IPEndPoint(IPAddress.Loopback, 54321);

            using var server = new RaptorClient(serverAddress.Port);
            using var client = new RaptorClient(clientAddress.Port);

            server.RegisterMessageHandler<int>(stringMessage =>
            {
                Debug.Log($"Server received: {stringMessage.Payload}");
            });

            await client.ConnectAsync(serverAddress);
            
            for (int i = 0; i < 1024; i++)
            {
                client.SendMessageReliable(i + 1, serverAddress, timeout.Token); // Not awaiting ack on purpose
            }

            await Task.Delay(TimeSpan.FromSeconds(1), timeout.Token);
        }
    }
}