using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Raptor.Chat.Shared;
using UnityEngine;

namespace Raptor.Chat.Client
{
    public class ChatClientDisconnectedState : IChatClientState
    {
        private string _chatServerAddress = "localhost";

        public void OnEnter(ChatClient chatClient)
        {
        }

        public void Present(ChatClient chatClient)
        {
            using (new GUILayout.HorizontalScope())
            {
                _chatServerAddress = GUILayout.TextField(_chatServerAddress);

                if (GUILayout.Button("Join"))
                {
                    Join(chatClient);
                }
            }
        }

        public void OnExit(ChatClient chatClient)
        {
        }

        private static IPAddress ResolveHost(string host)
        {
            var entry = Dns.GetHostEntry(host);
            return entry.AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork);
        }

        private async void Join(ChatClient chatClient)
        {
            chatClient.Client = new RaptorClient(0);
            var address = new IPEndPoint(ResolveHost(_chatServerAddress), Configuration.ServerPort);

            try
            {
                await chatClient.Client.ConnectAsync(address);
                chatClient.SwitchState(new ChatClientConnectedState());
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not connect to chat server hosted at {address} {e}");
            }
        }
    }
}