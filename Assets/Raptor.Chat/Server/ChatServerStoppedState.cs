using UnityEngine;

namespace Raptor.Chat.Server
{
    public class ChatServerStoppedState : ChatServerState
    {
        public override void Present(ChatServer chatServer)
        {
            if (GUILayout.Button("Start"))
            {
                Start();
                chatServer.SwitchState(new ChatServerRunningState());
            }
        }

        private void Start()
        {
        }
    }
}