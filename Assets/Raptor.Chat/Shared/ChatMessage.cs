using System;

namespace Raptor.Chat.Shared
{
    [Serializable]
    public readonly struct ChatMessage
    {
        public string Content { get; }

        public ChatMessage(string content)
        {
            Content = content;
        }
    }
}