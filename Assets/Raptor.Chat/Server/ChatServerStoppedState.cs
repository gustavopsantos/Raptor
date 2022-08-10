using UnityEngine;

namespace Raptor.Chat.Server
{
    public class ChatServerStoppedState : IChatServerState
    {
        public void OnEnter(ChatServer chatServer)
        {
        }

        public void Present(ChatServer chatServer)
        {
            if (GUILayout.Button("Start"))
            {
                chatServer.SwitchState(new ChatServerRunningState());
            }
        }

        public void OnExit(ChatServer chatServer)
        {
        }
    }
}