using System.Net;
using System.Threading;
using EasyButtons;
using UnityEngine;

namespace Raptor.Sample
{
    public class UnreliableMessageSample : MonoBehaviour
    {
        [Button]
        private async void Execute()
        {
            TimeProfiler.Start();
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 12345);
            var clientAddress = new IPEndPoint(IPAddress.Loopback, 54321);

            using var server = new RaptorClient(serverAddress.Port);
            using var client = new RaptorClient(clientAddress.Port);

            server.RegisterMessageHandler<string>(stringMessage =>
            {
                Debug.Log($"Server received message: {stringMessage.Payload}");
            });

            await client.ConnectAsync(serverAddress);
            client.SendMessageUnreliable("This is a unreliable message!", serverAddress);

            Thread.Sleep(16); // Avoid clients being disposed before message is received
        }
    }
}