using System;
using System.Collections.Generic;
using UnityEngine;

namespace Raptor.Chat.Client
{
    public class ChatClientConnectedState : ChatClientState
    {
        private string _composing;
        private Vector2 _scrollPosition;
        private readonly List<string> _messages = new();

        public override void Present(ChatClient chatClient)
        {
            using (new GUILayout.VerticalScope("box"))
            {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(100));

                foreach (var message in _messages)
                {
                    GUILayout.Label(message);
                }

                GUILayout.EndScrollView();
            }

            using (new GUILayout.HorizontalScope())
            {
                _composing = GUILayout.TextField(_composing);

                if (GUILayout.Button("Send", GUILayout.Width(42)))
                {
                    Send();
                }
            }
        }

        private void Send()
        {
            AddMessage();
        }

        private void AddMessage()
        {
            var composed = _composing.Trim();

            if (composed == string.Empty)
            {
                return;
            }

            _scrollPosition.y = float.MaxValue; // scroll to bottom
            _messages.Add($"[{Environment.MachineName}] {composed}");
            _composing = string.Empty;
        }
    }
}