using Raptor.Chat.Shared;

namespace Raptor.Chat.Client
{
    public class ChatClientConnectedState : IChatClientState
    {
        private readonly ChatPresenter _chatPresenter = new();

        public void OnEnter(ChatClient chatClient)
        {
            chatClient.Client.RegisterMessageHandler<ChatMessage>(_chatPresenter.Add);
        }

        public void Present(ChatClient chatClient)
        {
            _chatPresenter.Present(chatClient);
        }

        public void OnExit(ChatClient chatClient)
        {
            chatClient.Client.Dispose();
        }
    }
}