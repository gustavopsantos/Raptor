using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Raptor.Chat.Client
{
    public class ChatClient : MonoBehaviour
    {
        private IChatClientState _current;
        public RaptorClient Client { get; set; }

        private void Start()
        {
            SwitchState(new ChatClientDisconnectedState());
        }

        private void OnDestroy()
        {
            SwitchState(null);
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

        public void SwitchState([CanBeNull] IChatClientState state)
        {
            _current?.OnExit(this);
            _current = state;
            _current?.OnEnter(this);
        }
    }
}