using UnityEngine;
using Raptor.Chat.Shared;

namespace Raptor.Chat.Server
{
    public class ChatServerRunningState : IChatServerState
    {
        public void OnEnter(ChatServer chatServer)
        {
            chatServer.Client = new RaptorClient(Configuration.ServerPort);
            chatServer.Client.RegisterHandler(new ChatMessageHandler());
        }

        public void Present(ChatServer chatServer)
        {
            GUILayout.Label("Running...", "Button");
        }

        public void OnExit(ChatServer chatServer)
        {
            chatServer.Client.Dispose();
        }
    }
}