using System.Net;
using Raptor.ValueObjects;

namespace Raptor.Connection
{
    internal class Connection
    {
        public IPEndPoint EndPoint;
        public ConnectionState State;
        public PacketSequenceStorage SequenceStorage;
    }
}