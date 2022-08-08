using System;
using System.Net;
using System.Threading;
using EasyButtons;
using UnityEngine;

namespace Raptor.Sample
{
    public class RequestSample : MonoBehaviour
    {
        [Serializable]
        private readonly struct GetDateTime
        {
        }

        [Button]
        private async void Execute()
        {
            TimeProfiler.Start();
            var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(128));
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 12345);
            var clientAddress = new IPEndPoint(IPAddress.Loopback, 54321);

            using var server = new RaptorClient(serverAddress.Port);
            using var client = new RaptorClient(clientAddress.Port);

            server.RegisterRequestHandler<GetDateTime>(dateTimeRequest =>
            {
                Debug.Log("Server received date time request!");
                dateTimeRequest.Reply(DateTime.UtcNow, timeout.Token);
            });

            await client.ConnectAsync(serverAddress);
            var response = await client.Request<GetDateTime, DateTime>(new GetDateTime(), serverAddress, timeout.Token).ConfigureAwait(false);
            Debug.Log($"Server time is: {response.Payload}");
        }
    }
}