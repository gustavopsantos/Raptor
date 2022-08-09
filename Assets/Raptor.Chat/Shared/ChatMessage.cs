using System;

namespace Raptor.Chat.Shared
{
    [Serializable]
    public class ChatMessage
    {
        public string Content { get; }

        public ChatMessage(string content)
        {
            Content = content;
        }
    }
}