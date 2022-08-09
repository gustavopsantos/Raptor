using UnityEngine;

namespace Raptor.Chat.Server
{
    public class ChatServerRunningState : ChatServerState
    {
        private const int AnimationFrequency = 2;

        private readonly string[] _runningAnimation = new string[]
        {
            "Running",
            "Running.",
            "Running..",
            "Running...",
        };

        public override void Present(ChatServer chatServer)
        {
            var frame = ((int) (Time.time * AnimationFrequency)) % _runningAnimation.Length;
            GUILayout.Label(_runningAnimation[frame], "Button");
        }
    }
}