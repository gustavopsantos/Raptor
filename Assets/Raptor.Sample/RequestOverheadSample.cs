using System;
using System.Net;
using System.Threading;
using EasyButtons;
using UnityEngine;

namespace Raptor.Sample
{
    public class RequestOverheadSample : MonoBehaviour
    {
        [Button]
        private async void Execute()
        {
            Debug.LogWarning($"Unity Thread: {AppDomain.GetCurrentThreadId()}");
            
            TimeProfiler.Start();
            var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(256));
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 12345);
            var clientAddress = new IPEndPoint(IPAddress.Loopback, 54321);

            using var server = new RaptorClient(serverAddress.Port);
            using var client = new RaptorClient(clientAddress.Port);

            server.RegisterRequestHandler<byte>(byteRequest =>
            {
                Debug.Log($"Server received byte req at {TimeProfiler.Sample()}");
                byteRequest.Reply<short>(0, timeout.Token);
            });

            await client.ConnectAsync(serverAddress);
            
            Debug.Log($"Client sending request at {TimeProfiler.Sample()}");
            await client.Request<byte, short>(0, serverAddress, timeout.Token).ConfigureAwait(false);
            Debug.Log($"Client received response at: {TimeProfiler.Sample()} thread: {AppDomain.GetCurrentThreadId()}");
        }
    }
}