using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Raptor.Chat.Client
{
    public class ChatClientDisconnectedState : ChatClientState
    {
        private string _chatServerAddress = "localhost";

        public override void Present(ChatClient chatClient)
        {
            using (new GUILayout.HorizontalScope())
            {
                _chatServerAddress = GUILayout.TextField(_chatServerAddress);

                if (GUILayout.Button("Join"))
                {
                    Join();
                    chatClient.SwitchState(new ChatClientConnectedState());
                }
            }
        }

        private IPAddress ResolveHost(string host)
        {
            var entry = Dns.GetHostEntry(host);
            var ipv4 = entry.AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork);
            return ipv4;
        }

        private void Join()
        {
            var ip = ResolveHost(_chatServerAddress);
        }
    }
}