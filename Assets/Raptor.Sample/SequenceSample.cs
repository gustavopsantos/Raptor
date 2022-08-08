using System;
using System.Net;
using System.Threading;
using EasyButtons;
using Raptor.Interface;
using UnityEngine;

namespace Raptor.Sample
{
    public class SequenceSample : MonoBehaviour
    {
        [Button]
        private async void Execute()
        {
            TimeProfiler.Start();
            var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(512));
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 12345);
            var clientAddress = new IPEndPoint(IPAddress.Loopback, 54321);

            using var server = new RaptorClient(serverAddress.Port);
            using var client = new RaptorClient(clientAddress.Port);

            async void ConversationHandler(Sequence<string> nameRequest)
            {
                Debug.Log("Server received conversation request!");
                var typeRequest = await nameRequest.ReplyAndAwait<string, string>("My name is Server!", timeout.Token);
                await typeRequest.Reply("Im authoritative!", timeout.Token);
                Debug.Log("Server returned");
            }

            server.RegisterRequestHandler<string>(ConversationHandler);

            await client.ConnectAsync(serverAddress);
            var nameResponse = await client.Request<string, string>("Whats your name?", serverAddress, timeout.Token);
            Debug.Log($"Server name is: {nameResponse.Payload}");
            var typeResponse = await nameResponse.ReplyAndAwait<string, string>("Whats your type?", timeout.Token);
            Debug.Log($"Server type is: {typeResponse.Payload}");
            Debug.Log("Client returned");
        }
    }
}
