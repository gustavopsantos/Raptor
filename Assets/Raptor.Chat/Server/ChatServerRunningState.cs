using Raptor.Chat.Shared;
using Raptor.Interface;
using UnityEngine;

namespace Raptor.Chat.Server
{
    public class ChatServerRunningState : IChatServerState
    {
        private const int AnimationFrequency = 2;
        private readonly string[] _runningAnimation = {"Running.", "Running..", "Running..."};

        public void OnEnter(ChatServer chatServer)
        {
            Debug.Log("OnStateEnter - ChatServerRunningState");
            chatServer.Client = new RaptorClient(Configuration.ServerPort);
            chatServer.Client.RegisterMessageHandler<ChatMessage>(EnlistChatMessage);
        }

        public void Present(ChatServer chatServer)
        {
            var frame = ((int) (Time.time * AnimationFrequency)) % _runningAnimation.Length;
            GUILayout.Label(_runningAnimation[frame], "Button");
        }

        public void OnExit(ChatServer chatServer)
        {
            Debug.Log("OnStateDisposed - ChatServerRunningState");
            chatServer.Client.Dispose();
        }

        // TODO Create a interface for message handler and instantiate handler
        private void EnlistChatMessage(Message<ChatMessage> packet)
        {
        }
    }
}