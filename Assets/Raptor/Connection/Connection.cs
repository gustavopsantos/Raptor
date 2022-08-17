using System;
using System.Net;
using Raptor.ValueObjects;

namespace Raptor.Connection
{
    internal class Connection : IDisposable
    {
        public IPEndPoint Endpoint;
        public ConnectionState State;
        public PacketSequenceStorage SequenceStorage;
        public RetransmissionQueue RetransmissionQueue;

        public void Dispose()
        {
            RetransmissionQueue.Dispose();
        }
    }
}