namespace Raptor.Chat.Client
{
    public interface IChatClientState
    {
        public void OnEnter(ChatClient chatClient);
        public void Present(ChatClient chatClient);
        public void OnExit(ChatClient chatClient);
    }
}