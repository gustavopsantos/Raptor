using System.Threading;
using Raptor.Chat.Shared;
using Raptor.Interface;

namespace Raptor.Chat.Server
{
    public class ChatMessageHandler : IMessageHandler<ChatMessage>
    {
        public void Handle(Message<ChatMessage> message)
        {
            message.Mean.BroadcastMessageReliable(message.Payload, CancellationToken.None);
        }
    }
}