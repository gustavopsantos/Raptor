using UnityEngine;

namespace Raptor.Chat.Server
{
    public class ChatServer : MonoBehaviour
    {
        private ChatServerState _current;
        
        private void Start()
        {
            _current = new ChatServerStoppedState();
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

        public void SwitchState(ChatServerState state)
        {
            _current = state;
        }
    }
}
