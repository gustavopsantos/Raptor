using System;
using System.Net;
using UnityEngine;
using System.Net.Sockets;

namespace Raptor
{
    public class DatagramClient : IDisposable
    {
        private readonly UdpClient _client;
        private readonly Action<byte[], IPEndPoint> _handler;

        public DatagramClient(Action<byte[], IPEndPoint> handler, int port)
        {
            _handler = handler;
            _client = new UdpClient(port);
            DoNotReportUnreachableEndPoint();
            _client.BeginReceive(Receive, _client);
        }

        private void DoNotReportUnreachableEndPoint()
        {
            // https://docs.microsoft.com/en-us/windows/win32/winsock/winsock-ioctls#sio_udp_connreset-opcode-setting-i-t3
            const int SIO_UDP_CONNRESET = -1744830452; //SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12
            _client.Client.IOControl((IOControlCode) SIO_UDP_CONNRESET, new byte[] {0, 0, 0, 0}, null);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public void SendDatagram(byte[] bytes, IPEndPoint recipient)
        {
            _client.Send(bytes, bytes.Length, recipient);
        }

        private void Receive(IAsyncResult ar)
        {
            try
            {
                var source = new IPEndPoint(IPAddress.Any, 0);
                var bytes = _client.EndReceive(ar, ref source);
                _handler?.Invoke(bytes, source);
            }
            catch (Exception e) when (e.GetType() != typeof(ObjectDisposedException))
            {
                Debug.LogError(e);
            }
            finally
            {
                _client.BeginReceive(Receive, _client);
            }
        }
    }
}