using UnityEngine;
using JetBrains.Annotations;

namespace Raptor.Chat.Server
{
    public class ChatServer : MonoBehaviour
    {
        private IChatServerState _current;
        public RaptorClient Client { get; set; }

        private void Start()
        {
            SwitchState(new ChatServerStoppedState());
        }

        private void OnDestroy()
        {
            SwitchState(null);
        }

        private void OnGUI()
        {
            using (new GUILayout.AreaScope(new Rect(Screen.width - 8 - 200, 8, 200, 100)))
            {
                using (new GUILayout.VerticalScope("box", GUILayout.Width(200)))
                {
                    GUILayout.Label("Chat Server");
                    _current.Present(this);
                }
            }
        }

        public void SwitchState([CanBeNull] IChatServerState state)
        {
            _current?.OnExit(this);
            _current = state;
            _current?.OnEnter(this);
        }
    }
}