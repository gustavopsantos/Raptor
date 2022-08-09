namespace Raptor.Chat.Server
{
    public interface IChatServerState
    {
        public void OnEnter(ChatServer chatServer);
        public void Present(ChatServer chatServer);
        public void OnExit(ChatServer chatServer);
    }
}