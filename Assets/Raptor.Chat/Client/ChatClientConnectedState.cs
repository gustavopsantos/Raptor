using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Raptor.Chat.Shared;
using Raptor.Interface;
using UnityEngine;

namespace Raptor.Chat.Client
{
    public class ChatClientConnectedState : IChatClientState
    {
        private string _composing;
        private Vector2 _scrollPosition;
        private readonly List<string> _messages = new();

        public void OnEnter(ChatClient chatClient)
        {
            chatClient.Client.RegisterMessageHandler<ChatMessage>(EnlistChatMessage);
        }

        private void EnlistChatMessage(Message<ChatMessage> packet)
        {
            _scrollPosition.y = float.MaxValue; // scroll to bottom
            _messages.Add(packet.Payload.Content);
        }

        public void Present(ChatClient chatClient)
        {
            using (new GUILayout.VerticalScope("box"))
            {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(100));

                foreach (var message in _messages)
                {
                    GUILayout.Label(message);
                }

                GUILayout.EndScrollView();
            }

            using (new GUILayout.HorizontalScope())
            {
                _composing = GUILayout.TextField(_composing);

                if (GUILayout.Button("Send", GUILayout.Width(42)))
                {
                    Send(chatClient);
                }
            }
        }

        public void OnExit(ChatClient chatClient)
        {
            chatClient.Client.Dispose();
        }

        private void Send(ChatClient chatClient)
        {
            var composed = _composing.Trim();

            if (composed == string.Empty)
            {
                return;
            }

            _composing = string.Empty;
            var chatMessage = new ChatMessage($"[{Environment.MachineName}] {composed}");
            var serverAddress = new IPEndPoint(IPAddress.Loopback, Configuration.ServerPort);
            chatClient.Client.SendMessageReliable(chatMessage, serverAddress, CancellationToken.None);
        }
    }
}