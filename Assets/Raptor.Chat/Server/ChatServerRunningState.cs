using UnityEngine;
using System.Threading;
using Raptor.Interface;
using Raptor.Chat.Shared;

namespace Raptor.Chat.Server
{
    public class ChatServerRunningState : IChatServerState
    {
        public void OnEnter(ChatServer chatServer)
        {
            chatServer.Client = new RaptorClient(Configuration.ServerPort);
            chatServer.Client.RegisterMessageHandler<ChatMessage>(BroadcastChatMessage);
        }

        public void Present(ChatServer chatServer)
        {
            GUILayout.Label("Running...", "Button");
        }

        public void OnExit(ChatServer chatServer)
        {
            chatServer.Client.Dispose();
        }

        private void BroadcastChatMessage(Message<ChatMessage> packet)
        {
            packet.Mean.BroadcastMessageReliable(packet.Payload, CancellationToken.None);
        }
    }
}