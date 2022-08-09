using UnityEngine;

namespace Raptor.Chat.Client
{
    public class ChatClient : MonoBehaviour
    {
        private ChatClientState _current;

        private void Start()
        {
            _current = new ChatClientDisconnectedState();
        }

        private void OnGUI()
        {
            using (new GUILayout.AreaScope(new Rect(8, 8, Screen.width - 16, Screen.height - 16)))
            {
                using (new GUILayout.VerticalScope("box", GUILayout.Width(240)))
                {
                    GUILayout.Label("Chat Client");
                    _current.Present(this);
                }
            }
        }

        public void SwitchState(ChatClientState state)
        {
            _current = state;
        }
    }
}